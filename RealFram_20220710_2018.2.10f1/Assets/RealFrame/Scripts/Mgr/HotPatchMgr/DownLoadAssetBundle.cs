/****************************************************
    文件：DownLoadAssetBundle.cs
	作者：lenovo
    邮箱: 
    日期：2022/9/15 18:54:50
	功能：AB下载类
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DownLoadAssetBundle : DownLoadItem
{
    UnityWebRequest m_WebRequest;

    public DownLoadAssetBundle(string url, string path) : base(url, path)
    {

    }



    #region 重写


    public override IEnumerator Download(Action callback = null)
    {
        m_WebRequest = UnityWebRequest.Get(m_Url);
        m_StartDownLoad = true;
        m_WebRequest.timeout = 30;//s
        yield return m_WebRequest.SendWebRequest();
        m_StartDownLoad = false;

        if (m_WebRequest.isNetworkError)
        {
            Debug.LogError("Download Error" + m_WebRequest.error);
        }
        else
        {
            byte[] bytes = m_WebRequest.downloadHandler.data;
            Common.File_Create_Write(m_SaveFilePath, bytes);
            if (callback != null)
            {
                callback();
            }
        }
    }

    public override void Destory()
    {
        if (m_WebRequest != null)
        {
            m_WebRequest.Dispose();
            m_WebRequest = null;
        }
    }

    public override long GetCurLength()
    {
        if (m_WebRequest != null)
        {
            return (long)m_WebRequest.downloadedBytes;
        }
        return 0;
    }

    public override long GetLength()
    {
        return 0;
    }

    public override float GetProcess()
    {
        if (m_WebRequest != null)
        {
            return (long)m_WebRequest.downloadProgress;
        }
        return 0;
    }
    #endregion
   
}
