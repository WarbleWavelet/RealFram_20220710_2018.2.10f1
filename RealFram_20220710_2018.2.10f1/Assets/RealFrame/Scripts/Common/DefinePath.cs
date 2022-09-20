using UnityEditor;
using UnityEngine;


public class DefinePath
{
    /// <summary>工程跟目录</summary>
    public static string ProjectPath = Application.dataPath.Replace("Assets", "");
    //public static string ProjectPath = Common.TrimName(Application.dataPath, TrimNameType.SlashAfter) ;
    public static string RealFramePath = Application.dataPath + "/" + DefinePath.RealFrameName + "/";
    public const string RealFrameName = "RealFrame"; //Top文件夹

    //   /"+DefinePath.RealFrame+"

    #region Demo
    public static string Demo01_Bytes_Attack = "Assets/" + DefinePath.RealFrameName + "/StreamingAssets/attack";
    public const string Demo01_Prefab_Attack = "Assets/" + DefinePath.RealFrameName + "/GameData/Prefabs/Attack.prefab";
    public static string Demo02_Xml = RealFramePath + "Scenes/02 Class2Xml/Demo02_test.xml";
    public static string Demo03_WriteBytes = RealFramePath + "Scenes/03 Class2Bin/Demo03_test.bytes";
    public const string Demo03_ReadBytes = "Assets/" + DefinePath.RealFrameName + "/Scenes/03 Class2Bin/Demo03_test.bytes";
    public const string Demo04_Asset = "Assets/" + DefinePath.RealFrameName + "/Scenes/04 ReadAsset/TestAssets.asset";
    public const string Demo04_Prefab_Attack = "Assets/" + DefinePath.RealFrameName + "/GameData/Prefabs/Attack.prefab";
    public const string Demo05_Prefab_Attack = "Assets/" + DefinePath.RealFrameName + "/GameData/Prefabs/Attack.prefab";
    #endregion

    //

    #region 拓展
    public const string MenuItem_AB = "AB/";
    public const string MenuItem_App = "App/";
    public const string MenuItem_Offline = "离线数据/";
    public const string MenuItem_FormatTool = "数据转换/";
    public const string MenuItem_RealFrame = "RealFrame配置/";
    public const string MenuItem_Jenkins = "Jenkins/";
    public const int MenuItem_FormatTool_StartIdx = 0;
    public const string Assets_MyAssets = "My Assets/";
    #endregion



    #region 资源
    public const string Shader_BengHuai = "Custom/benghuai";


    public static string Demo05_Xml_AssetBundleConfig = RealFramePath + "AssetBundleConfig.xml";//XML可视化，随便删
    public static string Demo05_Bin_AssetBundleConfig = "Assets/" + DefinePath.RealFrameName + "/GameData/Data/ABData/AssetBundleConfig.bytes";
    public static string Demo05_AB_assetbundleconfig = "Assets/" + DefinePath.RealFrameName + "/StreamingAssets/assetbundleconfig";
    public const string Demo07_MP3_SenLin = "Assets/" + DefinePath.RealFrameName + "/GameData/Sounds/senlin.mp3";
    public const string Demo08_MP3_SenLin = "Assets/" + DefinePath.RealFrameName + "/GameData/Sounds/senlin.mp3";
    public const string Demo09_MP3_SenLin = "Assets/" + DefinePath.RealFrameName + "/GameData/Sounds/senlin.mp3";
    public const string Demo10_Prefab_Attack = "Assets/" + DefinePath.RealFrameName + "/GameData/Prefabs/Attack.prefab";
    public const string Demo11_Prefab_Attack = "Assets/" + DefinePath.RealFrameName + "/GameData/Prefabs/Attack.prefab";
    public const string Demo14_MP3_SenLin = "Assets/" + DefinePath.RealFrameName + "/GameData/Sounds/senlin.mp3";
    public const string Demo14_Images_PathPreFixed = "Assets/" + DefinePath.RealFrameName + "/GameData/Images/";
    #endregion  




    #region ScriptObject
    public const string ABCfgSOPath = "Assets/" + DefinePath.RealFrameName + "/Config/ABCfg.asset";
    public const string RealFrameCfgSOPath = "Assets/" + DefinePath.RealFrameName + "/Config/RealFrameCfg.asset";
    #endregion

    #region 打包相关



    //public static string OutputABOutterPath =  Application.dataPath + "/../AssetBundle/";//放到外面，不生成meta   ,失败

    public static string OutputXml = RealFramePath + "AssetBundleConfig.xml";//XML可视化，随便删
    public static string OutputBytes = RealFramePath + "AssetBundleConfig.bytes";//bytes，与下面简单的路径不同
    public static string OutputAB = RealFramePath + "StreamingAssets/assetbundleconfig"; //bytes路径的AB包路径

    public static string abCfg_Path = "Assets/" + DefinePath.RealFrameName + "/GameData/Data/ABData/"; //bytes
    public const string abCfg_Name = "assetbundleconfig"; //bytes
    public static string abCfg_Bytes = "Assets/" + DefinePath.RealFrameName + "/GameData/Data/ABData/AssetBundleConfig.bytes"; //bytes


    public static string AppBuildPath = Application.dataPath + "/../BuildTarget/";
    // public static string AppBuildPath_Andriod = Application.dataPath + "/../BuildTarget/Android/"; 
    public static string AppBuildPath_Andriod = DefinePath.ProjectPath + "BuildTarget/Android/";
    public static string AppBuildPath_IOS = Application.dataPath + "/../BuildTarget/IOS/";
    public static string AppBuildPath_Windows = Application.dataPath + "/../BuildTarget/Windows/";

    public static string ABBuildPath_Andriod = Application.dataPath + "/../AssetBundle/Android/";
    public static string ABBuildPath_IOS = Application.dataPath + "/../AssetBundle/IOS/";
    public static string ABBuildPath_Windows = Application.dataPath + "/../AssetBundle/Windows/";



    public static string OutputAB_InnerPath = RealFramePath + "StreamingAssets/";//打包时总包名是平台名
    public static string OutputAB_OutterPath = ProjectPath + "AssetBundle/";//Assets的上一级
    public static string ABMD5_InnerPath = RealFramePath + "Resources/";
    public static string ABMD5_OutterPath = ProjectPath + "Version/";//Assets的上一级
    #endregion

    //ab.LoadAssets                     assetbundleconfig                
    //AssetBundle.LoadFromFile          Application.streamingAssetsPath + "/assetbundleconfig


    #region 配置相关
    //public static string Cfg_MonsterData =Application.dataPath + "/GameData/Data/Bin/MonsterData.bytes";
    //public static string Cfg_BuffData = Application.dataPath + "/GameData/Data/Bin/BuffData.bytes";      
    public const string Cfg_MonsterData_Inner = "Assets/" + DefinePath.RealFrameName + "/GameData/Data/Bin/MonsterData.bytes";
    public const string Cfg_BuffData = "Assets/" + DefinePath.RealFrameName + "/GameData/Data/Bin/BuffData.bytes";
    public const string Cfg_BuffData2 = "Assets/" + DefinePath.RealFrameName + "/GameData/Data/Bin/BuffData2.bytes";
    public const string Cfg_UIPrefabPath = "Assets/" + DefinePath.RealFrameName + "/GameData/Prefabs/";
    #endregion



    #region 热更
    public static string Hot_OutterPath = Application.dataPath + "/../Hot/" ;
    #endregion
}


#region Demo
public class DefinePath_Demo13
{
    private const string PanelPath = "Assets/" + DefinePath.RealFrameName + "/GameData/Prefabs/UGUI/Panel/Demo13/";
    public const string MenuPanel = PanelPath + "Demo13MenuPanel.prefab";
}

public class DefinePath_Demo14
{
    public const string Scene_Menu = "Menu14";
    public const string Scene_Start = "Start14";
    public const string Scene_Empty = "Empty14";
    //
    private const string PanelPath = "Assets/" + DefinePath.RealFrameName + "/GameData/Prefabs/UGUI/Panel/Demo14/";
    private const string PanelPath1 = "Assets/" + DefinePath.RealFrameName + "/GameData/UGUI/";
    public const string Prefab_LoadPanel = PanelPath + "Demo14LoadPanel.prefab";
    public const string Prefab_MenuPanel = PanelPath + "Demo14MenuPanel.prefab";
    public const string Prefab_Attack = "Assets/" + DefinePath.RealFrameName + "/GameData/Prefabs/Attack.prefab";
    public const string MP3_SenLin = "Assets/" + DefinePath.RealFrameName + "/GameData/Sounds/senlin.mp3";

}

public class DefinePath_Demo15
{
    //界面名称


    //场景名称
    public const string Scene_Menu = "Menu15";
    public const string Scene_Start = "Start15";
    public const string Scene_Empty = "Empty15";

   
    public const string ATTACK = "Assets/GameData/Prefabs/Attack.prefab";   //临时prefab路径

 
    public const string MENUSOUND = "Assets/GameData/Sounds/menusound.mp3";    //临时音乐资源
    public const string Prefab_Attack = "Assets/" + DefinePath.RealFrameName + "/GameData/Prefabs/Attack.prefab";
    private const string PanelPath = "Assets/" + DefinePath.RealFrameName + "/GameData/Prefabs/UGUI/Panel/Demo15/";



    public const string Prefab_CommonConfirm = PanelPath + "CommonConfirm.prefab";
    public const string CommonConfirm =  "CommonConfirm";
    public const string Prefab_HotfixPanel = PanelPath + "HotfixPanel.prefab";      
    public const string Hotfix = "HotfixPanel.prefab";
}
#endregion




#region OfflineData
public class DefinePath_OfflineData
{

    public const string m_Type = "t:prefab";
    public const string m_Path = "Assets/" + DefinePath.RealFrameName + "/GameData/OfflineData/UIOfflineData";


}
public class DefinePath_UIOfflineData
{

    public const string m_Type = "t:prefab";
    public const string m_Path = "Assets/" + DefinePath.RealFrameName + "/GameData/Prefabs/UGUI/Panel";
}


public class DefinePath_ParticleOfflineData
{

    public const string m_Type = "t:prefab";
    public const string m_Path = "Assets/" + DefinePath.RealFrameName + "/GameData/OfflineData/ParticleOfflineData";
}
#endregion
