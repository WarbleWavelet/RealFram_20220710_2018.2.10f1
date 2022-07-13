/****************************************************
    文件：AssetBundleMgr.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/13 2:20:11
	功能：AB管理器
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class AssetBundleMgr : Singleton<AssetBundleMgr>
{


  static  ABConfig cfg = new ABConfig();
  static  Dictionary<uint, Mediator> medDic = new Dictionary<uint, Mediator>();


   static void Reset()
    {
        medDic.Clear();
        cfg = BinaryDeserilize();
    }
    public static void LoadAB()
    {
        Reset();

        //
        for (int i = 0; i < cfg.ABLst.Count; i++)
        {
            ABBase abBase = cfg.ABLst[i];


            if (medDic.ContainsKey(abBase.Crc))
            {
                Debug.LogErrorFormat("Key重复，AB包名 {0},资源名 {1}", abBase.ABName, abBase.AssetName);
            }
            else
            {
                Mediator mediator = new Mediator
                {
                    m_Crc = abBase.Crc,
                    m_AssetBundleName = abBase.ABName,
                    m_AssetName = abBase.AssetName,
                    m_dAssetBundleDepen = abBase.ABDependce

                };
                medDic.Add(mediator.m_Crc, mediator);

                Debug.Log(mediator.ToString());
            }



        }
        //加载依赖
        //for (int i = 0; i < abBase.ABDependce.Count; i++)
        //{
        //    AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + abBase.ABDependce[i]);
        //}
        //AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + abBase.ABName);
        //GameObject prefab = ab.LoadAsset<GameObject>("Attack");//注意加载的是ab，不是预制体，所以都小写

        //GameObject go = new GameObject();
        //Instantiate(prefab);
        
        //FixShader(go);
    }

    /// <summary>
    /// /反序列化存储的Cfg
    /// </summary>
    /// <returns></returns>
 static   ABConfig BinaryDeserilize()
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

    void FixShader(GameObject go)
    {
        MeshRenderer[] mr = go.GetComponentsInChildren<MeshRenderer>();
        List<Material> lst = new List<Material>();
        for (int i = 0; i < mr.Length; i++)
        {
            lst.AddRange(mr[i].materials);
        }
        SkinnedMeshRenderer[] smr = go.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < smr.Length; i++)
        {
            lst.AddRange(smr[i].materials);
        }

        for (int i = 0; i < lst.Count; i++)
        {
            lst[i].shader = Shader.Find("Custom/benghuai");
        }
    }
}
/// <summary>
/// 存储中间类
/// </summary>

public class Mediator
{ 
    /// <summary>检验码</summary>
    public uint m_Crc = 0;
    /// <summary>名字 </summary>
    public string m_AssetName = string.Empty;
    /// <summary>所在包名</summary>
    public string m_AssetBundleName = string.Empty;
    /// <summary>加载完的AB包</summary>
    public AssetBundle m_AssetBundle = null;
    /// <summary>依赖包</summary>
    public List<string> m_dAssetBundleDepen = new List<string>();



    public override string ToString()
    {
        string str = "";
        str += "\t" + "检验码" + m_Crc;
        str += "\t" + "所在包名" + m_AssetBundleName;
        str += "\t" + "名字" + m_AssetName;
        str += "\t" + "加载完的AB包" + m_AssetBundle;
        str += "\t" + "依赖包" + m_dAssetBundleDepen;

        return str;
    }
}