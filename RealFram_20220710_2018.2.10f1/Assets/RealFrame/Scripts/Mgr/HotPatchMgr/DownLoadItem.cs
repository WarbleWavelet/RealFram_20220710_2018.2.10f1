/****************************************************
    文件：DownLoadItem.cs
	作者：lenovo
    邮箱: 
    日期：2022/9/15 19:4:32
	功能：下载基类
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class DownLoadItem
{

    #region 字属

    protected string m_Url;                 // 网络资源URL路径
    protected string m_SavePath;            // 资源下载存放路径，不包含文件么
    protected string m_FileNameWithoutExt;  // 文件名，不包含后缀
    protected string m_FileExt;             // 文件后缀
    protected string m_FileName;            // 文件名，包含后缀
    protected string m_SaveFilePath;        // 下载文件全路径，路径+文件名+后缀
    protected long   m_FileLength;          // 原文件大小
    protected long   m_CurLength;           // 当前下载的大小
    protected bool   m_StartDownLoad;       // 是否开始下载



    #region 属性
    public string Url
    {
        get { return m_Url; }
    }

    public string SavePath
    {
        get { return m_SavePath; }
    }
    public string FileNameWithoutExt
    {
        get { return m_FileNameWithoutExt; }
    }
    public string FileExt
    {
        get { return m_FileExt; }
    }
    public string FileName
    {
        get { return m_FileName; }
    }
    public string SaveFilePath
    {
        get { return m_SaveFilePath; }
    }
    public long FileLength
    {
        get { return m_FileLength; }
    }
    public long CurLength
    {
        get { return m_CurLength; }
    }
    public bool StartDownLoad
    {
        get { return m_StartDownLoad; }
    }
    #endregion

    #endregion

    public DownLoadItem(string url, string path)
    {
        m_Url = url;
        m_SavePath = path;
        m_StartDownLoad = false;
        m_FileNameWithoutExt = Common.File_Name_WithoutSuffix(m_Url);
        m_FileExt = Common.File_Name_Suffix(m_Url);
        m_FileName = string.Format("{0}{1}", m_FileNameWithoutExt, m_FileExt);
        m_SaveFilePath = string.Format("{0}/{1}{2}", m_SavePath, m_FileNameWithoutExt, m_FileExt);
    }



    public virtual IEnumerator Download(Action callback = null)
    {
        yield return null;
    }

    /// <summary>
    /// 获取下载进度
    /// </summary>
    /// <returns></returns>
    public abstract float GetProcess();

    /// <summary>
    /// 获取当前下载的文件大小
    /// </summary>
    /// <returns></returns>
    public abstract long GetCurLength();

    /// <summary>
    /// 获取下载的文件大小
    /// </summary>
    /// <returns></returns>
    public abstract long GetLength();


    /// <summary>
    /// 关闭
    /// </summary>
    public abstract void Destory();
}

