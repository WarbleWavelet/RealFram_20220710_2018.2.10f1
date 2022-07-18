/****************************************************
    文件：Class2Bin.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/11 14:11:6
	功能：二进制的序列化与反序列化
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;


public class Class2Bin_05 : MonoBehaviour
{
    ABCfg cfg=new ABCfg();
    void Start()
    {
        LoadAB();

        //



    }

    private void LoadAB()
    {
         cfg = BinaryDeserilize();
        uint crc = CRC32.GetCRC32(DefinePath.path_ADB);
        ABBase abBase = new ABBase();
        //
        for (int i = 0; i < cfg.ABLst.Count; i++)
        {
            if (cfg.ABLst[i].Crc == crc)
            {
                abBase = cfg.ABLst[i];
            }
        }
        //加载依赖
        for (int i = 0; i < abBase.ABDependce.Count; i++)
        {
            AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + abBase.ABDependce[i]);
        }
        AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + abBase.ABName);
        GameObject prefab = ab.LoadAsset<GameObject>("Attack");//注意加载的是ab，不是预制体，所以都小写

        GameObject go = Instantiate(prefab);
        //
        FixShader(go);
    }

    /// <summary>
    /// /存储的Cfg
    /// </summary>
    /// <returns></returns>
    ABCfg BinaryDeserilize()
    {
        //TextAsset ta = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/AssetBundleConfig.bytes");
        AssetBundle ab=AssetBundle.LoadFromFile(DefinePath.ABSAVEPATH_Bin);
        TextAsset ta = ab.LoadAsset<TextAsset>( Constants.AssetBundleConfig);
        MemoryStream stream = new MemoryStream(ta.bytes);
        BinaryFormatter bf = new BinaryFormatter();
        ABCfg cfg = (ABCfg)bf.Deserialize(stream);
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



