/****************************************************
    文件：JenkinsEditor.cs
	作者：lenovo
    邮箱: 
    日期：2022/8/19 9:3:29
	功能：Jenkins交互(注意都是用Jenkins调用的，不能直接在Unity中调用)
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class JenkinsEditor : Editor
{
    public const string m_appName = DefinePath.RealFrameName;

    #region MenuItem


    [MenuItem(DefinePath.MenuItem_Jenkins + "写入txt当前日期", false, 100)]
    static void MenuItem_WriteTxt()
    {
        Common.Text_Write(DefinePath.RealFramePath + "输出可删除/Test_Jenkins_写入txt当前日期.txt", System.DateTime.Now.ToString());
    }




    #region Jenkins


    [MenuItem(DefinePath.MenuItem_Jenkins + "打Jenkins的PC包", false, 100)]
    static void MenuItem_Jenkins_BuildApp_PC()
    {
        //AssetBundleEditor.BuildAB_RootOutter(); //AB包

        PlayerSettings.Android.keyaliasName = Constants.Android_keyaliasName;//密钥
        PlayerSettings.Android.keystoreName = Constants.Android_keystoreName;
        PlayerSettings.Android.keyaliasPass = Constants.Android_keyaliasPass;
        PlayerSettings.Android.keystorePass = Constants.Android_keystorePass;

        string completedName = GetName_Completed(PackType.Jenkins_PC); //程序包
        string programPath = AppEditor.GetName_App(completedName);
        Common.Folder_Clear_Recursive(DefinePath.ABBuildPath_Windows);
        AppEditor.BuildApp(programPath);
        Common.Text_Write(DefinePath.AppBuildPath + "buildname.txt", completedName);
    }


    [MenuItem(DefinePath.MenuItem_Jenkins + "打Jenkins的安卓包", false, 100)]
    static void MenuItem_Jenkins_BuildApp_Android()
    {
        //AssetBundleEditor.BuildAB_RootOutter(); //AB包
        Common.Folder_Clear_Recursive(DefinePath.AppBuildPath_Andriod);

        PlayerSettings.Android.keyaliasName = Constants.Android_keyaliasName;//密钥
        PlayerSettings.Android.keystoreName = Constants.Android_keystoreName;
        PlayerSettings.Android.keyaliasPass = Constants.Android_keyaliasPass;
        PlayerSettings.Android.keystorePass = Constants.Android_keystorePass;

        string completedName = GetName_Completed(PackType.Jenkins_Android); //程序包
        string appPath = AppEditor.GetName_App(completedName);
        AppEditor.BuildApp(appPath);
        Common.Text_Write(DefinePath.AppBuildPath + "buildname.txt", completedName);
    }


    [MenuItem(DefinePath.MenuItem_Jenkins + "打Jenkins的IOS包（未完成）", false, 100)]
    static void MenuItem_Jenkins_BuildApp_IOS()
    {
        Common.Folder_Clear_Recursive(DefinePath.AppBuildPath_Andriod);


        //AssetBundleEditor.BuildAB_RootOutter(); //AB包
        string completedName = GetName_Completed(PackType.Jenkins_IOS); //用Jenkins的参数
        string appPath = AppEditor.GetName_App();
        AppEditor.BuildApp(appPath);
        Common.Text_Write(DefinePath.AppBuildPath + "buildname.txt", completedName);
    }

    #endregion


    #endregion






    #region 打包的名字
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

            case PackType.Jenkins_PC:
                {
                    BuildSettings_Jenkins_PC settings = GetBuildSettings_Jenkins_PC();
                    typeStr = "PC_" + SetBuildSetings_Jenkins_PC(settings);
                }
                break;
            case PackType.Jenkins_Android:
                {
                    BuildSettings_Jenkins_Android settings = GetBuildSettings_Jenkins_Android();
                    typeStr = "Android_" + SetBuildSetings_Jenkins_Android(settings);
                }
                break;
            case PackType.Jenkins_IOS:
                {
                    //BuildSettings_Jenkins_IOS settings = GetBuildSettings_Jenkins_IOS();
                    // typeStr = "IOS_" + SetBuildSetings_Jenkins_IOS(settings);
                    typeStr = "IOS";
                }
                break;
            default: break;
        }


        string str = string.Format("{0}_{1}_{2:yyyy_MM_dd_HH_mm}", m_appName, typeStr, DateTime.Now);
        return str;

    }

    #endregion










    #region 获取设置 Jenkins参数





    #region SetBuildSetings



    /// <summary>
    /// 从设置读取一个压缩包的命名前缀
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    static string SetBuildSetings_Jenkins_PC(BuildSettings_Jenkins_PC settings)
    {
        string fix = "";
        if (String.IsNullOrEmpty(settings.Version) == false)
        {
            PlayerSettings.bundleVersion = settings.Version;
            fix += settings.Version + "_";
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
    /// 从设置读取一个压缩包的命名前缀
    /// </summary>
    /// <param name="setting"></param>
    /// <returns></returns>
    static string SetBuildSetings_Jenkins_Android(BuildSettings_Jenkins_Android setting)
    {
        string fix = "";

        if (setting.Canal != Canal.None)//渠道
        {
            string symbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, symbol + ";" + setting.Canal.ToString());
            fix += setting.Canal.ToString();
        }



        if (String.IsNullOrEmpty(setting.Version) == false)
        {
            PlayerSettings.bundleVersion = setting.Version;
            fix += "_" + setting.Version;
        }
        if (String.IsNullOrEmpty(setting.Build) == false)
        {
            PlayerSettings.macOS.buildNumber = setting.Build;
            fix += "_" + setting.Build;
        }
        if (String.IsNullOrEmpty(setting.Name) == false)
        {
            PlayerSettings.productName = setting.Name;
            // PlayerSettings.applicationIdentifier =Constants.Android_applicationIdentifierFix + setting.Name; //com.xxx.xxx
        }
        PlayerSettings.MTRendering = setting.MultithreadRendering; //多渠道渲染
        if (setting.MultithreadRendering == true)
        {
            fix += "_MTR";
        }

        if (setting.IL2CPP == true) //IL2CPP
        {
            fix += "_IL2CPP";
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        }
        else
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
        }
        if (setting.Debug == true)
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
    /// 从设置读取一个压缩包的命名前缀
    /// </summary>
    /// <param name="setting"></param>
    /// <returns></returns>
    static string SetBuildSetings_Jenkins_IOS(BuildSettings_Jenkins_IOS setting)
    {
        string fix = "";

        if (setting.Canal != Canal.None)//渠道
        {
            string symbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, symbol + ";" + setting.Canal.ToString());
            fix += setting.Canal.ToString();
        }



        if (String.IsNullOrEmpty(setting.Version) == false)
        {
            PlayerSettings.bundleVersion = setting.Version;
            fix += "_" + setting.Version;
        }
        if (String.IsNullOrEmpty(setting.Build) == false)
        {
            PlayerSettings.iOS.buildNumber = setting.Build;
            fix += "_" + setting.Build;
        }
        if (String.IsNullOrEmpty(setting.Name) == false)
        {
            PlayerSettings.productName = setting.Name;
            // PlayerSettings.applicationIdentifier =Constants.Android_applicationIdentifierFix + setting.Name; //com.xxx.xxx
        }
        PlayerSettings.MTRendering = setting.MultithreadRendering; //多渠道渲染
        if (setting.MultithreadRendering == true)
        {
            fix += "_MTR";
        }

        if (setting.IL2CPP == true) //IL2CPP
        {
            fix += "_IL2CPP";
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        }
        else
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
        }
        if (setting.Debug == true)
        {
            EditorUserBuildSettings.development = true;
            EditorUserBuildSettings.connectProfiler = true;
            fix += "_Debug";
        }
        else
        {
            EditorUserBuildSettings.development = false;
        }
        if (setting.DynamicBatching == true)
        {
            fix += "_DynamicBatching";
        }
        else
        {

        }

        return fix;

    }
    #endregion



    #region Get  BuildSettings


    /// <summary>
    /// 从cmd读取Jenkins的参数设置
    /// </summary>
    /// <returns></returns>
    static BuildSettings_Jenkins_PC GetBuildSettings_Jenkins_PC()
    {
        string[] strArr = Environment.GetCommandLineArgs();
        BuildSettings_Jenkins_PC setting = new BuildSettings_Jenkins_PC();
        foreach (var item in strArr)
        {
            setting.Version = GetVal(item, "Version");
            setting.Build = GetVal(item, "Build");
            setting.Name = GetVal(item, "Name");
            setting.Debug = Common.String2Bool(GetVal(item, "Debug"));

        }

        return setting;
    }



    /// <summary>
    /// 从cmd读取Jenkins的参数设置
    /// </summary>
    /// <returns></returns>
    static BuildSettings_Jenkins_Android GetBuildSettings_Jenkins_Android()
    {
        string[] paraArr = Environment.GetCommandLineArgs();
        BuildSettings_Jenkins_Android setting = new BuildSettings_Jenkins_Android();
        foreach (string item in paraArr)
        {


            if (Common.String2Enum<Canal>(GetVal(item, "Canal")) != null)
            {
                setting.Canal = (Canal)Common.String2Enum<Canal>(GetVal(item, "Canal"));
            }

            setting.Version = GetVal(item, "Version");
            setting.Build = GetVal(item, "Build");
            setting.Name = GetVal(item, "Name");
            setting.Debug = Common.Try_String2Bool(GetVal(item, "Debug"));
            setting.MultithreadRendering = Common.Try_String2Bool(GetVal(item, "MultithreadRendering"));
            setting.IL2CPP = Common.Try_String2Bool(GetVal(item, "IL2CPP"));
        }
        return setting;
    }



    /// <summary>
    /// 从cmd读取Jenkins的参数设置
    /// </summary>
    /// <returns></returns>
    static BuildSettings_Jenkins_IOS GetBuildSettings_Jenkins_IOS()
    {
        string[] paraArr = Environment.GetCommandLineArgs();
        BuildSettings_Jenkins_IOS setting = new BuildSettings_Jenkins_IOS();
        foreach (string item in paraArr)
        {


            if (Common.String2Enum<Canal>(GetVal(item, "Canal")) != null)
            {
                setting.Canal = (Canal)Common.String2Enum<Canal>(GetVal(item, "Canal"));
            }

            setting.Version = GetVal(item, "Version");
            setting.Build = GetVal(item, "Build");
            setting.Name = GetVal(item, "Name");
            setting.Debug = Common.Try_String2Bool(GetVal(item, "Debug"));
            setting.MultithreadRendering = Common.Try_String2Bool(GetVal(item, "MultithreadRendering"));
            setting.IL2CPP = Common.Try_String2Bool(GetVal(item, "IL2CPP"));
            setting.DynamicBatching = Common.Try_String2Bool(GetVal(item, "DynamicBatching"));
        }
        return setting;
    }

    static string GetVal(string pair, string property)
    {
        if (pair.StartsWith(property))
        {
            string[] pairArr = pair.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
            if (pairArr.Length == 2)//Version 0.1
            {
                return pairArr[1].Trim();
            }
        }
        return null;
    }
    #endregion




    /// <summary>
    /// Version=0.1  =>  0.1
    /// </summary>
    /// <param name="item"></param>
    /// <param name="property"></param>
    /// <returns></returns>


    //if (str.StartsWith("Place"))
    //{
    //    var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
    //    if (tempParam.Length == 2)
    //    {
    //        buildSetting.Place = (Place)Enum.Parse(typeof(Place), tempParam[1], true);
    //    }
    //}
    #endregion


}



#region BuildSettings

/// <summary>
/// Jenkins 自定义部署的参数 (接受Jenkins传来的参数)
/// </summary>
public class BuildSettings_Jenkins_PC
{
    public string Version = "";   //版本号
    public string Name = "";   //打包的名字
    public string Build = "";    //build次数
    public bool Debug = true;  //？调试
}


/// <summary>
/// Jenkins 自定义部署的参数 (接受Jenkins传来的参数)
/// </summary>
public class BuildSettings_Jenkins_Android
{
    public string Version = "";   //版本号
    public string Name = "";   //打包的名字
    public string Build = "";    //build次数
    public bool Debug = true;  //？调试
    public bool MultithreadRendering = true;  //？多线程渲染
    public bool IL2CPP = true;  //？Scripting Backed是否IL2CPP(需要NDK)
    public Canal Canal = Canal.None;
}


/// <summary>
/// Jenkins 自定义部署的参数 (接受Jenkins传来的参数)
/// </summary>
public class BuildSettings_Jenkins_IOS
{
    public string Version = "";   //版本号
    public string Name = "";   //打包的名字
    public string Build = "";    //build次数
    public bool Debug = true;  //？调试
    public bool MultithreadRendering = true;  //？多线程渲染
    public bool IL2CPP = true;  //？Scripting Backed是否IL2CPP(需要NDK)
    public Canal Canal = Canal.None;
    public bool DynamicBatching = true;//动态合批
}

#endregion



/// <summary>
/// 打包方式
/// </summary>
public enum PackType
{
    None,
    Unity_PC,
    Unity_Android,
    Unity_IOS,
    Jenkins_PC,
    Jenkins_IOS,
    Jenkins_Android
}


/// <summary>
/// 发布渠道
/// </summary>
public enum Canal
{
    None = 0,
    Xiaomi,
    Meizu,
    Huawei,
    Weixin,
    Bilibili
}