using UnityEngine;

public class DefinePath
{
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
    public static string OutputABPath = Application.streamingAssetsPath;
    public static string OutputXml = Application.dataPath  + "/AssetBundleConfig.xml";//XML可视化，随便删
    public static string OutputBytes = Application.dataPath  + "/AssetBundleConfig.bytes";
    public static string OutputAB = Application.streamingAssetsPath + "/assetbundleconfig";
    public static string OutputABName = "assetbundleconfig";
    public static string InputBytes = "Assets/GameData/Data/ABData/AssetBundleConfig.bytes";
    //ab.LoadAssets                     assetbundleconfig                
    //AssetBundle.LoadFromFile          Application.streamingAssetsPath + "/assetbundleconfig
}

                                                                             