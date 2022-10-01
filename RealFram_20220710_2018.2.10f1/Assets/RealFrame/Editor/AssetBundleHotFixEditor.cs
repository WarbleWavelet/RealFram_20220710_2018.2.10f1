/****************************************************
    文件：AssetBundleHotFixEditor.cs
	作者：lenovo
    邮箱: 
    日期：2022/9/14 15:58:35
	功能：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class AssetBundleHotFixEditor
{

    static Dictionary<string, ABMD5Base> m_abMD5Dic = new Dictionary<string, ABMD5Base>(); //MD5
    static string m_Hot_OutterPath = DefinePath.Hot_OutterPath + Common_Editor.BuildTarget;
    static string m_AB_InnerPath = DefinePath.OutputAB_InnerPath + Common_Editor.BuildTarget + "/";
    static string m_AB_OutterPath = DefinePath.OutputAB_OutterPath;
    static string m_ABMD5_InnerPath = DefinePath.ABMD5_InnerPath;
    static string m_ABMD5_OutterPath = DefinePath.ABMD5_OutterPath + Common_Editor.BuildTarget + "/";


    [MenuItem(DefinePath.MenuItem_AB + "AB热更编辑器", false, DefinePath.MenuItem_Index_AB_ABHotfix)]
    static void MenuItem_AB_Init()
    {
        AssetBundleHotFixEWnd.GetWnd();
    }



    #region MD5
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hotfix">是否热更</param>
    /// <param name="abMD5Path"></param>
    /// <param name="hotCnt">热更次数</param>
    public static void BuildAB(bool hotfix = false, string abMD5Path = "", string hotCnt = "1")
    {
        if (hotfix)
        {
            ReadMD5Com(abMD5Path, hotCnt);
        }
        else
        {
            WriteABMD5();
        }

    }


    /// <summary>
    /// 写入MD5（RealFrame/Resources/ABMD5.bytes）
    /// </summary>
  public  static void WriteABMD5()
    {
        DirectoryInfo di = new DirectoryInfo(m_AB_OutterPath);
        FileInfo[] fiArr = di.GetFiles("*", SearchOption.AllDirectories);
        ABMD5 abmd5 = new ABMD5();
        abmd5.ABMD5Lst = new List<ABMD5Base>();
        for (int i = 0; i < fiArr.Length; i++)
        {
            if (fiArr[i].Name.EndsWith(".meta") == false && fiArr[i].Name.EndsWith(".manifest") == false)
            {
                ABMD5Base abmd5Base = new ABMD5Base
                {
                    Name = fiArr[i].Name,
                    MD5 = MD5Mgr.Instance.BuildFileMD5(fiArr[i].FullName),
                    Size = fiArr[i].Length / 1024.0f // KB
                };


                abmd5.ABMD5Lst.Add(abmd5Base);
            }
        }
        string innerPath = string.Format("{0}ABMD5.bytes", m_ABMD5_InnerPath); //内部生成
        Common.Class2Bin(abmd5, innerPath);

        Common.Folder_New(m_ABMD5_OutterPath);//外部拷贝
        string outterPath = string.Format("{0}ABMD5_{1}.bytes", m_ABMD5_OutterPath, PlayerSettings.bundleVersion);
        Common.File_Delete(outterPath); //清空
        File.Copy(innerPath, outterPath);
    }


    static void ReadMD5Com(string abmd5Path, string hotCnt)
    {
        m_abMD5Dic.Clear();
        using (FileStream fileStream = new FileStream(abmd5Path, FileMode.Open, FileAccess.Read))
        {
            BinaryFormatter bf = new BinaryFormatter();
            ABMD5 abmd5 = bf.Deserialize(fileStream) as ABMD5;
            foreach (ABMD5Base abmd5Base in abmd5.ABMD5Lst)
            {
                m_abMD5Dic.Add(abmd5Base.Name, abmd5Base);
            }
        }

        List<string> changeList = new List<string>();
        DirectoryInfo directory = new DirectoryInfo(m_AB_OutterPath);
        FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            if (!files[i].Name.EndsWith(".meta") && !files[i].Name.EndsWith(".manifest"))
            //if (Common.NotEndsWith(files[i].Name, ".meta", ".manifest"))
            {
                string name = files[i].Name;
                string md5 = MD5Mgr.Instance.BuildFileMD5(files[i].FullName);
                ABMD5Base abMD5Base = null;
                if (m_abMD5Dic.ContainsKey(name) == false)//新AB
                {
                    changeList.Add(name);
                }
                else
                {
                    if (m_abMD5Dic.TryGetValue(name, out abMD5Base))
                    {
                        if (md5 != abMD5Base.MD5)//发生变化
                        {
                            changeList.Add(name);
                        }
                    }
                }
            }
        }

        CopyABAndGeneratXml(changeList, hotCnt); //拷贝
    }

    /// <summary>
    /// 拷贝筛选的AB包及自动生成服务器配置表
    /// </summary>
    /// <param name="changeLst"></param>
    /// <param name="hotCnt"></param>
    static void CopyABAndGeneratXml(List<string> changeLst, string hotCnt)
    {

        Common.Folder_New(m_Hot_OutterPath);
        Common.Folder_ClearWithout_NotRecursive(m_Hot_OutterPath);

        foreach (string str in changeLst)
        {
            if (!str.EndsWith(".manifest"))
            {
                File.Copy(m_AB_OutterPath + "/" + str, m_Hot_OutterPath + "/" + str);
            }
        }

        //生成服务器Patch
        DirectoryInfo directory = new DirectoryInfo(m_Hot_OutterPath);
        FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
        Patches pathces = new Patches();
        pathces.Version = 1;
        pathces.Files = new List<Patch>();
        for (int i = 0; i < files.Length; i++)
        {
            Patch patch = new Patch();
            patch.Md5 = MD5Mgr.Instance.BuildFileMD5(files[i].FullName);
            patch.Name = files[i].Name;
            patch.Size = files[i].Length / 1024.0f;
            patch.Platform = EditorUserBuildSettings.activeBuildTarget.ToString();
            //Apache D:\ProgramFilesTrim\Apache\Apache24\htdocs\AssetBundle\0.1\1\...
            patch.Url = "http://127.0.0.1:8081/AssetBundle/" + PlayerSettings.bundleVersion + "/" + hotCnt + "/" + files[i].Name;
            pathces.Files.Add(patch);
        }
        FormatTool.Class2Xml(m_Hot_OutterPath + "/Patch.xml", pathces);
    }
    #endregion

                                                                                

}
