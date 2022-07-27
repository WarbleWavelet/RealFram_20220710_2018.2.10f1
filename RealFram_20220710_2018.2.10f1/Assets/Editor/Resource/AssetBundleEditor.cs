using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using static ABCfgSO;

public class AssetBundleEditor
{


    #region 字属
    static AssetBundleWriter abWriter;
    //
    /// <summary><包名,path></summary>
    static Dictionary<string, string> m_floderDic = new Dictionary<string, string>();
    /// <summary> 单个prefab的AB包 (prefab.name, prefab_dependLst);</summary>
    static Dictionary<string, List<string>> m_prefabDic = new Dictionary<string, List<string>>();
    //
    /// <summary>m_AllFileAB  过滤表<path></summary>
    static List<string> fliter_floderLst = new List<string>();
    /// <summary>被AB标记的 (path，abName)，需要依赖项=>路径=>abName</summary>
    static Dictionary<string, string> m_abMarkDic = new Dictionary<string, string>();
    /// <summary>筛选出有效路径</summary>
    static List<string> fliter_pathLst = new List<string>();
    //
    static ABCfgSO m_abCfgSO;
    static string m_abCfgSOPath = DefinePath.ABCfgSOPath;
    /// <summary>AB的生成位置</summary>
    static string m_outputABPath = DefinePath.OutputABPath;
    static string m_outputXml = DefinePath.OutputXml;
    static string m_outputBytes = DefinePath.InputBytes;
    static string m_outputAB = DefinePath.OutputAB;
    static string m_outputABName = DefinePath.OutputABName;
    static string m_inputBytes = DefinePath.InputBytes;
    #endregion


    #region ABCfgSO
    [MenuItem(Constants.MenuItem + "/定位标记数据的SO（除了配置表）", false, 0)] //Alt+R打开资源路径 ,Unity上的路径
    static void OpenResourcesUIPanel() 
    {
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(m_abCfgSOPath);
    }


    [MenuItem(Constants.MenuItem + "/01 添加标记数据和Init类（除了配置表）", false, 1)]//按钮在菜单栏的位置
    public static void InitABCfgSO() //文件夹
    {
        m_abCfgSO = AssetDatabase.LoadAssetAtPath<ABCfgSO>(m_abCfgSOPath);
        m_abCfgSO.m_PrefabPathLst.Clear();
        m_abCfgSO.m_FolderPathLst.Clear();
        m_abCfgSO.m_PrefabPathLst.Add("Assets/GameData/Prefabs");
        m_abCfgSO.m_FolderPathLst.Add(new ABCfgSO.AB2Path { m_ABName = "sound", m_Path = "Assets/GameData/Sounds" });
        m_abCfgSO.m_FolderPathLst.Add(new ABCfgSO.AB2Path { m_ABName = "shader", m_Path = "Assets/GameData/Shaders" });
        m_abCfgSO.m_FolderPathLst.Add(new ABCfgSO.AB2Path { m_ABName = "image", m_Path = "Assets/GameData/Images" });
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    [MenuItem(Constants.MenuItem + "/Clear ABCfgSO", false, 3)]//按钮在菜单栏的位置
    public static void ClearABCfgSO() 
    {
        m_abCfgSO =  AssetDatabase.LoadAssetAtPath<ABCfgSO>(m_abCfgSOPath);
        m_abCfgSO.m_PrefabPathLst.Clear();
        m_abCfgSO.m_FolderPathLst.Clear();
        AssetDatabase.Refresh();
    }

    //[MenuItem(Constants.MenuItem + "/添加标记数据（配置表）", false, 2)]//按钮在菜单栏的位置
    public static void AddABCfgPath()//单独领出来SetABName
    {
        m_abCfgSO =  AssetDatabase.LoadAssetAtPath<ABCfgSO>(m_abCfgSOPath);    
        m_abCfgSO.m_FolderPathLst.Add(new ABCfgSO.AB2Path { m_ABName = "assetbundleconfig", m_Path = "Assets/GameData/Data/ABData" });
        AssetDatabase.Refresh();
    }
    #endregion



    #region 标记
    [MenuItem(Constants.MenuItem + "/02 标记并初始内存",false,21)]//按钮在菜单栏的位置
    public static void MarkAB()
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        // Load cfg
        m_abMarkDic.Clear();
        fliter_floderLst.Clear();
        fliter_pathLst.Clear();
        //
        m_floderDic.Clear();
        m_prefabDic.Clear();
        //

        InitFloderDic(m_abCfgSO.m_FolderPathLst);
        List<string> prefabPathLst = GetAllPath(m_abCfgSO.m_PrefabPathLst, "t:Prefab");
        InitPrefabDic(prefabPathLst);
        SetABName(m_floderDic, m_prefabDic);
        SetABName(m_outputABName, m_inputBytes);//cfg


        m_abMarkDic =GetAllAssetBundlesDic();

        //耗性能，慎用
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        //
        EditorUtility.ClearProgressBar();//前面几个，后面一个就可以
    }



    [MenuItem(Constants.MenuItem + "/清理标记", false,22)]//按钮在菜单栏的位置
    public static void Unmark()
    {
        string[] oldNameArr = AssetDatabase.GetAllAssetBundleNames();

        for (int i = 0; i < oldNameArr.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(oldNameArr[i], true);
        }
        
        AssetDatabase.Refresh();
    }
    #endregion



    #region 打包 删包
    [MenuItem(Constants.MenuItem + "/03 打包", false,41)]//按钮在菜单栏的位置
    static void BuildAB()
    {
        abWriter = new AssetBundleWriter()  //打包
        {
            m_OutputPath = m_outputABPath,//输出位置
            m_BuildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression,
            m_BuildTarget = EditorUserBuildSettings.activeBuildTarget
        };
        //
        if (Directory.Exists(abWriter.m_OutputPath) == false)
        {
            Directory.CreateDirectory(abWriter.m_OutputPath);
        }
      
        BuildPipeline.BuildAssetBundles(   //目录，模式，平台
            abWriter.m_OutputPath,
            abWriter.m_BuildAssetBundleOptions,
            abWriter.m_BuildTarget
        );
        AssetDatabase.Refresh();//有时耗时长，不要到处使用
    }


    [MenuItem(Constants.MenuItem + "/删包", false,42)]//按钮在菜单栏的位置
    static void DeleteAB()
    {
        DirectoryInfo di = new DirectoryInfo(m_outputABPath);  //搜索该文件夹
        FileInfo[] fiArr = di.GetFiles("*", SearchOption.AllDirectories);

        for (int i = 0; i < fiArr.Length; i++)//全删除
        {
            FileInfo fi=fiArr[i];
            if (ContainABName(fi.Name) 
                || fi.Name.EndsWith(".meta") 
                || fi.Name.EndsWith(".manifest") //manifest里面是资源依赖和标识符等，会重写，防止慢就不删了
                || fi.Name.EndsWith( m_outputABName) )
            {
                continue;
            }
            else
            {
                if (File.Exists(fi.FullName))
                {
                    File.Delete(fi.FullName);
                    File.Delete(fi.FullName.Replace("manifest",""));  //删除mani
                }
            }
        }

        AssetDatabase.Refresh();
    }
    #endregion



    #region 生成Xml Bin
    /// <summary>
    /// <path,ABName>
    /// </summary>
    /// <param name="dic"></param>
    [MenuItem(Constants.MenuItem+"/04 生成Xml Bin", false,61)]
    static void WriteData()
    {

        ABCfg abCfg=new ABCfg();
        abCfg.ABLst = new List<ABBase>();


        foreach (var item in m_abMarkDic) //根据m_abMarkDic，生成ABCfg
        {
            string path = item.Key;
            string abName = item.Value;
            ABBase  abBase=new ABBase //生成 
            {
                Path = path,
                Crc = CRC32.GetCRC32(path),
                ABName = abName, 
                AssetName = path.Remove( 0, item.Value.LastIndexOf("/")+1)            
            };
            abBase.ABDependce =new List<string>();
             //
            string[] dependArr = AssetDatabase.GetDependencies(abName); 
            for (int i = 0; i < dependArr.Length; i++) //遍历依赖
            {
                string dependPath=dependArr[i];
                if ( path == dependPath || dependPath.EndsWith(".cs"))//依赖项是自身或脚本
                {
                    continue;   
                }

                string _abName = "";
                if (m_abMarkDic.TryGetValue( path, out _abName))
                {
                    if (_abName == m_abMarkDic[path])//已经加了一个AB包，该AB包中有该资源
                    {
                        continue;
                    }

                    if (abBase.ABDependce.Contains(_abName) == false)//添加所需依赖
                    {
                        abBase.ABDependce.Add(_abName);
                    }
                }
            }
            abCfg.ABLst.Add(abBase);
        }

        Class2Xml(abCfg, m_outputXml); //生成Xml
        Class2Bin(abCfg, m_outputBytes); //生成bytes

        AssetDatabase.Refresh();
    
    }

   
    [MenuItem(Constants.MenuItem + "/删Xml Bin", false, 62)]//按钮在菜单栏的位置
    static void DeleteData()
    {
        DirectoryInfo di = new DirectoryInfo(  Application.dataPath );
        FileInfo[] fiArr = di.GetFiles("*", SearchOption.AllDirectories);//全搜索

        for (int i = 0; i < fiArr.Length; i++)
        {
            FileInfo fi = fiArr[i];
            if (File.Exists(m_outputXml))//找到就删除
            {
                File.Delete(m_outputXml);
            }
            if (File.Exists(m_outputBytes))
            {
                File.Delete(m_outputBytes);
            }
        }

        AssetDatabase.Refresh();
    }

    #endregion



    #region 辅助
    /// <summary>
    /// 得到所有AB的数据Dic<path,name>
    /// </summary>
    /// <returns></returns>
    static Dictionary<string, string> GetAllAssetBundlesDic()
    {
        string[] abNameArr = AssetDatabase.GetAllAssetBundleNames(); 
        Dictionary<string, string> abDic = new Dictionary<string, string>();
        for (int i = 0; i < abNameArr.Length; i++)//所有ab名
        {
            string[] pathArr = AssetDatabase.GetAssetPathsFromAssetBundle(abNameArr[i]);
            for (int j = 0; j < pathArr.Length; j++)//所有ab路径
            {
                string path = pathArr[j];
                if (path.EndsWith(".cs")==true )//过滤脚本
                {
                    continue;
                }

                Debug.Log("AB包 \"" + abNameArr[i] + "\" 包含的资源的路径：" + path);
                if (ValidPath(path)==true)//存储起来
                { 
                    abDic.Add(path, abNameArr[i]);
                }
               
            }
        }

        return abDic;//返回
    }


    /// <summary>
    /// 为了可视化
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private static void Class2Xml<T>(T cfg, string outputPath)
    {
        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }
        FileStream fs = new FileStream(outputPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
        XmlSerializer xml = new XmlSerializer(cfg.GetType());
        xml.Serialize(sw, cfg);
        sw.Close();
        fs.Close();

        AssetDatabase.Refresh();
    }


    /// <summary>
    /// 生成一个去m_inputBytes读取数据的AB包
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private static void Class2Bin<T>(T cfg, string outputPath)
    {
        FileStream fs = new FileStream(outputPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        fs.Seek(0, SeekOrigin.Begin);//清空
        fs.SetLength(0);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, cfg);
        fs.Close();


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
            //try
            //{
            //    ai.assetBundleName = abName;     //AB包的名字  
            //}
            //catch (Exception e)
            //{

            //    throw new Exception(e.ToString());
            //}
            //finally
            //{
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

    static void InitFloderDic(List<AB2Path> folderPathLst)
    {
        foreach (var item in folderPathLst)
        {
            Debug.Log(item.m_ABName + "_" + item.m_Path);
            string abName = item.m_ABName;
            string path = item.m_Path;
            //
            if (m_floderDic.ContainsKey(abName))
            {
                Debug.LogError("key重复");
            }
            else
            {
                m_floderDic.Add(abName, path);
                fliter_floderLst.Add(path);
                fliter_pathLst .Add(path);
            }
        }

    }



    static void  InitPrefabDic(List<string> prefabPathLst )
    {
        for (int i = 0; i < prefabPathLst.Count; i++)
        {
            string path = prefabPathLst[i];
            if (ContainPath(path) == false)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                string[] dependArr = AssetDatabase.GetDependencies(path);
                List<string> prefab_dependLst = new List<string>();
                fliter_pathLst.Add(path);

                for (int j = 0; j < dependArr.Length; j++)
                {
                    string depend = dependArr[j];
                    Debug.Log("依赖路径：" + depend);

                    if (ContainPath(depend) == false && depend.Equals(".cs") == false)
                    {
                        fliter_floderLst.Add(depend);
                        prefab_dependLst.Add(depend);

                    }
                }
                //fliter相同预制体
                if (m_prefabDic.ContainsKey(prefab.name) == true )
                {
                    Debug.LogError("存在相同预制体：" + prefab.name);
                }
                else
                {
                    m_prefabDic.Add(prefab.name, prefab_dependLst);
                }
            }
            EditorUtility.DisplayProgressBar("AB进度:", "Prefab:" + path, i * 1.0f / prefabPathLst.Count);
        }
    }



   static bool ContainPath( string path)
    {
        List<string> lst = fliter_floderLst;
        for (int i = 0; i < lst.Count ; i++)
        {
            if (lst[i] == path || ( path.Contains(lst[i])&& path.Replace( lst[i],"")[0]== '/'))//  Test/a  TestTT/a => /a  TT/a
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// 文件夹下文件名 == AB名？
    /// </summary>
    /// <param name="abName"></param>
    /// <returns></returns>

    static bool ContainABName(string abName)
    {
        string[] arr =AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == abName)
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
            m_OutputPath = "AssetBundles",
            m_BuildAssetBundleOptions = BuildAssetBundleOptions.None,
            m_BuildTarget = BuildTarget.StandaloneWindows64
        };
        abWriter = new AssetBundleWriter()
        {
            m_OutputPath = Application.streamingAssetsPath,//StreamingAssets
            m_BuildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression,
            m_BuildTarget = EditorUserBuildSettings.activeBuildTarget
        };

    }


    /// <summary>
    /// 是不是有效路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static bool ValidPath(string path)
    {
        foreach (var item in fliter_pathLst)
        {
            if (path.Contains(item))
            {
                
                return true ;
            }
        }

        return false;
    }

    private static void SetABName(Dictionary<string, string> floderDic, Dictionary<string, List<string>> prefabDic)
    {
        foreach (var item in floderDic)
        {
            SetABName(item.Key, item.Value);
        }

        foreach (var item in prefabDic)
        {
            SetABName(item.Key, item.Value);
        }
    }





    /// <summary>
    /// 得到所有路径下的所有预制体的路径
    /// </summary>
    /// <param name="pathLst"></param>
    /// <param name="type">"t:Prefab"</param>
    /// <returns></returns>
    static List<string> GetAllPath(List<string> pathLst, string type)
    {
        AssetDatabase.Refresh();
        string[] guidArr = AssetDatabase.FindAssets(type, m_abCfgSO.m_PrefabPathLst.ToArray());   //处理预制体
        List<string> prefabPathLst = new List<string>();
        for (int i = 0; i < guidArr.Length; i++)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guidArr[i]);
            prefabPathLst.Add(prefabPath);
        }


     

        return prefabPathLst;
    }
    #endregion



    #region 说定位
    public class EditorTool
    {


            //Alt+R打开资源路径
            [MenuItem("HSJ/快捷方式/打开UI预制路径 &R")]
            static void OpenResourcesUIPanel()
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefab/Panel/LoginPanel.prefab");
            }
            //Alt+S打开脚本路径
            [MenuItem("HSJ/快捷方式/打开Panel脚本路径 &S")]
            static void OpenScript()
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Scripts/MessageBoxPanel.cs");
            }
            //Alt+S打开指定文件夹路径
            [MenuItem("HSJ/快捷方式/打开工程目录 &O")]
            private static void OpenProjectFolder()
            {
                EditorUtility.RevealInFinder(Application.dataPath);
            }
      
    }
    #endregion
}


/// <summary>
/// 写入AB的几个参数配置
/// </summary>
public class AssetBundleWriter
{
    /// <summary>目录</summary>
    public string m_OutputPath;
    /// <summary>模式</summary>
    public BuildAssetBundleOptions m_BuildAssetBundleOptions;
    /// <summary>buildTarget</summary>
    public BuildTarget m_BuildTarget;

}



