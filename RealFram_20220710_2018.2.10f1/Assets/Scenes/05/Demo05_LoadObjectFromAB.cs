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
                ABCfg cfg = BinaryDeserilize<ABCfg>( DefinePath.Demo05_Bytes_Cfg );
                Object obj = LoadObjectFromAB( cfg, DefinePath.Demo04_Attack_Prefab, "Attack"  );

                GameObject go = Instantiate(obj) as GameObject;
                Common.FixShader(go);
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
                AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + abBase.ABDependce[i]);
            }


            AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + abBase.ABName);
            return ab.LoadAsset<Object>(objectName);//注意加载的是ab，不是预制体，所以都小写

        }

        /// <summary>
        /// 存储的Cfg
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cfgABPath">之前打包的bytes（现在打包又包括它）</param>
        /// <param name="abCfgName"></param>
        /// <returns></returns>
        T BinaryDeserilize<T>(string cfgABPath )
        {
            string abCfgName = Common.TrimName(cfgABPath, TrimNameType.Slash);
            AssetBundle ab = AssetBundle.LoadFromFile(cfgABPath);
            TextAsset ta = ab.LoadAsset<TextAsset>(abCfgName);
            MemoryStream stream = new MemoryStream(ta.bytes);
            BinaryFormatter bf = new BinaryFormatter();
            T cfg = (T)bf.Deserialize(stream);
            stream.Close();

            return cfg;
        }




    }

}



