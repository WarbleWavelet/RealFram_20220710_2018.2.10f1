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
    static string m_AB_InnerPath = DefinePath.OutputABInnerPath;
    static string m_ab_Xml = DefinePath.OutputXml;
    static string m_assetbundleconfig_Path = DefinePath.abCfg_Path;


    static string m_abCfg_Name = DefinePath.abCfg_Name;
    static string m_abCfg_Bytes = DefinePath.abCfg_Bytes;
    static bool isFirstMarkAB = true;


    #endregion



    #region MenuItem
    /// <summary>
    /// 一键打包
    /// </summary>

    //放在文件夹Editor下。相邻超过10为一组有分割线
    [MenuItem(DefinePath.MenuItem_AB + "/1234 一键打包到内部", false, 81)]//按钮在菜单栏的位置
    public static void Build()
    {
        //DataEditor.Xml2BinAll();
        //AssetBundleEditor.InitABCfgSO();
        //AssetBundleEditor.MarkAB();
        //AssetBundleEditor.WriteBinAndXml();
        //AssetBundleEditor.BuildAB(m_AB_InnerPath);
        //
        DataEditor.Xml2BinAll();    //Bin下需要的二进制
        InitABCfgSO();
        MarkAB();             //第一次跳过assetbundleconfig
        WriteBin();         //生成assetbundleconfig
        MarkAB();             //第二次包括assetbundleconfig
        WriteBinAndXml();
        BuildAB(m_AB_InnerPath);
    }


    #region ABCfgSO


    [MenuItem(DefinePath.MenuItem_AB + "/定位标记数据的SO（除了配置表）", false, 0)] //Alt+R打开资源路径 ,Unity上的路径
    static void OpenResourcesUIPanel()
    {
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(m_abCfgSOPath);
    }


    [MenuItem(DefinePath.MenuItem_AB + "/01 添加标记数据和Init类（除了配置表）", false, 1)]//按钮在菜单栏的位置
    public static void InitABCfgSO() //文件夹
    {
        m_abCfgSO = AssetDatabase.LoadAssetAtPath<ABCfgSO>(m_abCfgSOPath);
        m_abCfgSO.m_PrefabPathLst.Clear();
        m_abCfgSO.m_FolderPathLst.Clear();
        string path = "Assets/" + DefinePath.RealFrame;

       m_abCfgSO.m_PrefabPathLst.Add(path + "/GameData/Prefabs");//不用加/
        m_abCfgSO.m_FolderPathLst.Add(new ABCfgSO.AB2Path { m_ABName = "sound", m_Path = path + "/GameData/Sounds" });//不用加/
        m_abCfgSO.m_FolderPathLst.Add(new ABCfgSO.AB2Path { m_ABName = "shader", m_Path = path + "/GameData/Shaders" });
        m_abCfgSO.m_FolderPathLst.Add(new ABCfgSO.AB2Path { m_ABName = "image", m_Path = path + "/GameData/Images" });
        m_abCfgSO.m_FolderPathLst.Add(new ABCfgSO.AB2Path { m_ABName = "ugui", m_Path = path + "/GameData/UGUI" });
        m_abCfgSO.m_FolderPathLst.Add(new ABCfgSO.AB2Path { m_ABName = "bytes", m_Path = path + "/GameData/Data/Bin" });
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    [MenuItem(DefinePath.MenuItem_AB + "/Clear ABCfgSO", false, 3)]//按钮在菜单栏的位置
    public static void ClearABCfgSO()
    {
        m_abCfgSO = AssetDatabase.LoadAssetAtPath<ABCfgSO>(m_abCfgSOPath);
        m_abCfgSO.m_PrefabPathLst.Clear();
        m_abCfgSO.m_FolderPathLst.Clear();
        AssetDatabase.Refresh();
    }


    #region 说明
    // [MenuItem(DefinePath.MenuItem_AB + "/添加标记数据（配置表）", false, 2)]//按钮在菜单栏的位置
    public static void AddABCfgPath()//单独领出来SetABName
    {
        m_abCfgSO = GetABCfgSO();
        m_abCfgSO.m_FolderPathLst.Add(new ABCfgSO.AB2Path { m_ABName = m_abCfg_Name, m_Path = m_assetbundleconfig_Path });
        AssetDatabase.Refresh();
    }
    #endregion

    static ABCfgSO GetABCfgSO()
    {
        return AssetDatabase.LoadAssetAtPath<ABCfgSO>(m_abCfgSOPath); 
    }
    #endregion



    #region 标记
    [MenuItem(DefinePath.MenuItem_AB + "/02 标记", false, 21)]//按钮在菜单栏的位置
    public static void MenuItem_MarkABOnly()
    {
        MarkAB();
        isFirstMarkAB = false;
    }


    [MenuItem(DefinePath.MenuItem_AB + "/0203 标记并初始内存(先打包了assetbundleconfig)",false,21)]//按钮在菜单栏的位置
    public static void MenuItem_MarkAB()
    {
        MarkAB( );
        isFirstMarkAB = false;
        WriteBin(); //生成assetbundleconfig
        MarkAB( );
        WriteBinAndXml();
    }



    [MenuItem(DefinePath.MenuItem_AB + "/清理标记", false,22)]//按钮在菜单栏的位置
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
        [MenuItem(DefinePath.MenuItem_AB + "/04 打包", false,61)]//按钮在菜单栏的位置
     public  static void MenuItem_BuildAB()
    {
        BuildAB(m_AB_InnerPath);
    }

    [MenuItem(DefinePath.MenuItem_AB + "/删包（RealFrame\\StreamingAssets）", false, 61)]//按钮在菜单栏的位置
    public static void MenuItem_DeleteAB()
    {
        DeleteAB(m_AB_InnerPath);
    }





    static void DeleteAB(string path)
    {
        DirectoryInfo di = new DirectoryInfo(path);  //搜索该文件夹
        FileInfo[] fiArr = di.GetFiles("*", SearchOption.AllDirectories);

        for (int i = 0; i < fiArr.Length; i++)//全删除
        {
            string fileFullName = fiArr[i].FullName;    // A/B/c.xxx

                if (File.Exists(fileFullName))
                {
                    File.Delete(fileFullName);//删除本身
                }
                if (File.Exists(fileFullName + ".manifest"))
                {
                    File.Delete(fileFullName + ".manifest");//删除他的manifest
                }
            
        }

        AssetDatabase.Refresh();
    }
    #endregion
   



    #region 生成Xml Bin
  
    
    [MenuItem(DefinePath.MenuItem_AB+"/03 生成Bin(Bin也要标记打包)", false,41)]
    static void MenuItem_WriteBin()
    {
        WriteBin();
    
    }       
    
    [MenuItem(DefinePath.MenuItem_AB+"/03 生成Xml", false,41)]
    static void MenuItem_WriteXml()
    {
        WriteXml();
    
    }

    /// <summary>
    /// <path,ABName>
    /// </summary>
    /// <param name="dic"></param>
    [MenuItem(DefinePath.MenuItem_AB+"/03 生成Xml Bin(Bin也要标记打包)", false,41)]
    static void MenuItem_WriteBytesAndXml()
    {
        WriteBinAndXml();
    
    }   


    [MenuItem(DefinePath.MenuItem_AB + "/删Xml Bin(RealFrame\\xml和RealFrame\\GameData\\Data\\ABData\\bytes)", false, 42)]//按钮在菜单栏的位置
    static void MenuItem_DeleteBytesAndXml()
    {

        DeleteData( Application.dataPath ,m_ab_Xml );
        DeleteData( Application.dataPath ,m_abCfg_Bytes );

    }

    #endregion




    #region 集合


    [MenuItem(DefinePath.MenuItem_AB + "/_1234 一键清除内部打包", false, 83)]//按钮在菜单栏的位置
    public static void Clear()
    {   
        ClearABCfgSO();
        DeleteData(Application.dataPath, m_ab_Xml);
        DeleteData(Application.dataPath, m_abCfg_Bytes);
        DeleteAB(m_AB_InnerPath);
        Unmark();
     

        Debug.Log("一键清除内部打包成功！");
    }
    #endregion
    #endregion



    #region 辅助

    /// <summary>
    /// 已被打包
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    static bool IsPackedAB(string fileName)
    {
        string[] abNameArr = AssetDatabase.GetAllAssetBundleNames();
        return ContainABName(abNameArr, fileName);
    }


    static void DeleteData(string path, string deletePath)
    {
        DirectoryInfo di = new DirectoryInfo(path);
        FileInfo[] fiArr = di.GetFiles("*", SearchOption.AllDirectories);//全搜索

        for (int i = 0; i < fiArr.Length; i++)
        {
            FileInfo fi = fiArr[i];
            if (File.Exists(deletePath))//找到就删除
            {
                File.Delete(deletePath);
            }
        }

        AssetDatabase.Refresh();
    }


    /// <summary>
    /// 得到所有AB的数据Dic<path,name>
    /// </summary>
    /// <returns></returns>
    static Dictionary<string, string> GetAllAssetBundlesDic()
    {
        Dictionary<string, string> abDic = new Dictionary<string, string>();
        string[] abNameArr = AssetDatabase.GetAllAssetBundleNames(); 
         abDic = new Dictionary<string, string>();
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

                //Debug.Log("AB包 \"" + abNameArr[i] + "\" 包含的资源的路径：" + path);
                if (ValidPath(path)==true)//存储起来
                { 
                    abDic.Add(path, abNameArr[i]);    
                }
               
            }
        }

        return abDic;
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
            Debug.LogErrorFormat("路径{0}下资源{1}为空",path,abName);
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


    /// <summary>
    /// 为了参数看的
    /// </summary>
    /// <param name="folderPathLst"></param>
    /// <param name="m_floderDic"></param>
    /// <param name="fliter_floderLst"></param>
    /// <param name="fliter_pathLst"></param>
    static void InitFloderDic(AB2Path[] folderPathLst,
         Dictionary<string,string> m_floderDic,
         List<string> fliter_floderLst,
         List<string> fliter_pathLst)
    {
        foreach (var item in folderPathLst)
        {
            Debug.LogFormat("包：{0}\t\t\t\t路径：{1}",item.m_ABName ,item.m_Path);
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


    public static void BuildAB(string outputABPath)
    {
        Common.TickPath(outputABPath);
        DeleteUselessAB(m_AB_InnerPath);

       //
        abWriter = new AssetBundleWriter()  //打包
        {
            m_OutputPath = outputABPath,//输出位置
            m_BuildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression,
            m_BuildTarget = EditorUserBuildSettings.activeBuildTarget
        };
        AssetBundleManifest mft = BuildPipeline.BuildAssetBundles(   //目录，模式，平台
             abWriter.m_OutputPath,
             abWriter.m_BuildAssetBundleOptions,
             abWriter.m_BuildTarget
         );

        if (mft == null) //打包结果
        {
            Debug.LogError("AB打包失败");
        }
        else
        {

            Debug.Log("AB打包完毕");
        }
        AssetDatabase.Refresh();//有时耗时长，不要到处使用
    }

    /// <summary>
    /// 根据 abMarkDic去除依赖生成ABCfg
    /// </summary>
    /// <param name="abMarkDic"></param>
    /// <returns></returns>
    static ABCfg WriteData(Dictionary<string, string> abMarkDic)
    {
        #region 依赖
        ABCfg abCfg = new ABCfg
        {
            ABLst = new List<ABBase>()
        };


        foreach (var item in abMarkDic) //根据m_abMarkDic，生成ABCfg
        {
            string path = item.Key;
            string abName = item.Value;

            if (ValidPath(path) == false)
            {
                continue;
            }

            ABBase abBase = new ABBase //生成 
            {
                Path = path,
                Crc = CRC32.GetCRC32(path),
                ABName = abName,
                AssetName = path.Remove(0, item.Value.LastIndexOf("/") + 1)
            };
            abBase.ABDependce = new List<string>();
            //
            string[] dependArr = AssetDatabase.GetDependencies(abName);
            for (int i = 0; i < dependArr.Length; i++) //遍历依赖
            {
                string dependPath = dependArr[i];
                if (path == dependPath || path.EndsWith(".cs"))//依赖项是自身或脚本
                {
                    continue;
                }

                string _abName = "";
                if (abMarkDic.TryGetValue(path, out _abName))
                {
                    if (_abName == abMarkDic[path])//已经加了一个AB包，该AB包中有该资源
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
        #endregion
        return abCfg;
    }



    /// <summary>
    /// 预制体的asset
    /// </summary>
    /// <param name="assetPathLst"></param>
    static void  InitPrefabDic(string[] assetPathLst,
         Dictionary<string, List<string>> prefabDic,
         List<string> fliter_floderLst,
         List<string> fliter_pathLst)
    {
        for (int i = 0; i < assetPathLst.Length; i++)
        {
            string path = assetPathLst[i];                
            fliter_pathLst.Add(path);
            EditorUtility.DisplayProgressBar("查找Prefab", "Prefab:" + path, i * 1.0f / assetPathLst.Length);

            if (ContainPath(fliter_floderLst,path) == false)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                string[] dependArr = AssetDatabase.GetDependencies(path);
                List<string> prefab_dependLst = new List<string>();


                for (int j = 0; j < dependArr.Length; j++)
                {
                    string depend = dependArr[j];
                    //Debug.Log("依赖路径：" + depend);

                    if (ContainPath(fliter_floderLst,depend) == false
                        && depend.EndsWith(".cs") == false)
                    {
                        fliter_floderLst.Add(depend);
                        prefab_dependLst.Add(depend);

                    }
                }
                //fliter相同预制体
                if (prefabDic.ContainsKey(prefab.name) == true )
                {
                    Debug.LogError("存在相同预制体：" + prefab.name);
                }
                else
                {
                  
                    prefabDic.Add(prefab.name, prefab_dependLst);
                }
                Debug.LogFormat("包：{0}\t\t\t\t路径：{1}", prefab.name, path);
            }
            EditorUtility.DisplayProgressBar("AB进度:", "Prefab:" + path, i * 1.0f / assetPathLst.Length);
        }
    }


    /// <summary>
    /// 删除没用的文件
    /// </summary>
    static void DeleteUselessAB(string outputABPath, string fileDontDelete="")
    {

        DirectoryInfo di = new DirectoryInfo(outputABPath);  //搜索该文件夹
        FileInfo[] fiArr = di.GetFiles("*", SearchOption.AllDirectories);

        for (int i = 0; i < fiArr.Length; i++)//全删除
        {
            string fileName = fiArr[i].Name;            //c.xxx
            string fileFullName = fiArr[i].FullName;    // A/B/c.xxx
            if (IsPackedAB(fileName)
                || fileName.EndsWith(".meta")
                || fileName.EndsWith(".manifest") //manifest里面是资源依赖和标识符等，会重写，防止慢就不删了
               // || fileName.EndsWith(fileDontDelete)//Editor演示要用到，就不自动删了
                )
            {
                continue;
            }
            else
            {
                if (File.Exists(fileFullName))
                {
                    File.Delete(fileFullName);//删除本身
                }
                if (File.Exists(fileFullName + ".manifest"))
                {
                    File.Delete(fileFullName + ".manifest");//删除他的manifest
                }
            }
        }

        AssetDatabase.Refresh();
    }


    static bool ContainPath(List<string> pathLst, string path)
    {
        for (int i = 0; i < pathLst.Count ; i++)
        {
            if (pathLst[i] == path || ( path.Contains(pathLst[i])&& path.Replace( pathLst[i],"")[0]== '/'))//  Test/a  TestTT/a => /a  TT/a
            {
                return true;
            }
        }
        return false;
    }



    #region assetBundleConfig的bin和xml
  static void WriteBinAndXml()
    {
        ABCfg abCfg = WriteData(m_abMarkDic);

        Class2Xml(abCfg, m_ab_Xml); //生成Xml
        Class2Bin(abCfg, m_abCfg_Bytes); //生成bytes
        AssetDatabase.Refresh();

    }
    static void WriteBin()
    {
        ABCfg abCfg = WriteData(m_abMarkDic);

        Class2Bin(abCfg, m_abCfg_Bytes); //生成bytes
        AssetDatabase.Refresh();

    }
    static void WriteXml()
    {
        ABCfg abCfg = WriteData(m_abMarkDic);

        Class2Xml(abCfg, m_ab_Xml); //生成Xml
        AssetDatabase.Refresh();

    }

    #endregion
  

    /// <summary>
    /// 文件夹下文件名 == AB名？
    /// </summary>
    /// <param name="abName"></param>
    /// <returns></returns>

    static bool ContainABName(string[] arr,string abName)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == abName)
            {
                return true;
            }
        }
        return false;
    }

    public static void MarkAB()
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        // Load cfg
        m_abMarkDic.Clear();
        fliter_floderLst.Clear();
        fliter_pathLst.Clear();
        m_floderDic.Clear();
        m_prefabDic.Clear();
        m_abCfgSO = GetABCfgSO();
        //
        string[] prefab_assetPathArr = GetAllPath(m_abCfgSO.m_PrefabPathLst.ToArray(), "t:Prefab");
        AB2Path[] floder_assetPathArr = m_abCfgSO.m_FolderPathLst.ToArray();
        //
        InitPrefabDic(prefab_assetPathArr, m_prefabDic, fliter_floderLst, fliter_pathLst);
        InitFloderDic(floder_assetPathArr, m_floderDic, fliter_floderLst, fliter_pathLst);

        SetABName(m_floderDic, m_prefabDic);
        try
        {
               SetABName(m_abCfg_Name, m_abCfg_Bytes);//cfg
        }
        catch (Exception)
        {


            throw new System.Exception("第一次生成AB失败"+ m_abCfg_Name+","+ m_abCfg_Bytes);
        }
           
        
      



        m_abMarkDic = GetAllAssetBundlesDic();
        //
        AssetDatabase.SaveAssets();    //耗性能，慎用
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();//前面几个，后面一个就可以
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
    static string[] GetAllPath(string[] prefabPathArr,
        string type)
    {
        AssetDatabase.Refresh();
        string[] guidArr = AssetDatabase.FindAssets(type, prefabPathArr);   //处理预制体
        List<string> assetPathLst = new List<string>();
        for (int i = 0; i < guidArr.Length; i++)
        {

                string prefabPath = AssetDatabase.GUIDToAssetPath(guidArr[i]);
                assetPathLst.Add(prefabPath);

        }


     

        return assetPathLst.ToArray();
    }
    #endregion



    #region 说定位
    public class EditorTool
    {


            //Alt+R打开资源路径
           // [MenuItem("HSJ/快捷方式/打开UI预制路径 &R")]
            static void OpenResourcesUIPanel()
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefab/Panel/LoginPanel.prefab");
            }
            //Alt+S打开脚本路径
         //   [MenuItem("HSJ/快捷方式/打开Panel脚本路径 &S")]
            static void OpenScript()
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Scripts/MessageBoxPanel.cs");
            }
            //Alt+S打开指定文件夹路径
          // [MenuItem("HSJ/快捷方式/打开工程目录 &O")]
            private static void OpenProjectFolder()
            {
                EditorUtility.RevealInFinder(Application.dataPath);
            }
      
    }
    #endregion


}


/// <summary>
/// 自定义的写入AB的几个参数配置
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



