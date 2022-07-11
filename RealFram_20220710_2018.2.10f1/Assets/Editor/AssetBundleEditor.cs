using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleEditor
{
   static AssetBundleWriter abWriter;

    void Start()
    {
        abWriter = new AssetBundleWriter()
        {
            outputPath = "AssetBundles",
            buildAssetBundleOptions = BuildAssetBundleOptions.None,
            buildTarget = BuildTarget.StandaloneWindows64
        };
        abWriter = new AssetBundleWriter()
        {
            outputPath = Application.streamingAssetsPath,//StreamingAssets
            buildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression,
            buildTarget = EditorUserBuildSettings.activeBuildTarget
        };

    }

    [MenuItem(Constants.MenuItem + "/Print ABConfig")]//按钮在菜单栏的位置
    public static void Build()
    {
        ABConfig abConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(DefinePath.ABCONFIGPATH);
        foreach (var item in abConfig.m_AllPrefabPath)
        {
            Debug.Log(item);
        }
        foreach (var item in abConfig.m_AllFileDirAB)
        {
            Debug.Log(item.ABName+"_"+item.Path);
        }
    }


    [MenuItem(Constants.MenuItem+"/Build AssetBundles")]//按钮在菜单栏的位置
    static void BuildAllAssetBundles()//不能传参
    {
        if (Directory.Exists(abWriter.outputPath) == false)
        {
            Directory.CreateDirectory(abWriter.outputPath);
        }
        //目录，模式，平台
        BuildPipeline.BuildAssetBundles(
            abWriter.outputPath,
            abWriter.buildAssetBundleOptions,
            abWriter.buildTarget
        );
        AssetDatabase.Refresh();
    }
}
class AssetBundleWriter
{
    /// <summary>目录</summary>
    public string outputPath;
    /// <summary>模式</summary>
    public BuildAssetBundleOptions buildAssetBundleOptions;
    /// <summary>buildTarget</summary>
    public BuildTarget buildTarget;

}