/****************************************************
    文件：AppEditor.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/27 17:51:18
	功能：
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;



public class AppEditor
{

    #region 字属
    static string m_appBuildPath_Android = DefinePath.AppBuildPath_Andriod;
    static string m_appBuildPath_IOS = DefinePath.AppBuildPath_IOS;
    static string m_appBuildPath_Windows = DefinePath.AppBuildPath_Windows;
    //
    static string m_abBuildPath_Android = DefinePath.ABBuildPath_Andriod + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
    static string m_abBuildPath_IOS = DefinePath.ABBuildPath_IOS + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
    static string m_abBuildPath_Windows = DefinePath.ABBuildPath_Windows + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
    //
    static string m_OutputABInnerPath = DefinePath.OutputABInnerPath;
    static string m_OutputABOutterPath = DefinePath.OutputABOutterPath + "Windows/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";

    public static string m_appName = PlayerSettings.productName;//注意设为RealFrame
    #endregion
    [MenuItem(DefinePath.MenuItem_AB + "/5 外部生成执行程序", false, 82)]//按钮在菜单栏的位置
    public static void BuildApp()
    {

        BuildPipeline.BuildPlayer(
            GetAllEnabledScenes(), //打工程包
            GetExecutableProgramPath(),
            EditorUserBuildSettings.activeBuildTarget,
            BuildOptions.None
        );

        Debug.LogFormat("导出到外部成功：{0}", m_OutputABOutterPath);
    }

    //[MenuItem(Constants.MenuItem + "/一键打包到外部", false, 82)]//因为脚本不能加AB的错误，不要一键了
    [MenuItem(DefinePath.MenuItem_AB + "/5 外导包 生成执行程序", false, 82)]//按钮在菜单栏的位置
    public static void Output()
    {

        //AssetBundleEditor.Build();  //内部AB包

        Copy(m_OutputABInnerPath, m_OutputABOutterPath); //外部AB包


        BuildPipeline.BuildPlayer(
            GetAllEnabledScenes(), //打工程包
            GetExecutableProgramPath(),
            EditorUserBuildSettings.activeBuildTarget,
            BuildOptions.None
        );

        DeleteAllPackInPath(m_OutputABInnerPath);//删除内部的包

        Debug.LogFormat("导出到外部成功：{0}", m_OutputABOutterPath);
    }

    [MenuItem(DefinePath.MenuItem_AB + "/12345一键生成、外导包和生成执行程序", false, 82)]//按钮在菜单栏的位置
    public static void BuildAndOutput()
    {

        AssetBundleEditor.Build();  //内部AB包

        Copy(m_OutputABInnerPath, m_OutputABOutterPath); //外部AB包


        BuildPipeline.BuildPlayer(
            GetAllEnabledScenes(), //打工程包
            GetExecutableProgramPath(),
            EditorUserBuildSettings.activeBuildTarget,
            BuildOptions.None
        );

        DeleteAllPackInPath(m_OutputABInnerPath);//删除内部的包
        Debug.LogFormat("一键生成并且导出到外部成功：{0}", m_OutputABOutterPath);
    }

    #region 辅助


   public static void BuildApp(string path)
    {

        BuildPipeline.BuildPlayer(
            GetAllEnabledScenes(), //打工程包
            path,
            EditorUserBuildSettings.activeBuildTarget,
            BuildOptions.None
        );

        Debug.LogFormat("导出到外部成功：{0}", m_OutputABOutterPath);


    }
    /// <summary>
    /// 根据平台得到工程输出路径 (全写，包括后缀名)
    /// </summary>
    /// <returns></returns>
    static string GetExecutableProgramPath()
    {
        string savePath = "";
        string last = "";

        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:
                {
                    savePath += m_appBuildPath_Android;
                    last = ".apk";
                }
                break;
            case BuildTarget.iOS:
                {
                    savePath += m_appBuildPath_IOS;
                    last = ".ipa"; //IOS系统软件的后缀名是IPA
                }
                break;
            case BuildTarget.StandaloneWindows64:
                {
                    savePath += m_appBuildPath_Windows;
                    last = ".exe";
                }
                break;
            case BuildTarget.StandaloneWindows:
                {
                    savePath += m_appBuildPath_Windows;
                    last = ".exe";
                }
                break;
            default: { } break;
        }
        savePath += m_appName;
        savePath += "_" + EditorUserBuildSettings.activeBuildTarget;
        savePath += "_" + string.Format("{0:yyyy_MM_dd_HH_mm}", System.DateTime.Now);
        savePath += "/" + m_appName;
        savePath += last;


        return savePath;
    }



    public static string GetExecutableProgramPath(string path)
    {
        string savePath = "";
        string last = "";

        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:
                {
                    savePath += m_appBuildPath_Android;
                    last = ".apk";
                }
                break;
            case BuildTarget.iOS:
                {
                    savePath += m_appBuildPath_IOS;
                    last = ".ipa"; //IOS系统软件的后缀名是IPA
                }
                break;
            case BuildTarget.StandaloneWindows64:
                {
                    savePath += m_appBuildPath_Windows;
                    last = ".exe";
                }
                break;
            case BuildTarget.StandaloneWindows:
                {
                    savePath += m_appBuildPath_Windows;
                    last = ".exe";
                }
                break;
            default: { } break;
        }
        savePath += path;
        savePath += "/" + m_appName;
        savePath += last;


        return savePath;
    }


    /// <summary>
    /// 输出AB文件夹的名字
    /// </summary>
    /// <returns></returns>
    public static string GetABFolderName()
    {
        return string.Format("{0}_{1}_{2:yyyy_MM_dd_HH_mm}", m_appName, EditorUserBuildSettings.activeBuildTarget, DateTime.Now);

    }




    /// <summary>
    /// setting中添加剂或的场景
    /// </summary>
    /// <returns></returns>
    static string[] GetAllEnabledScenes()
    {
        List<string> sceneLst = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled == false)
            {
                continue;
            }
            sceneLst.Add(scene.path);
        }

        return sceneLst.ToArray();
    }


    /// <summary>
    /// 文件夹拷贝
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    static void Copy(string from, string to)
    {


        try //递归拷贝
        {
            //取路径
            Common.TickPath(to);
            string toPath = Path.Combine(to, Path.GetFileName(from));// A/   B/b  =>  A/b
            toPath = Common.TrimName(toPath, TrimNameType.SlashPre);//去掉StreamingAssets
            if (File.Exists(from) == true)
            {
                toPath += Path.DirectorySeparatorChar;// Path.DirectorySeparatorChar: '\'
            }

            //取文件
            Common.TickPath(toPath);

            string[] fileArr = Directory.GetFileSystemEntries(from);
            //赋值
            foreach (string file in fileArr)
            {
                if (file.EndsWith(".meta") == true)
                {
                    continue;
                }
                if (Directory.Exists(file) == true)
                {
                    Copy(file, toPath);  //文件夹拷贝
                }
                else
                {

                    File.Copy(file, toPath + "/" + Path.GetFileName(file), true);//文件拷贝  ,文件夹和文件名
                }
            }

        }
        catch (Exception)
        {

            Debug.LogErrorFormat("无法复制：{0} => {1}", from, to);
        }
    }


    /// <summary>
    /// 删除文件夹下的所有文件
    /// </summary>
    static void DeleteAllPackInPath(string path)
    {
        try
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileSystemInfo[] fiArr = di.GetFileSystemInfos();

            foreach (var fsi in fiArr)
            {
                if (fsi is DirectoryInfo)
                {
                    DirectoryInfo _di = new DirectoryInfo(fsi.FullName);
                    _di.Delete(true);
                }
                else
                {
                    File.Delete(fsi.FullName);
                }
            }

        }
        catch (Exception)
        {

            throw;
        }
    }
    #endregion


}
