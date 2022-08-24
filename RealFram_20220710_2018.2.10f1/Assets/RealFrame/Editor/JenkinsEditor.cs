/****************************************************
    文件：JenkinsEditor.cs
	作者：lenovo
    邮箱: 
    日期：2022/8/19 9:3:29
	功能：Jenkins交互
*****************************************************/

using System;
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
        //string compressedPackageName = GetCompressedPackageNameByUnity();//用unity的参数
        string compressedPackageName = GetCompressedPackageNameByJenkins(); //用Jenkins的参数
        string savePath = AppEditor.GetExecutableProgramPath(compressedPackageName );
        AppEditor.BuildApp(savePath);
        WriteTxt(DefinePath.AppBuildPath + "buildname.txt", compressedPackageName);
    }



    #region 辅助
    /// <summary>
    /// 向filePath写入fileContent
    /// </summary>
    /// <param name="filePath">全写，包括文件名和后缀</param>
    /// <param name="fileContent"></param>
    static void WriteTxt(string filePath, string fileContent)
    {
        FileInfo fi = new FileInfo(filePath);
        StreamWriter sw = fi.CreateText();
        sw.WriteLine(fileContent);

        sw.Close();
        sw.Dispose();
    }
    #endregion



    #region 程序压缩的包的命名
    /// <summary>
    ///  压缩包的名字
    /// </summary>
    /// <returns></returns>
    public static string GetCompressedPackageNameByUnity()
    {
        return AppEditor.GetABFolderName();

    }

    public static string GetCompressedPackageNameByJenkins()
    {

        JenkinsBuildSettings settings = GetJenkinsBuildSettings();
        return AppEditor.m_appName + "_PC_" + GetUnityBuildSetings(settings) + string.Format("{0:yyyy_MM_dd_HH_mm}", DateTime.Now);

    }
    #endregion





    #region 获取设置 Jenkins参数
    /// <summary>
    /// 从设置读取一个压缩包的命名前缀
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    static string GetUnityBuildSetings(JenkinsBuildSettings settings)
    {
        string fix = "";
        if (String.IsNullOrEmpty(settings.Version) == false)
        {
            PlayerSettings.bundleVersion = settings.Version;
            fix += settings.Version;
        }
        if (String.IsNullOrEmpty(settings.Build) == false)
        {
            PlayerSettings.macOS.buildNumber = settings.Build;
            fix += "_" + settings.Build;
        }
        if (String.IsNullOrEmpty(settings.Name) == false)
        {
            PlayerSettings.productName = settings.Name;
        }
        if (settings.Debug)
        {
            EditorUserBuildSettings.development = true;
            EditorUserBuildSettings.connectProfiler = true;
            fix += "_Debug";
        }
        else
        {
            EditorUserBuildSettings.development = false;
        }

        return fix;

    }

    /// <summary>
    /// 从cmd读取Jenkins的参数设置
    /// </summary>
    /// <returns></returns>
    static JenkinsBuildSettings GetJenkinsBuildSettings()
    {
        string[] strArr = Environment.GetCommandLineArgs();
        JenkinsBuildSettings setting = new JenkinsBuildSettings();
        foreach (var item in strArr)
        {
            setting.Version = GetVal(item, "Version");
            setting.Build = GetVal(item, "Build");
            setting.Name = GetVal(item, "Name");
            setting.Debug = GetVal(item, "Debug") == "true" ? true : false;
        }

        return setting;
    }

    /// <summary>
    /// Version=0.1  =>  0.1
    /// </summary>
    /// <param name="item"></param>
    /// <param name="property"></param>
    /// <returns></returns>
    static string GetVal(string item, string property)
    {
        if (item.StartsWith(property))
        {
            string[] pairArr = item.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
            if (pairArr.Length == 2)//Version 0.1
            {
                return pairArr[1].Trim();
            }
        }
        return null;
    }
    #endregion


}

/// <summary>
/// Jenkins 自定义部署的参数 (接受Jenkins传来的参数)
/// </summary>
public class JenkinsBuildSettings
{
    public string Version = "";   //版本号
    public string Name = "";   //打包的名字
    public string Build = "";    //build次数
    public bool Debug = true;  //？调试
}