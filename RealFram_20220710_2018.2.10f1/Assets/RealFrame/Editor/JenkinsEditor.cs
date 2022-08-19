/****************************************************
    文件：JenkinsEditor.cs
	作者：lenovo
    邮箱: 
    日期：2022/8/19 9:3:29
	功能：Jenkins交互
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class JenkinsEditor : Editor
{

    [MenuItem(DefinePath.MenuItem_Jenkins + "写入txt当前日期", false, 100)]
    static void MenuItem_WriteTxt()
    {
        FileInfo fi = new FileInfo(DefinePath.RealFrameRoot+ "输出可删除/Test_Jenkins_写入txt当前日期.txt");
        StreamWriter sw = fi.CreateText();
        sw.WriteLine(System.DateTime.Now);

        sw.Close();
        sw.Dispose();

        AssetDatabase.Refresh();
    }

}
