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
        WriteTxt(DefinePath.RealFrameRoot + "输出可删除/Test_Jenkins_写入txt当前日期.txt", System.DateTime.Now.ToString());
    }

    [MenuItem(DefinePath.MenuItem_Jenkins + "压缩程序包", false, 100)]
    static void MenuItem_Build_PC()
    {
        string compressedPackageName = GetCompressedPackageName();
        WriteTxt(DefinePath.AppBuildPath + "buildname.txt", compressedPackageName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath">全写，包括文件名和后缀</param>
    /// <param name="fileContent"></param>
    static void WriteTxt(string filePath,string fileContent)
    {
        FileInfo fi = new FileInfo( filePath );
        StreamWriter sw = fi.CreateText();
        sw.WriteLine( fileContent );

        sw.Close();
        sw.Dispose(); 
    }

    /// <summary>
    ///  压缩包的名字
    /// </summary>
    /// <returns></returns>
    public static string GetCompressedPackageName()
    {
        return AppEditor.GetABFolderName();

    }
}
