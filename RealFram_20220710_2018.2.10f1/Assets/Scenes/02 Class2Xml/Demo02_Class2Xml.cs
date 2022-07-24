using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using FileMode = System.IO.FileMode;


namespace Demo02
{

    public class Demo02_Class2Xml : MonoBehaviour
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
                 XmlCfg cfg = new XmlCfg
            { 
                Id = 1,
                Name = "dfsd",
                Lst = new List<int>(),
            };
            cfg.Lst.Add(2);
            cfg.Lst.Add(3);



            XmlSerilize(cfg, DefinePath.Demo02_Xml);   
        }

        void B()
        {
            XmlCfg _cfg = XmlDeSerilize<XmlCfg>(DefinePath.Demo02_Xml);
            Debug.Log(_cfg.Id + "   " + _cfg.Name);
            foreach (int a in _cfg.Lst)
            {
                Debug.Log(a);
            }
        }


        #region 辅助

        #region Class2Xml

        /// <summary>
        /// xml序列化
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="path"></param>
        void XmlSerilize<T>(T cfg, string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            XmlSerializer xml = new XmlSerializer(cfg.GetType());
            xml.Serialize(sw, cfg);
            sw.Close();
            fs.Close();

             AssetDatabase.Refresh();
        }
        #endregion



        #region Xml2Class


        /// <summary>
        /// Xm反序列化
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        T XmlDeSerilize<T>(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            XmlSerializer xs = new XmlSerializer(typeof(T));
            T cfg = (T)xs.Deserialize(fs);
            fs.Close();
            return cfg;
        }
        #endregion


        #endregion


    }




}

