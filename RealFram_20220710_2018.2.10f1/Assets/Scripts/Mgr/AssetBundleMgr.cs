/****************************************************
    文件：AssetBundleMgr.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/13 2:20:11
	功能：AB管理器
       1：读取AssetBundle配置表
       2：设置中间类进行引用计数
       3：根据路径加载AssetBundle
       4：根据路径卸载AssetBundle
       5：为ResourceManager提供加载中间类，根据中间类释放资源，查找中间类等方法

*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class AssetBundleMgr : Singleton<AssetBundleMgr>
{


    ABConfig cfg = new ABConfig();
    /// <summary>资源数据</summary>
    Dictionary<uint, ResItem> resItemDic = new Dictionary<uint, ResItem>();
    /// <summary>已加载的AB包</summary> 
    Dictionary<uint, ABItem> abItemDic = new Dictionary<uint, ABItem>();
    ClassObjectPool<ABItem> abItemPool = ObjectMgr.Instance.TryGetClassObjectPool<ABItem>(Constants.ClassobjectPool_MAXCNT);

    void Reset()
    {
        resItemDic.Clear();
        cfg = BinaryDeserilize();
    }

    /// <summary>
    /// 加载配置文件
    /// </summary>
   public  void LoadABCfg()
    {
        Reset();
        //
        for (int i = 0; i < cfg.ABLst.Count; i++)
        {
            ABBase abBase = cfg.ABLst[i];

            if (resItemDic.ContainsKey(abBase.Crc))
            {
                Debug.LogErrorFormat("Key重复，AB包名 {0},资源名 {1}", abBase.ABName, abBase.AssetName);
            }
            else
            {
                ResItem mediator = new ResItem
                {
                    m_Crc = abBase.Crc,
                    m_AssetBundleName = abBase.ABName,
                    m_AssetName = abBase.AssetName,
                    m_AssetBundleDepend = abBase.ABDependce

                };
                resItemDic.Add(mediator.m_Crc, mediator);

                Debug.Log(mediator.ToString());
            }
        }
    }


    #region ResItem
    /// <summary>
    /// 获取资源
    /// </summary>
    /// <param name="crc"></param>
    /// <returns></returns>

    public ResItem LoadResItem(uint crc)
    {
        ResItem mediator = null;
        if (resItemDic.TryGetValue(crc, out mediator) == false || mediator == null)
        {
            return null;
        }
        if (mediator.m_AssetBundle != null)//已有
        {
            return mediator;
        }
        //未有
        AssetBundle ab = LoadAB(mediator.m_AssetBundleName);
        LoadDepend(mediator);

        return mediator;


    }

    public ResItem GetResItem(uint crc)
    {
        ResItem resItem = null;
        resItemDic.TryGetValue(crc, out resItem);
        return resItem;

    }
    #endregion





    #region 删
    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <param name="mediator"></param>
    public void ReleaseResItem(ResItem mediator)
    {
        if (mediator == null)
        {
            return;
        }

        List<string> dpLst = mediator.m_AssetBundleDepend;
        if (dpLst != null && dpLst.Count > 0)
        {
            for (int i = 0; i < dpLst.Count; i++)
            {
                UnloadAsserBundle(dpLst[i]);
            }

        }
    }

    /// <summary>
    /// 卸载资源引用的AB
    /// </summary>
    /// <param name="name"></param>
    public void UnloadAsserBundle(string name)
    {
        uint crc = CRC32.GetCRC32(name);
        ABItem abItem = null;
        if (abItemDic.TryGetValue(crc, out abItem) && abItem != null)//加载
        {
            abItem.m_refCnt--;
            if (abItem.m_refCnt <= 0 && abItem.m_AB != null)
            {
                abItem.m_AB.Unload(true);//Unloads all assets in the bundle.
                abItem.Reset();
                abItemPool.Recycle(abItem);
                abItemDic.Remove(crc);
            }

        }
    }

    #endregion

    #region 辅助

    /// <summary>
    /// /反序列化存储的Cfg
    /// </summary>
    /// <returns></returns>
    ABConfig BinaryDeserilize()
    {
        AssetBundle ab = AssetBundle.LoadFromFile(DefinePath.ABSAVEPATH_Bin);
        TextAsset ta = ab.LoadAsset<TextAsset>(Constants.AssetBundleConfig);
        if (ta == null)
        {
            Debug.LogErrorFormat(" \"{0}\" is not exist", DefinePath.ABSAVEPATH_Bin);

            return null;
        }

        MemoryStream stream = new MemoryStream(ta.bytes);
        BinaryFormatter bf = new BinaryFormatter();
        ABConfig cfg = (ABConfig)bf.Deserialize(stream);
        stream.Close();

        return cfg;
    }


    /// <summary>
    /// 加载AssetBundle
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
   public AssetBundle LoadAB(string name)
    {
        uint crc = CRC32.GetCRC32(name);
        ABItem abItem = null;
        if (abItemDic.TryGetValue(crc, out abItem) == false)//加载
        {

            string path = Application.streamingAssetsPath + "/" + name;
            AssetBundle ab = null;
            if (File.Exists(path))
            {
                ab = AssetBundle.LoadFromFile(path);
            }
            else
            {
                Debug.LogErrorFormat("Load AB Error：path not exist:{0}", path);
            }
            //

            if (abItem == null)
            {
                Debug.LogErrorFormat("Load ABItem Error：path not exist:{0}", path);
            }

            abItem = abItemPool.Spawn(true);
            abItem.m_AB = ab;
            abItem.m_refCnt++;
            abItemDic.Add(crc, abItem);

        }
        else
        {
            abItem.m_refCnt++;
        }
        return abItem.m_AB;
    }



    /// <summary>
    /// 加载依赖
    /// </summary>
    /// <param name="resItem"></param>
    void LoadDepend(ResItem resItem)
    {
        if (resItem.m_AssetBundleDepend != null)
        {
            for (int i = 0; i < resItem.m_AssetBundleDepend.Count; i++)
            {
                LoadAB(resItem.m_AssetBundleDepend[i]);
            }
        }

    }

    #endregion


}



#region 数据类
/// <summary>
/// 存储中间类（关键）。对接AB包（ABBase）和UnityEngine.Object(在Unty运行时的使用情况)
/// </summary>

public class ResItem//Ocean命名为ResItem。还是ResItem吧。 Asset是在硬盘上的，还没入Cache
{

    #region AB
    /// <summary>检验码</summary>
    public uint m_Crc = 0;

    /// <summary>名字 </summary>
    public string m_AssetName = string.Empty;

    /// <summary>所在包名</summary>
    public string m_AssetBundleName = string.Empty;

    /// <summary>依赖包</summary>
    public List<string> m_AssetBundleDepend = new List<string>();
    #endregion

    #region UnityEngine.Object
    /// <summary>ResItem唯一标识</summary>
    public int m_GUID = 0;
    /// <summary>加载完的AB包</summary>
    public AssetBundle m_AssetBundle = null;

    /// <summary> 资源对象  </summary>
    public Object m_Obj=null;

    /// <summary>上次使用时间</summary>
    public float m_LastUseTime = 0.0f;

    /// <summary>引用计数</summary>
     int m_RefCnt=0;
    public int RefCnt
    {
        get
        {
            return m_RefCnt;
        }

        set
        {
            if (m_RefCnt < 0)
            {
                Debug.LogErrorFormat("RefCnt Err:{0}",m_RefCnt);
            }
            m_RefCnt = value;
        }
    }
    #endregion






    public override string ToString()
    {
        string str = "";
        str += "\t" + "检验码" + m_Crc;
        str += "\t" + "所在包名" + m_AssetBundleName;
        str += "\t" + "名字" + m_AssetName;
        str += "\t" + "加载完的AB包" + m_AssetBundle;
        str += "\t" + "依赖包" + m_AssetBundleDepend;

        return str;
    }
}

/// <summary>
/// A B都引用了C,卸载时，防止卸载C
/// </summary>
public class ABItem
{
    public AssetBundle m_AB;
    /// <summary>引用计数</summary> 
    public int m_refCnt;

    public void Reset()
    {
        m_AB = null;
        m_refCnt = 0;
    }
}
#endregion
