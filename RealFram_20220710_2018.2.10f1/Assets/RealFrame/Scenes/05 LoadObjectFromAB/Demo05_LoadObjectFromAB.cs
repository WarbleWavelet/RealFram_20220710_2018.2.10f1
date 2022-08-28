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
using Object = UnityEngine.Object;
using UnityEngine.UI;

namespace Demo05
{
    public class Demo05_LoadObjectFromAB : MonoBehaviour
    {
        public Button btn1;
        void Start()
        {

            btn1.onClick.AddListener(() =>
            {
                ABCfg cfg = BinaryDeserilize<ABCfg>( DefinePath.Demo05_Bin_AssetBundleConfig );
                Object obj = LoadObjectFromAB( cfg, DefinePath.Demo05_Prefab_Attack, "Attack"  );

                GameObject go = Instantiate(obj) as GameObject;
                Common.FixShader(go, DefinePath.Shader_BengHuai);
            });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectPath">资产在的目录</param>
        /// <param name="assetName">资产名</param>
        /// <returns></returns>
        private Object LoadObjectFromAB(ABCfg cfg, string objectPath, string objectName)
        {
            uint crc = CRC32.GetCRC32(objectPath);

            ABBase abBase = new ABBase();         
            for (int i = 0; i < cfg.ABLst.Count; i++) //遍历ABLst
            {
                if (cfg.ABLst[i].Crc == crc)
                {
                    abBase = cfg.ABLst[i];
                }
            }
          
              for (int i = 0; i < abBase.ABDependce.Count; i++)  //加载依赖
            {                                        
                AssetBundle.LoadFromFile(Application.dataPath + "/" + DefinePath.RealFrameName + "/StreamingAssets/" + abBase.ABDependce[i]);
            }


            AssetBundle ab = AssetBundle.LoadFromFile(Application.dataPath+"/"+DefinePath.RealFrameName + "/StreamingAssets/" + abBase.ABName);
            return ab.LoadAsset<Object>(objectName);//注意加载的是ab，不是预制体，所以都小写

        }

        /// <summary>
        /// 存储的Cfg
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">之前打包的bytes（现在打包又包括它）</param>
        /// <param name="abCfgName"></param>
        /// <returns></returns>
        T BinaryDeserilize<T>(string path )
        {
            path = DefinePath.Demo05_AB_assetbundleconfig ;
            string name = Common.TrimName( DefinePath.Demo05_Bin_AssetBundleConfig, TrimNameType.SlashAndPoint);


            AssetBundle ab = AssetBundle.LoadFromFile(path);
            TextAsset ta = ab.LoadAsset<TextAsset>(name);
            MemoryStream stream = new MemoryStream(ta.bytes);
            BinaryFormatter bf = new BinaryFormatter();
            T cfg = (T)bf.Deserialize(stream);
            stream.Close();

            return cfg;
        }


        private void OnApplicationQuit()
        {
            Debug.Log("OnApplicationQuit");

            //#if UNITY_EDITOR//这样写才对，不能括外面
            ResourceMgr.Instance.ClearAllResItem();
            Resources.UnloadUnusedAssets();
            Debug.Log("清存");
            //#endif
        }

    }

}



