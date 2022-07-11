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

 void SerilizeTest( string path)
    {
        TestSerilize testSerilize = new TestSerilize();
        testSerilize.Id = 1;
        testSerilize.Name = "测试";
        testSerilize.List = new List<int>();
        testSerilize.List.Add(2);
        testSerilize.List.Add(3);
        XmlSerilize(testSerilize,path);
    }

    /// <summary>
    /// xml序列化
    /// </summary>
    /// <param name="testSerilize"></param>
    /// <param name="path"></param>
    void XmlSerilize(TestSerilize testSerilize,string path)
    {
        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
        XmlSerializer xml = new XmlSerializer(testSerilize.GetType());
        xml.Serialize(sw, testSerilize);
        sw.Close();
        fs.Close();

        AssetDatabase.Refresh();
    }
    #endregion



    #region Xml2Class
 void DeSerilizerTest(string path)
    {
        TestSerilize testSerilize = XmlDeSerilize(path);
        Debug.Log(testSerilize.Id + "   " + testSerilize.Name);
        foreach (int a in testSerilize.List)
        {
            Debug.Log(a);
        }
    }

    /// <summary>
    /// Xm反序列化
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    TestSerilize XmlDeSerilize(string path)
    {
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        XmlSerializer xs = new XmlSerializer(typeof(TestSerilize));
        TestSerilize testSerilize = (TestSerilize)xs.Deserialize(fs);
        fs.Close();
        return testSerilize;
    }
    #endregion
   

    #endregion


}


[System.Serializable]
public class TestSerilize
{
	[XmlAttribute("Id")]
	public int Id { get; set; }
	[XmlAttribute("Name")]
	public string Name { get; set; }
	[XmlElement("List")]
	public List<int> List { get; set; }
}
