using UnityEngine;

public class DefinePath
{
    public static string Demo01_Bytes_Attack = Application.streamingAssetsPath + "/attack";
    public const string Demo01_Attack_Prefab = "Assets/GameData/Prefabs/Attack.prefab";
    public static string Demo02_Xml = Application.dataPath + "/Scenes/01-04/02 Class2Xml/Demo02_test.xml";
    public static string Demo03_WriteBytes = Application.dataPath + "/Scenes/01-04/03 Class2Bin/Demo03_test.bytes";
    public const string Demo03_ReadBytes = "Assets/Scenes/01-04/03 Class2Bin/Demo03_test.bytes";
    public static string Demo04_Asset = "Assets/Scenes/01-04/04 ReadAsset/TestAssets.asset";
    public const string Demo04_Attack_Prefab = "Assets/GameData/Prefabs/Attack.prefab";
    //
    public const string ABCONFIGPATH = "Assets/Editor/ABConfig.asset";
    public static string ABSAVEPATH = Application.streamingAssetsPath;

    public static string Demo05_AB_Xml = Application.dataPath  + "/AssetBundleConfig.xml";//XML可视化，随便删
    public static string Demo05_Bytes_Cfg = Application.streamingAssetsPath + "/assetbundleconfig";


    //ab.LoadAssets                     assetbundleconfig                
    //AssetBundle.LoadFromFile          Application.streamingAssetsPath + "/assetbundleconfig
}

