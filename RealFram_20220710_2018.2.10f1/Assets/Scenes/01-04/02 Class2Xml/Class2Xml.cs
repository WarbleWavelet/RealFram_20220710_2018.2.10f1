using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using FileMode = System.IO.FileMode;



public class Class2Xml : MonoBehaviour
{

    void Start()
    {
        //SerilizeTest(DefinePath.path_Xml);
        DeSerilizerTest(DefinePath.path_Xml);
    }




    #region 辅助

    #region Class2Xml

    void SerilizeTest(string path)
    {
        TestABConfig testSerilize = new TestABConfig();
        testSerilize.Id = 1;
        testSerilize.Name = "测试";
        testSerilize.Lst = new List<int>();
        testSerilize.Lst.Add(2);
        testSerilize.Lst.Add(3);
        XmlSerilize(testSerilize, path);
    }

    /// <summary>
    /// xml序列化
    /// </summary>
    /// <param name="testSerilize"></param>
    /// <param name="path"></param>
    void XmlSerilize(TestABConfig testSerilize, string path)
    {
        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
        XmlSerializer xml = new XmlSerializer(testSerilize.GetType());
        xml.Serialize(sw, testSerilize);
        sw.Close();
        fs.Close();

        //  AssetDatabase.Refresh();
    }
    #endregion



    #region Xml2Class
    void DeSerilizerTest(string path)
    {
        TestABConfig testSerilize = XmlDeSerilize(path);
        Debug.Log(testSerilize.Id + "   " + testSerilize.Name);
        foreach (int a in testSerilize.Lst)
        {
            Debug.Log(a);
        }
    }

    /// <summary>
    /// Xm反序列化
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    TestABConfig XmlDeSerilize(string path)
    {
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        XmlSerializer xs = new XmlSerializer(typeof(ABConfig));
        TestABConfig testSerilize = (TestABConfig)xs.Deserialize(fs);
        fs.Close();
        return testSerilize;
    }
    #endregion


    #endregion


}

[System.Serializable]
public class TestABConfig
{
    [XmlAttribute("Id")]
    public int Id { get; set; }
    [XmlAttribute("Name")]
    public string Name { get; set; }
    [XmlElement("List")]
    public List<int> Lst { get; set; }
}



