/****************************************************
    文件：Bin2Class.cs
	作者：lenovo
    邮箱:   
    日期：2022/7/28 17:4:4
	功能： 格式转换
*****************************************************/

using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;



public class FormatTool
{


    #region Xml
    /// <summary>
    /// 运行时使用的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T Xml2Class<T>(string path) where T : class
    {
        T t = default(T);
        TextAsset textAsset = ResourceMgr.Instance.LoadResource<TextAsset>(path);

        if (textAsset == null)
        {
            UnityEngine.Debug.LogError("cant load TextAsset: " + path);
            return null;
        }

        try
        {
            using (MemoryStream stream = new MemoryStream(textAsset.bytes))  //转流
            {
                XmlSerializer xs = new XmlSerializer(typeof(T)); //xml流
                t = (T)xs.Deserialize(stream); //反
            }
            ResourceMgr.Instance.UnloadResItemByPath(path, true);//卸载
        }
        catch (Exception e)
        {
            Debug.LogError("load TextAsset exception: " + path + "," + e);
        }
        return t;
    }


    public static System.Object Xml2Class(string xmlPath, Type type)
    {
        System.Object obj = null;
        try
        {
            using (FileStream fs = new FileStream(xmlPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                XmlSerializer xs = new XmlSerializer(type);
                obj = xs.Deserialize(fs);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("此xml无法转成二进制（可能xml为空）: " + xmlPath + "," + e);
        }

        return obj;
    }
    #endregion




    #region Bin


    /// <summary>
    /// 读取二进制
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T Bin2Class<T>(string path) where T : class
    {
        T t = default(T);
        TextAsset textAsset = ResourceMgr.Instance.LoadResource<TextAsset>(path);

        if (textAsset == null)
        {
            UnityEngine.Debug.LogError("cant load TextAsset: " + path);
            return null;
        }

        try
        {
            using (MemoryStream stream = new MemoryStream(textAsset.bytes))
            {
                BinaryFormatter bf = new BinaryFormatter();
                t = (T)bf.Deserialize(stream);
            }
            ResourceMgr.Instance.UnloadResItemByPath(path, true);
        }
        catch (Exception e)
        {
            Debug.LogError("load TextAsset exception: " + path + "," + e);
        }
        return t;
    }
    #endregion



    #region Class
    /// <summary>
    /// 类转换成二进制
    /// </summary>
    /// <param name="path"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool Class2Bin(string path, System.Object obj)
    {
        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, obj);
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("此类无法转换成二进制 " + obj.GetType() + "," + e);
        }
        return false;
    }


    /// <summary>
    /// 编辑器是用的
    /// </summary>
    /// <param name="toPath"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool Class2Xml(string toPath, System.Object obj)
    {
        if (File.Exists(toPath))
        {
            File.Delete(toPath);
        }


        try
        {
            using (FileStream fs = new FileStream(toPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)) //用完自动Close
            {
                using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                {

                    //XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                    // xmlSerializerNamespaces.Add(string.Empty, string.Empty);
                    XmlSerializer xml = new XmlSerializer(obj.GetType());
                    xml.Serialize(sw, obj);

                }
            }
            return true;


        }
        catch (System.Exception)
        {

            Debug.LogErrorFormat("Xml2Class，无该类{0}", obj.GetType());
        }


        return false;
    }

    #endregion





    #region protobuf



    public static bool Class2Protobuf(string path,System.Object obj)
    {
        try
        {
            using (Stream file = File.Create(path))
            {
                Serializer.Serialize(file,obj);
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("Protobuf2Bin失败：{0}", e); ;
            return false;
        }
    }


    public static T Protobuf2Class<T>(string path) where T:class
    {
        try
        {
            using (Stream file = File.OpenRead(path))
            {
               return Serializer.Deserialize<T>(file);
            }
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("Bin2Protobuf失败：{0}", e); ;
            return null;
        }
    }


    public static byte[] Class2Protobuf( System.Object obj)
    {
        try
        {
            using (MemoryStream ms =new MemoryStream())
            {
                Serializer.Serialize(ms, obj);
                byte[] bytes = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(bytes,0,bytes.Length);
                return bytes ;
            }
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("Protobuf2Bin失败：{0}", e); ;
            return null;
        }
    }


    public static T Protobuf2Class<T>(byte[] bytes) where T : class
    {
        try
        {
            using (MemoryStream ms =new MemoryStream())
            {
                ms.Write(bytes,0,bytes.Length);
                ms.Position = 0;
                return Serializer.Deserialize<T>(ms);
            }
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("Bin2Protobuf失败：{0}", e); ;
            return null;
        }
    }
    #endregion
}




