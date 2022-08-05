using UnityEngine;
using UnityEditor;

public class DefinePath
{
    /// <summary>工程跟目录</summary>
    public static string ProjectRoot = Application.dataPath.Replace("Assets", "");

    public static string Demo01_Bytes_Attack = Application.streamingAssetsPath + "/attack";
    public const string Demo01_Prefab_Attack = "Assets/GameData/Prefabs/Attack.prefab";
    public static string Demo02_Xml = Application.dataPath + "/Scenes/01-04/02 Class2Xml/Demo02_test.xml";
    public static string Demo03_WriteBytes = Application.dataPath + "/Scenes/01-04/03 Class2Bin/Demo03_test.bytes";
    public const string Demo03_ReadBytes = "Assets/Scenes/01-04/03 Class2Bin/Demo03_test.bytes";
    public const string Demo04_Asset = "Assets/Scenes/01-04/04 ReadAsset/TestAssets.asset";
    public const string Demo04_Prefab_Attack = "Assets/GameData/Prefabs/Attack.prefab";
    //


    public static string Demo05_AB_Xml = Application.dataPath  + "/AssetBundleConfig.xml";//XML可视化，随便删
    public static string Demo05_Bytes_Cfg = Application.streamingAssetsPath + "/assetbundleconfig";
    public const string Demo09_MP3_SenLin = "Assets/GameData/Sounds/senlin.mp3";
    public const string Demo14_MP3_SenLin = "Assets/GameData/Sounds/senlin.mp3";


    public const string ABCfgSOPath = "Assets/Editor/Resource/ABConfig.asset";
    public static string OutputABInnerPath = Application.streamingAssetsPath;
    //public static string OutputABOutterPath =  Application.dataPath + "/../AssetBundle/";//放到外面，不生成meta   ,失败

    public static string OutputXml = Application.dataPath  + "/AssetBundleConfig.xml";//XML可视化，随便删
    public static string OutputBytes = Application.dataPath  + "/AssetBundleConfig.bytes";//bytes，与下面简单的路径不同
    public static string OutputAB = Application.streamingAssetsPath + "/assetbundleconfig"; //bytes路径的AB包路径
    public static string OutputABName = "assetbundleconfig";//bytes路径的AB包名
    public static string InputBytes = "Assets/GameData/Data/ABData/AssetBundleConfig.bytes"; //bytes



    #region 打包相关
    public static string AppBuildPath_Andriod = Application.dataPath + "/../BuildTarget/Android/"; 
    public static string AppBuildPath_IOS = Application.dataPath + "/../BuildTarget/IOS/";
    public static string AppBuildPath_Windows = Application.dataPath + "/../BuildTarget/Windows/";

    public static string ABBuildPath_Andriod = Application.dataPath + "/../AssetBundle/Android/";
    public static string ABBuildPath_IOS = Application.dataPath + "/../AssetBundle/IOS/";
    public static string ABBuildPath_Windows = Application.dataPath + "/../AssetBundle/Windows/";

    public static string OutputABOutterPath = Common.TrimName(Application.dataPath, TrimNameType.SlashPre) + "/AssetBundle/";//Assets的上一级
    #endregion

    //ab.LoadAssets                     assetbundleconfig                
    //AssetBundle.LoadFromFile          Application.streamingAssetsPath + "/assetbundleconfig


    #region 配置相关
    //public static string Cfg_MonsterData =Application.dataPath + "/GameData/Data/Bin/MonsterData.bytes";
    //public static string Cfg_BuffData = Application.dataPath + "/GameData/Data/Bin/BuffData.bytes";      
    public const string Cfg_MonsterData = "Assets/GameData/Data/Bin/MonsterData.bytes";
    public const string Cfg_BuffData = "Assets/GameData/Data/Bin/BuffData.bytes";
    public const string Cfg_BuffData2 = "Assets/GameData/Data/Bin/BuffData2.bytes";
    #endregion
}

