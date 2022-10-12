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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

public class AssetBundleMgr : Singleton<AssetBundleMgr>
{



    #region 字属


    ABCfg m_abCfg = new ABCfg();
    /// <summary>资源数据</summary>
    Dictionary<uint, ResItem> m_resItemDic = new Dictionary<uint, ResItem>();
    /// <summary>已加载的AB包</summary> 
    Dictionary<uint, ABItem> m_abItemDic = new Dictionary<uint, ABItem>();
    ClassObjectPool<ABItem> m_abItemPool = ObjectMgr.Instance.GetOrNewClassObjectPool<ABItem>(Constants.ClassObjectPool_MAXCNT);

    private static string m_assetbundleconfigPath_Local_Download = @"C:\Users\lenovo\AppData\LocalLow\DefaultCompany\RealFrame_Test\DownLoad\assetbundleconfig";
    private static string m_assetbundleconfigPath_Local_Origin = @"C:\Users\lenovo\AppData\LocalLow\DefaultCompany\RealFrame_Test\Origin\assetbundleconfig";
    private static string m_assetbundleconfigPath_Inner = DefinePath.assetbundleconfig_Inner;
    private static string m_assetbundleconfigPath_Outter = @"D:\Data\Projects\Unity\Ocean_RealFram_20220710_2018.2.10f1\RealFram_20220710_2018.2.10f1\AssetBundle\assetbundleconfig";
    private static string m_assetbundleconfigPath = "";
    private static string m_assetbundleconfigName = "assetbundleconfig";
    private static string m_abCfg_Bytes = DefinePath.abCfg_Bytes;       //AssetBundleConfig.bytes
    private static string m_PrivateKey=Constants.PrivateKey;    



    #endregion



    #region 生命



    /// <summary>
    ///  加载配置文件LoadABCfg
    /// </summary>
    /// <param name="log">打印日志</param>
    public void InitMgr(bool log = true)
    {

#if UNITY_EDITOR //    EDITOR 才有判断 GetLoadFromAB()的必要
        if (ResourceMgr.Instance.GetLoadFromAB() == false)
        {
            return;
        }
#endif
        m_resItemDic.Clear();




        //
        try//报错跑到底层方法，找起来慢
        {
            LoadABCfg(ABLoadType.AES_Inner, out m_abCfg);
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("assetbundle解析错误，路径类型是{0}", ABLoadType.AES_Inner); 
        }
     

        //
        for (int i = 0; i < m_abCfg.ABLst.Count; i++)
        {
            ABBase abBase = m_abCfg.ABLst[i];

            if (m_resItemDic.ContainsKey(abBase.Crc))
            {

                Debug.LogErrorFormat("Key重复，AB包名 {0},资源名 {1}", abBase.ABName, abBase.AssetName);
            }
            else
            {
                ResItem resItem = new ResItem
                {
                    m_Crc = abBase.Crc,
                    m_ABName = abBase.ABName,
                    m_AssetName = abBase.AssetName,
                    m_ABDepend = abBase.ABDependce,
                    RefCnt = 0

                };
                m_resItemDic.Add(resItem.m_Crc, resItem);
                if (log)
                {
                    Debug.Log(resItem.ToString());
                }
            }
        }
    }



    #endregion


    #region AB ABItem

    ABItem ABItem_TryGetValue(uint crc)
    {
        ABItem abItem = null;
        m_abItemDic.TryGetValue(crc, out abItem);

        return abItem;
    }
    /// <summary>
    /// 加载AssetBundle
    /// </summary>
    /// <param name="ABName"></param>
    /// <returns></returns>
    public AssetBundle AB_Get(string ABName)//Dic或Ab包
    {
        uint crc = CRC32.GetCRC32(ABName);
        ABItem abItem = ABItem_TryGetValue(crc);
        if (abItem != null)//Get
        {
            abItem.m_RefCnt++;

        }
        else//Get不到就Load
        {
            abItem = ABItem_Load(ABName);
        }
        return abItem.m_AB;
    }


    /// <summary>
    /// Get不到就Load
    /// </summary>
    /// <param name="abName"></param>
    /// <returns></returns>
    ABItem ABItem_Load(string abName)
    {
        uint crc = CRC32.GetCRC32(abName);


      
        string path = "";
        string hotfixPath = HotPatchMgr.Instance.Exist_assetbundleconfig_Hotfix(abName);
        if (String.IsNullOrEmpty(hotfixPath) == false)
        {
            path = hotfixPath;//热更路径
        }
        else
        {
            path = DefinePath.Path_AB_Inner + abName;//暂时用内部路径做本地路径
        }  
        // path = LoadABPath + abName;
        path = DefinePath.LocalPath_Origin   + abName;
        path = DefinePath.Path_AB_Inner + abName;


        AssetBundle ab;
        if (false)
        {
             ab = AssetBundle.LoadFromFile(path);//有可能ab解析错误
        }
        else
        {
            byte[] bytes = AES.AESFileByteDecrypt(path, m_PrivateKey);
             ab = AssetBundle.LoadFromMemory(bytes);
        }
       


        if (ab == null)
        {
            Debug.LogErrorFormat("Load ab Error：path not exist:{0}", path);

        }
        ABItem abItem = ABItem_Spawn(true);
        abItem.m_AB = ab;
        abItem.m_RefCnt++;
        m_abItemDic.Add(crc, abItem);

        return abItem;
    }

    ABItem ABItem_Spawn(bool createEmptyPool = true)
    {
        return m_abItemPool.Spawn(createEmptyPool);
    }

    /// <summary>
    /// 卸载资源引用的AB
    /// </summary>
    /// <param name="name"></param>
    void AB_Unload(string name)
    {
        uint crc = CRC32.GetCRC32(name);
        ABItem abItem = null;
        if (m_abItemDic.TryGetValue(crc, out abItem) && abItem != null)//加载
        {
            abItem.m_RefCnt--;
            if (abItem.m_RefCnt <= 0 && abItem.m_AB != null)
            {
                abItem.m_AB.Unload(true);//Unloads all assets in the bundle.
                abItem.Reset();
                m_abItemPool.Recycle(abItem);
                m_abItemDic.Remove(crc);
            }
        }
    }

    /// <summary>
    /// 加载依赖,load依赖AB包
    /// </summary>
    /// <param name="resItem"></param>
    void LoadDepend(ref ResItem resItem)
    {
        if (resItem.m_ABDepend != null)
        {
            for (int i = 0; i < resItem.m_ABDepend.Count; i++)
            {
                AB_Get(resItem.m_ABDepend[i]);
            }
        }

    }
    #endregion  


    #region ResItem
    /// <summary>
    /// GetResItem不到就LoadResItem
    /// </summary>
    /// <param name="crc"></param>
    /// <returns></returns>
    public ResItem ResItem_TryGetValue(uint crc)
    {
        ResItem resItem = null;
        if (m_resItemDic == null || m_resItemDic.Count==0)
        {
            //Debug.LogErrorFormat("ABMgr字典未初始化");
            return null;
        }
        m_resItemDic.TryGetValue(crc, out resItem);
        if (resItem == null)
        {
            //Debug.LogErrorFormat("ABMgr字典中没找到resItem");
            return null;
        }
        else
        {
            Debug.LogFormat("路径{2}\n查找{0}\n同名{1}", crc, resItem.m_Crc, resItem.m_AssetName);
        }
        return resItem;

    }



    /// <summary>
    /// 不能GetResItem就LoadResItem
    /// </summary>
    /// <param name="crc"></param>
    /// <returns></returns>
    public ResItem ResItem_Load(uint crc)
    {
        ResItem resItem = ResItem_TryGetValue(crc);
        if (resItem == null)
        {
            return null;
        }
        //if (resItem.m_AB != null)//这里引用了，直接返回进行不了计数
        //{
        //    return resItem;
        //}
        //未有
        resItem.m_AB = AB_Get(resItem.m_ABName);
        LoadDepend(ref resItem);

        return resItem;
    }







    /// <summary>
    /// 卸载AB包
    /// </summary>
    /// <param name="resItem"></param>
    public void AB_Unload(ResItem resItem)
    {
        if (resItem == null)
        {
            return;
        }

        List<string> lst = resItem.m_ABDepend;
        if (lst != null && lst.Count > 0)
        {
            for (int i = 0; i < lst.Count; i++)
            {
                AB_Unload(lst[i]);
            }

        }
        AB_Unload(resItem.m_ABName);
    }




    #endregion





    #region 辅助





    /// <summary>
    /// /反序列化存储的Cfg
    /// </summary>
    /// <returns></returns>
    static ABCfg Bin2ABCfg(string assetbundleconfigName)
    {
        ABCfg m_abCfg = null;
        AssetBundle ab = AssetBundle.LoadFromFile(assetbundleconfigName);//Load AB
        if (ab == null)
        {

            Debug.LogErrorFormat("该路径不存在AB包{0}", assetbundleconfigName);
        }
        TextAsset ta = ab.LoadAsset<TextAsset>(m_abCfg_Bytes);//load bytes
        if (ta == null)
        {
            Debug.LogErrorFormat(" \"{0}\" is not exist", assetbundleconfigName);

        }

        MemoryStream stream = new MemoryStream(ta.bytes);
        BinaryFormatter bf = new BinaryFormatter();
        m_abCfg = (ABCfg)bf.Deserialize(stream);
        stream.Close();

        return m_abCfg;

    }

    static ABCfg Bin2ABCfg(byte[] bytes)
    {
        if (bytes == null)
        {
            Debug.LogErrorFormat("assetbundleconfig的长度为", bytes.Length);
        }
        ABCfg m_abCfg = null;
        AssetBundle ab = AssetBundle.LoadFromMemory(bytes);//Load AB
        if (ab == null)
        {

             // Debug.LogErrorFormat("该路径不存在AB包{0}", assetbundleconfig);
        }
        TextAsset ta = ab.LoadAsset<TextAsset>(m_abCfg_Bytes);//load bytes
        if (ta == null)
        {
            // Debug.LogErrorFormat(" \"{0}\" is not exist", assetbundleconfig);

        }

        MemoryStream stream = new MemoryStream(ta.bytes);
        BinaryFormatter bf = new BinaryFormatter();
        m_abCfg = (ABCfg)bf.Deserialize(stream);
        stream.Close();
        return m_abCfg;

    }


    void LoadABCfg(ABLoadType m_abLoadType, out ABCfg m_abCfg)
    {


            

        m_abCfg = new ABCfg();
        if (String.IsNullOrEmpty(HotPatchMgr.Instance.Exist_assetbundleconfig_Hotfix(m_assetbundleconfigName)) == false)
        {
            //m_abLoadType = ABLoadType.Local_DownLoad;
        }
        switch (m_abLoadType)
        {
            case ABLoadType.Inner:
                {
                    m_abCfg = Bin2ABCfg(m_assetbundleconfigPath_Inner);
                }
                break;
            case ABLoadType.Local_DownLoad: //本地资源下载。不用AES
                {
                    m_abCfg = Bin2ABCfg(m_assetbundleconfigPath_Local_Download);
                }
                break;
            case ABLoadType.AES_Inner:
                {
                    byte[] bytes = AES.AESFileByteDecrypt(m_assetbundleconfigPath_Inner, m_PrivateKey);
                    m_abCfg = Bin2ABCfg(bytes);
                }
                break;

            case ABLoadType.AES_Local_Origin: //安卓时使用 ,解压
                {
                    byte[] bytes = AES.AESFileByteDecrypt(m_assetbundleconfigPath_Local_Origin, m_PrivateKey);
                    m_abCfg = Bin2ABCfg(bytes);
                }
                break;
            default: break;

        }
    }




    #endregion 


}

#region 数据类

/// <summary>
/// 存储中间类（关键）。对接AB包（ABBase）和UnityEngine.Object(在Unty运行时的使用情况)  <para />
/// using AB==AssetBundle  <para />
/// public uint m_Crc = 0;  <para />
/// public string m_AssetName = string.Empty;   <para />
/// public string m_ABName = string.Empty;<para />
/// public List string m_ABDepend   <para />
/// public AssetBundle m_AB = null;  <para />
/// public int m_GUID = 0;  <para />
/// public Object m_Obj = null;  <para />
/// public float m_LastUseTime = 0.0f;  <para />
/// public bool m_JmpClr = true;
/// </summary>
public class ResItem//Ocean命名为ResItem。还是ResItem吧。 Asset是在硬盘上的，还没入Cache
{

    #region AB
    /// <summary>检验码</summary>
    public uint m_Crc = 0;

    /// <summary>名字 </summary>
    public string m_AssetName = string.Empty;

    /// <summary>所在包名</summary>
    public string m_ABName = string.Empty;

    /// <summary>依赖包</summary>
    public List<string> m_ABDepend = new List<string>();

    /// <summary>加载完的AB包</summary>
    public AssetBundle m_AB = null;

    #endregion

    #region UnityEngine.Object
    /// <summary>ResItem唯一标识</summary>
    public int m_Guid = 0;

    /// <summary>
    /// 资源对象（prefab,图片，音频，Unity中一切物体继承于UnityEngine.Object） <para /> 
    ///Unity中一切物体（比如GameObject）继承于UnityEngine.Object，区别于System.object <para /> 
    ///</summary>
    public UnityEngine.Object m_Obj = null;

    /// <summary>上次使用该资源（RefCnt++）时间</summary>
    public float m_LastUseTime = 0.0f;

    /// <summary>跳砖场景时（Clear）需不需要清除</summary> 
    public bool m_JmpClr = true;


    #region m_RefCnt
    /// <summary>引用计数</summary>
    int m_RefCnt = 0;
    public int RefCnt
    {
        get
        {
            return m_RefCnt;
        }

        set
        {
            m_RefCnt = value;
            if (m_RefCnt < 0)
            {
                Debug.LogErrorFormat("RefCnt Err:{0}", m_RefCnt);
            }

        }
    }
    #endregion

    #endregion


    public ResItem()
    {

    }
    public ResItem(uint crc)
    {
        this.m_Crc = crc;
    }


    public override string ToString()
    {
        string str = "";
        str += "\t" + "检验码" + m_Crc;
        str += "\t" + "所在包名" + m_ABName;
        str += "\t" + "名字" + m_AssetName;
        str += "\t" + "加载完的AB包" + m_AB;
        str += "\t" + "依赖包" + m_ABDepend;

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
    public int m_RefCnt;

    public void Reset()
    {
        m_AB = null;
        m_RefCnt = 0;
    }
}
#endregion



/// <summary>
/// ab加载路径
/// </summary>
public enum Path_assetbundleconfig
{
    Inner, //Assets/RealFrame/StreamingAsset/平台
    Outter,//AssetBundle
    Hotfix // Application.persistentDataPath

}


/// <summary>
/// ab加载方式
/// </summary>
public enum ABLoadType
{
    Inner,
    AES_Inner,
    AES_Local_Origin,
    Local_DownLoad,
     
}

