using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Common
{




    #region 枚举 布尔

    /// <summary>
    /// 字符串转枚举
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_string"></param>
    /// <returns></returns>
    public static object String2Enum<T>(string _string, bool _bool = true)
    {
        if (_string == null) return null;
        return (T)Enum.Parse(typeof(T), _string, _bool);
    }


    /// <summary>
    /// 字符串转布尔
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_string"></param>
    /// <returns></returns>
    public static bool String2Bool(string _string)
    {
        return _string == "true" ? true : false;
    }


    public static bool Try_String2Bool(string _string)
    {
        bool _bool;
        bool.TryParse(_string, out _bool);
        return _bool;
    }

    #endregion

    #region 文件操作

    /// <summary>
    /// 向filePath写入fileContent
    /// </summary>
    /// <param name="filePath">全写，包括文件名和后缀</param>
    /// <param name="fileContent"></param>
    public static void Text_Write(string filePath, string fileContent)
    {
        FileInfo fi = new FileInfo(filePath);
        StreamWriter sw = fi.CreateText();
        sw.WriteLine(fileContent);

        sw.Close();
        sw.Dispose();
    }


    public static void AB_Clear(string path)
    {
        if (Folder_Exits(path) == false)
        {

            Debug.LogErrorFormat("该路径不存在：{0}", path);
            return;
        }

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

    }





    /// <summary>
    /// 文件夹拷贝
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public static void File_Copy(string from, string to)
    {


        try //递归拷贝
        {
            //取路径
            Common.TickPath(to);
            string toPath = Path.Combine(to, Path.GetFileName(from));// A/   B/b  =>  A/b
            toPath = Common.TrimName(toPath, TrimNameType.SlashPre);//去掉StreamingAssets
            if (File.Exists(from) == true)
            {
                toPath += Path.DirectorySeparatorChar;// Path.DirectorySeparatorChar: '\'
            }

            //取文件
            Common.TickPath(toPath);

            string[] fileArr = Directory.GetFileSystemEntries(from);
            //赋值
            foreach (string file in fileArr)
            {
                if (file.EndsWith(".meta") == true)
                {
                    continue;
                }
                if (Directory.Exists(file) == true)
                {
                    File_Copy(file, toPath);  //文件夹拷贝
                }
                else
                {

                    File.Copy(file, toPath + "/" + Path.GetFileName(file), true);//文件拷贝  ,文件夹和文件名
                }
            }

        }
        catch (Exception)
        {

            Debug.LogErrorFormat("无法复制：{0} => {1}", from, to);
        }
    }


    /// <summary>
    /// 删除文件夹下的所有文件,递归删除
    /// </summary>
    public static void Folder_Clear_Recursive(string path)
    {
        try
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileSystemInfo[] fsiArr = di.GetFileSystemInfos();

            foreach (var fsi in fsiArr)
            {
                if (fsi is DirectoryInfo)
                {
                    DirectoryInfo _di = new DirectoryInfo(fsi.FullName);
                    _di.Delete(true);
                }
                else
                {
                    File.Delete(fsi.FullName);
                }
            }

        }
        catch (Exception)
        {

            throw;
        }
    }

    /// <summary>
    /// 删除文件夹下的所有文件,除了后缀是lst的任一个 ,不递归删除
    /// </summary>
    public static void Folder_ClearWithout_NotRecursive(string path,params string[] lst)
    {
        try
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileSystemInfo[] fsiArr = di.GetFiles("*",SearchOption.AllDirectories);

            foreach (var fsi in fsiArr)
            {
                if (EndsWith(path, lst))
                {
                    continue;
                }
                 
                File.Delete(fsi.FullName);
            }

        }
        catch (Exception)
        {

            throw;
        }
    }


    /// <summary>
    /// 存在文件夹（带/也可以）
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static  bool Folder_Exits(string path)
    {
        return Directory.Exists(path);
    }


    /// <summary>
    /// 存在文件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool File_Exits(string path)
    {
        return File.Exists(path);
    }


    /// <summary>
    /// 删除该文件
    /// </summary>
    /// <param name="path"></param>
    public static void File_Delete(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
        #endregion


        #region Guid
        /// <summary>异步的Guid，为了可以取消该异步</summary> 
        static long m_asyncGuid = 0;
    /// <summary>异步的Guid，为了可以取消该异步</summary>
    public static long CreateGuid()
    {
        return m_asyncGuid++;
    }
    #endregion




    public static void Log(object obj)
    {
        UnityEngine.Debug.Log(obj.GetType().ToString() + "." + new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().ToString());//类名.方法名
    }



    #region 字符串处理



 public static string TrimName(string path, TrimNameType type)
    {
        switch (type)
        {
            case TrimNameType.SlashAfter:
                {
                    return path.Substring(path.LastIndexOf('/') + 1);//sdcvghasvdj/gdhsag/a.prefab => a.prefab
                }
            //break;             
            case TrimNameType.SlashPre:
                {
                    return path.Substring(0, path.LastIndexOf('/'));//sdcvghasvdj/gdhsag/a.prefab =>sdcvghasvdj/gdhsag
                }
            //break;
            case TrimNameType.SlashAndPoint:
                {
                    string name = path.Substring(path.LastIndexOf('/') + 1);// plane.unity3d
                    name = name.Substring(0, name.LastIndexOf('.'));// plane
                    return name;
                }
            //break;
            default:
                {
                    return path;
                } //break;
        }


    }



    public static bool NotEndsWith(string str, params string[] lst)
    {
        foreach (var item in lst)
        {
            if (str.EndsWith(item))
            {
                return false;
            }
        }

        return true;
    }


    public static bool EndsWith(string str, params string[] lst)
    {
        foreach (var item in lst)
        {
            if (str.EndsWith(item))
            {
                return true;
            }
        }

        return false;
    }
    #endregion

   


    #region 辅助
    /// <summary>
    /// AB加载，实例时Shader丢失，修复该问题
    /// </summary>
    /// <param name="go"></param>
    public static void FixShader(GameObject go, string shaderName)
    {
        MeshRenderer[] mr = go.GetComponentsInChildren<MeshRenderer>();
        List<Material> lst = new List<Material>();
        for (int i = 0; i < mr.Length; i++)
        {
            lst.AddRange(mr[i].materials);
        }
        SkinnedMeshRenderer[] smr = go.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < smr.Length; i++)
        {
            lst.AddRange(smr[i].materials);
        }

        for (int i = 0; i < lst.Count; i++)
        {
            lst[i].shader = Shader.Find(shaderName);
        }
    }
    #endregion


    public static void PlayBGMusic(AudioSource source, AudioClip clip)

    {

        source.clip = clip;
        source.Play();

    }


    public static void BindBtn(Button btn, Action action)
    {
        btn.onClick.AddListener(() =>
        {
            action();

        });
    }


    /// <summary>有就好，没有就创建</summary>
    public static void TickPath(string path)
    {
        if (Directory.Exists(path) == false) //输出path
        {
            Directory.CreateDirectory(path);
        }

    }

    /// <summary>
    /// 去除所有空格
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static string TrimAllSpace(string val)
    {
        return val.Trim().Replace(" ", "");
    }



    #region 转格式
    /// <summary>
    /// 为了可视化
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public static void Class2Xml<T>(T cfg, string outputPath)
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

       
    }


    /// <summary>
    /// 将类对象cfg转Bin，放在path下
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public static void Class2Bin<T>(T cfg, string path)
    {
        FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        fs.Seek(0, SeekOrigin.Begin);//清空
        fs.SetLength(0);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, cfg);
        fs.Close();


    }
    #endregion  
}




public enum TrimNameType
{
    None,
    /// <summary>A/B/C.prefab => C.prefab</summary>
    SlashAfter,
    /// <summary>A/B/C.prefab => A/B</summary>
    SlashPre,
    /// <summary>A/B/C.prefab => C</summary>
    SlashAndPoint,
}