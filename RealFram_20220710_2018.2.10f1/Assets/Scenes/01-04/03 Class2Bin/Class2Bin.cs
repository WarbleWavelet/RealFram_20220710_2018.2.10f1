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

public class Class2Bin : MonoBehaviour 
{

	void Start () 
    {
        TestABConfig testSerilize = new TestABConfig();
        testSerilize.Id = 5;
        testSerilize.Name = "二进制测试";
        testSerilize.Lst = new List<int>();
        testSerilize.Lst.Add(10);
        testSerilize.Lst.Add(18);		
        //
        BinarySerilize(testSerilize,DefinePath.path_Bin_Write);
        //
        //StartCoroutine(A());


    }


   
    //IEnumerator A()
    //{
    //    yield return new WaitForSeconds(3f);
    //    TestSerilize testSerilize = BinaryDeserilize(DefinePath.path_Bin_Read);
    //    Debug.Log(testSerilize.Id + "   " + testSerilize.Name);
    //    foreach (int a in testSerilize.List)
    //    {
    //        Debug.Log(a);
    //    }
    //}

    void BinarySerilize(TestABConfig serilize,string path)
    {
        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, serilize);
        fs.Close();

       // AssetDatabase.Refresh();
    }

    //TestSerilize BinaryDeserilize(string path)
    //{
    //    TextAsset ta = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
    //    MemoryStream stream = new MemoryStream(ta.bytes);
    //    BinaryFormatter bf = new BinaryFormatter();
    //    TestSerilize testSerilize = (TestSerilize)bf.Deserialize(stream);
    //    stream.Close();

    //    return testSerilize;
    //}
}
