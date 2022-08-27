/****************************************************
    文件：AppEditor.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/27 17:51:18
	功能：处理Untiy内部打包（不涉及Jenkins）
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


    public static string m_appBuildPath_Android = DefinePath.AppBuildPath_Andriod;
    public static string m_appBuildPath_IOS = DefinePath.AppBuildPath_IOS;
    public static string m_appBuildPath_Windows = DefinePath.AppBuildPath_Windows;
    //
    static string m_abBuildPath_Android = DefinePath.ABBuildPath_Andriod + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
    static string m_abBuildPath_IOS = DefinePath.ABBuildPath_IOS + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
    static string m_abBuildPath_Windows = DefinePath.ABBuildPath_Windows + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
    //
    static string m_OutputABInnerPath = DefinePath.OutputAB_InnerPath;
    static string m_OutputABOutterPath = DefinePath.OutputAB_OutterPath + "Windows/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";

    public static string m_appName = PlayerSettings.productName;//注意设为RealFrame
    #endregion


    #region MenuItem



    [MenuItem(DefinePath.MenuItem_App + "打Unity的PC包", false, 100)]
    static void MenuItem_App_BuildApp_PC()
    {
        // AssetBundleEditor.BuildAB_RootOutter(); //AB包

        string completedName = GetName_Completed(PackType.Unity_PC);//用unity的参数
        string programPath = GetName_App(completedName);
        BuildApp(programPath);
        Common.Text_Write(DefinePath.AppBuildPath + "buildname.txt", completedName);
    }


    [MenuItem(DefinePath.MenuItem_App + "打Unity的安卓包", false, 100)]
    static void MenuItem_App_BuildApp_Android()
    {

        // AssetBundleEditor.BuildAB_RootOutter(); //AB包

        PlayerSettings.Android.keyaliasName = Constants.Android_keyaliasName;//密钥
        PlayerSettings.Android.keystoreName = Constants.Android_keystoreName;
        PlayerSettings.Android.keyaliasPass = Constants.Android_keyaliasPass;
        PlayerSettings.Android.keystorePass = Constants.Android_keystorePass;


        string compressedPackageName = GetName_Completed(PackType.Unity_Android);//用unity的参数
        string programPath = GetName_App(compressedPackageName);
        BuildApp(programPath);
        Common.Text_Write(DefinePath.AppBuildPath + "buildname.txt", compressedPackageName);
    }


    [MenuItem(DefinePath.MenuItem_App + "打Unity的IOS包", false, 100)]
    static void MenuItem_App_BuildApp_IOS()
    {

        // AssetBundleEditor.BuildAB_RootOutter(); //AB包

        Common.File_Clear(m_appBuildPath_IOS);
        string completedName = GetName_Completed(PackType.Unity_IOS);//用unity的参数
        string programPath = GetName_App(completedName);

        BuildApp(programPath);
        Common.Text_Write(DefinePath.AppBuildPath + "buildname.txt", completedName);
    }
    #endregion






    #region 看使用情景来用

    //[MenuItem(Constants.MenuItem + "/一键打包到外部", false, 82)]//因为脚本不能加AB的错误，不要一键了
    [MenuItem(DefinePath.MenuItem_App + "5 外导包 生成执行程序", false, 82)]//按钮在菜单栏的位置
    public static void Output()
    {

        //AssetBundleEditor.Build();  //内部AB包

        Common.File_Copy(m_OutputABInnerPath, m_OutputABOutterPath); //外部AB包


        BuildPipeline.BuildPlayer(
            GetScenes_Enabled(), //打工程包
            GetName_App(),
            EditorUserBuildSettings.activeBuildTarget,
            BuildOptions.None
        );

        Common.File_Clear(m_OutputABInnerPath);//删除内部的包

        Debug.LogFormat("导出到外部成功：{0}", m_OutputABOutterPath);
    }

    [MenuItem(DefinePath.MenuItem_App + "12345一键生成、外导包和生成执行程序", false, 82)]//按钮在菜单栏的位置
    public static void BuildAndOutput()
    {

        AssetBundleEditor.BuildAB_RootInner();  //内部AB包

        Common.File_Copy(m_OutputABInnerPath, m_OutputABOutterPath); //外部AB包


        BuildPipeline.BuildPlayer(
            GetScenes_Enabled(), //打工程包
            GetName_App(),
            EditorUserBuildSettings.activeBuildTarget,
            BuildOptions.None
        );

        Common.File_Clear(m_OutputABInnerPath);//删除内部的包
        Debug.LogFormat("一键生成并且导出到外部成功：{0}", m_OutputABOutterPath);
    }


    #endregion

    #region 辅助


    public static void BuildApp(string path)
    {
        //打工程包
        BuildPipeline.BuildPlayer(
            GetScenes_Enabled(),
            path,
            EditorUserBuildSettings.activeBuildTarget,
            BuildOptions.None
        );

        Debug.LogFormat("导出到外部成功：{0}", m_OutputABOutterPath);


    }



    /// <summary>
    /// exe apk ipa的完整路径名
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetName_App(string path = "")
    {
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:
                {
                    return string.Format("{0}{1}{2}", m_appBuildPath_Android, path, ".apk");
                }
            case BuildTarget.iOS:
                {
                    return string.Format("{0}{1}", m_appBuildPath_IOS, path); //IOS系统软件的后缀名是IPA
                }
            case BuildTarget.StandaloneWindows64:
                {
                    return string.Format("{0}{1}/{2}{3}", m_appBuildPath_Windows, path, m_appName, ".exe");
                }
            case BuildTarget.StandaloneWindows:
                {
                    return string.Format("{0}{1}/{2}{3}", m_appBuildPath_Windows, path, m_appName, ".exe");
                }
            default: break;
        }
        return null;
    }


    /// <summary>
    /// 压缩包的名字 || 文件夹的名字 || apk ipa的名字， 看怎么用
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetName_Completed(PackType type)
    {

        string typeStr = "";
        switch (type)
        {
            case PackType.Unity_PC:
                {
                    typeStr = "PC";
                }
                break;
            case PackType.Unity_Android:
                {
                    typeStr = "Android";
                }
                break;
            case PackType.Unity_IOS:
                {
                    typeStr = "IOS";
                }
                break;
            default: break;
        }

        return string.Format("{0}_{1}_{2:yyyy_MM_dd_HH_mm}", m_appName, typeStr, DateTime.Now);

    }




    /// <summary>
    /// setting中添加剂或的场景
    /// </summary>
    /// <returns></returns>
    static string[] GetScenes_Enabled()
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



    #endregion


}
