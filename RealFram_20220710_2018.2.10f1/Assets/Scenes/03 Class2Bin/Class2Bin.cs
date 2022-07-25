/****************************************************
    文件：Class2Bin.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/11 14:11:6
	功能：二进制的序列化与反序列化
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


namespace Demo03
{
    public class Class2Bin : MonoBehaviour
    {
        public Button btn1;
        public Button btn2;
        void Start()
        {


            btn1.onClick.AddListener(A);
            btn2.onClick.AddListener(B);



        }

        void A()
        {
            XmlCfg cfg = new XmlCfg();
            cfg.Id = 5;
            cfg.Name = "二进制测试";
            cfg.Lst = new List<int>();
            cfg.Lst.Add(10);
            cfg.Lst.Add(18);
            //
            BinarySerilize(cfg, DefinePath.Demo03_WriteBytes);
        }


        void B()
        {
            XmlCfg cfg = BinaryDeserilize(DefinePath.Demo03_ReadBytes);
            Debug.Log(cfg.Id + "   " + cfg.Name);
            if (cfg != null)
            { 
                foreach (int a in cfg.Lst)
                {
                    Debug.Log(a);
                }            
            }

        }


        void BinarySerilize(XmlCfg serilize, string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, serilize);
            fs.Close();

#if UNITY_EDITOR
             AssetDatabase.Refresh();
#endif


        }

        XmlCfg BinaryDeserilize(string path)
        {
            XmlCfg cfg = null;
#if UNITY_EDITOR
            TextAsset ta = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            MemoryStream stream = new MemoryStream(ta.bytes);
            BinaryFormatter bf = new BinaryFormatter();
             cfg = (XmlCfg)bf.Deserialize(stream);
            stream.Close();

            
         
#endif

                 return cfg;
        }
    }

}
