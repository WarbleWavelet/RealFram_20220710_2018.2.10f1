using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleEditor
{


    #region 字段 属性 构造
    static AssetBundleWriter abWriter;
    /// <summary><包名,path></summary>
    static Dictionary<string, string> m_floderDic = new Dictionary<string, string>();
    /// <summary>m_AllFileAB  过滤表<path></summary>
    static List<string> fliter_floderLst = new List<string>();
    //
    /// <summary> 单个prefab的AB包 </summary>
    static Dictionary<string, List<string>> m_prefabDic = new Dictionary<string, List<string>>();
    #endregion


    #region MenuItem
    [MenuItem(Constants.MenuItem + "/标记")]//按钮在菜单栏的位置
    public static void Build()
    {
        // Load cfg
        m_floderDic.Clear();
        fliter_floderLst.Clear();

        //
        ABCfg cfg = InitCfg();
        InitFloderDic(cfg);
        //
        string[] guidArr = AssetDatabase.FindAssets("t:Prefab", cfg.prefabPathLst.ToArray());
        for (int i = 0; i < guidArr.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guidArr[i]);
            EditorUtility.DisplayProgressBar("AB进度:", "Prefab:" + path, i * 1.0f / guidArr.Length);
            //
            InitPrefabDic(path);
        }

        SetABDicName();


        //耗性能，慎用
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        //
        EditorUtility.ClearProgressBar();//前面几个，后面一个就可以
    }


    [MenuItem(Constants.MenuItem + "/清理标记")]//按钮在菜单栏的位置
    public static void Clear()
    {
        string[] oldNameArr = AssetDatabase.GetAllAssetBundleNames();

        for (int i = 0; i < oldNameArr.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(oldNameArr[i], true);
        }

        AssetDatabase.Refresh();
    }
    #endregion

    [MenuItem(Constants.MenuItem + "/打包")]//按钮在菜单栏的位置
    static void GetABName()
    {
        string[] abNameArr = AssetDatabase.GetAllAssetBundleNames();
        Dictionary<string, string> abDic=new Dictionary<string, string>();
        for (int i = 0; i < abNameArr.Length; i++)
        {
            string[] pathArr = AssetDatabase.GetAssetPathsFromAssetBundle(abNameArr[i]);
            for (int j = 0; j < pathArr.Length; j++)
            {
                Debug.Log("AB包 \"" + abNameArr[i] + "\" 包含的资源的路径：" + pathArr[j]);
                abDic.Add(pathArr[j],abNameArr[i] );
            }
        }
        //打包
        abWriter = new AssetBundleWriter()
        {
            outputPath = Application.streamingAssetsPath,//StreamingAssets
            buildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression,
            buildTarget = EditorUserBuildSettings.activeBuildTarget
        };
        //
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
        AssetDatabase.Refresh();//有时耗时长，不要到处使用
    }

    #region MenuItem 01
    [MenuItem(Constants.MenuItem + "/Build AssetBundles")]//按钮在菜单栏的位置
    static void BuildAllAssetBundles()//不能传参
    {
        abWriter = new AssetBundleWriter()
        {
            outputPath = Application.streamingAssetsPath,//StreamingAssets
            buildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression,
            buildTarget = EditorUserBuildSettings.activeBuildTarget
        };
        //
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
        AssetDatabase.Refresh();//有时耗时长，不要到处使用
    }
    #endregion


    #region 辅助




    /// <summary>
    /// 设置两个AB包Dic的名字
    /// </summary>

    static void SetABDicName()
    {
        foreach (var item in m_floderDic)
        {
            SetABName(item.Key, item.Value);
        }

        foreach (var item in m_prefabDic)
        {
            SetABName(item.Key, item.Value);
        }
    }


    static void SetABName(string abName, string path)
    { 
        AssetImporter ai=AssetImporter.GetAtPath(path);
        if (ai == null)
        {
            Debug.LogError("路径" + path + "下资源" + abName + "为空");
        }
        else
        { 
            ai.assetBundleName = abName;
        }
    }

    static void SetABName(string abName, List<string> pathLst)
    {
        for (int i = 0; i < pathLst.Count; i++)
        {
            SetABName(abName, pathLst[i]);
        }

    }

    static void InitFloderDic(ABCfg cfg)
    {
        foreach (var item in cfg.folderPathLst)
        {
            Debug.Log(item.ABName + "_" + item.Path);
            string key = item.ABName;
            string value = item.Path;
            //
            if (m_floderDic.ContainsKey(key))
            {
                Debug.LogError("key重复");
            }
            else
            {
                m_floderDic.Add(key, value);
                fliter_floderLst.Add(value);
            }
        }

    }



    static void  InitPrefabDic(string path )
    {
        if (ContainPath(path) == false)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            string[] dependArr = AssetDatabase.GetDependencies(path);
            List<string> prefab_dependLst = new List<string>();
            for (int j = 0; j < dependArr.Length; j++)
            {
                string dependPath = dependArr[j];
                Debug.Log("依赖路径：" + dependPath);

                if (ContainPath(dependPath) == false && dependPath.Equals(".cs") == false)
                {
                    fliter_floderLst.Add(dependPath);
                    prefab_dependLst.Add(dependPath);

                }
            }
            //fliter相同预制体
            if (m_prefabDic.ContainsKey(prefab.name))
            {
                Debug.LogError("存在相同预制体：" + prefab.name);
            }
            else
            {
                m_prefabDic.Add(prefab.name, prefab_dependLst);
            }
        }

    }

    /// <summary>
    /// 防止修改时丢失数据
    /// </summary>
    /// <param name="abConfig"></param>
    static ABCfg InitCfg()
    {
        ABCfg cfg = AssetDatabase.LoadAssetAtPath<ABCfg>(DefinePath.ABCONFIGPATH);
        cfg.prefabPathLst.Clear();
        cfg.folderPathLst.Clear();

        cfg.prefabPathLst.Add("Assets/GameData/Prefabs");
        cfg.folderPathLst.Add(new ABCfg.AB2Path { ABName = "sound", Path = "Assets/GameData/Sounds" });
        cfg.folderPathLst.Add(new ABCfg.AB2Path { ABName = "shader", Path = "Assets/GameData/Shaders" });

        return cfg;
    }

   static bool ContainPath( string path)
    {
        List<string> lst = fliter_floderLst;
        for (int i = 0; i < lst.Count ; i++)
        {
            if (lst[i] == path || path.Contains(lst[i]))
            {
                return true;
            }
        }
        return false;
    }


    void SomeTypeRecord()
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
    #endregion
}
/// <summary>
/// 写入AB的几个参数配置
/// </summary>
class AssetBundleWriter
{
    /// <summary>目录</summary>
    public string outputPath;
    /// <summary>模式</summary>
    public BuildAssetBundleOptions buildAssetBundleOptions;
    /// <summary>buildTarget</summary>
    public BuildTarget buildTarget;

}