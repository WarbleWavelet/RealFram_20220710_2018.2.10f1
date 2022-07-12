using UnityEngine;

public class DefinePath
{
    public static string path_Stream = Application.streamingAssetsPath + "/attack.unity3d";
    public const string path_ADB = "Assets/GameData/Prefabs/Attack.prefab";
    public static string path_Xml = Application.dataPath + "/Scenes/02 Class2Xml/test.xml";
    public static string path_Bin_Write = Application.dataPath + "/Scenes/03 Class2Bin/test.bytes";
    public const string path_Bin_Read = "Assets/Scenes/03 Class2Bin/test.bytes";
    public const string path_04_Asset_Read = "Assets/Scenes/04 ReadAsset/TestAssets.asset";
    //
    public const string ABCONFIGPATH = "Assets/Editor/ABConfig.asset";
    public static string ABSAVEPATH = Application.streamingAssetsPath;
    public static string ABSAVEPATH_XML = Application.dataPath + "/AssetBundleConfig.xml";
    public static string ABSAVEPATH_Bin = Application.streamingAssetsPath + "/AssetBundleConfig.bytes";
}

