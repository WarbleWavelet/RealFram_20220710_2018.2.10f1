/****************************************************
    文件：CfgMgr.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/28 18:36:57
	功能： ConfigManager 
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CfgMgr : Singleton<CfgMgr>
{

    #region 字属
    /// <summary>加载的配置表</summary>
	protected static Dictionary<string, ExcelBase> m_ExcelDic = new Dictionary<string, ExcelBase>();
    #endregion


   /// <summary>
   ///     增
   /// </summary>
   /// <typeparam name="T"></typeparam>
   /// <param name="binPath">二进制文件路径</param>
   /// <returns></returns>
    public  T LoadData<T>(string binPath) where T : ExcelBase
    {
        if (string.IsNullOrEmpty(binPath))//Null
        {
            return null;
        }

        if (m_ExcelDic.ContainsKey(binPath))  //重复
        {
            Debug.LogError("重复加载相同配置文件" + binPath);
            return m_ExcelDic[binPath] as T;
        }

        T data = FormatTool.Bin2Class<T>(binPath);

#if UNITY_EDITOR//各种情况忘记转Bin
        if (data == null)
        {
            Debug.Log(binPath + "不存在，从xml加载数据了！");
            string xmlPath = binPath.Replace("Bin", "Xml").Replace(".bytes", ".xml");  //注意Bin Xm命名一致，别坑自己人
            data = FormatTool.Xml2Class<T>(xmlPath);
        }
#endif

        if (data != null)
        {
            data.Init();
        }

        m_ExcelDic.Add(binPath, data);

        return data;
    }


    /// <summary>
    /// /查
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public T GetData<T>(string path) where T : ExcelBase
    {

        if (string.IsNullOrEmpty(path))
        {
            return null;
        }


        ExcelBase data = null;
        if (m_ExcelDic.TryGetValue(path, out data) == true)
        {
            return data as T;
        }
        else
        {
            data = LoadData<T>(path) ;
        }
        return null;

    }
}

