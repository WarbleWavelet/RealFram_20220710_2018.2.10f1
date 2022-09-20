/****************************************************
    文件：HotPatchManager.cs
	作者：lenovo
    邮箱: 
    日期：2022/9/15 14:32:50
	功能：热更配置表下载和比较

    特点：基准包是最原始的版本，而不是上一次的版本做对比
          自动重新下载，
          下载失败的回调

    内容： 主要流程：
            1，设置热更配表数据结构
            2，生成热更包及热更配表放到服务器
            3，下载热更配置表读取需要热更文件
            4，下载热更文件
            5，游戏中加载热更

            其他：
            1，自动生成热更包及自动生成热更配置表
            2，本地记录下载热更版本，方便比较
            3，热更下载中断等处理
            4，热更资源下载后校验
            5，热更回退
            6，资源加密，解密
            7，资源解压

    优点：版本回退，
    缺点：服务器存储大；MD5消耗时间
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class HotPatchMgr : Singleton<HotPatchMgr>
{


    #region 字属


    private string m_CurVersion;  //当前版本号
    private string m_CurPackName;  //当前包名
    
    //
    private MonoBehaviour m_Mono;                                                       //下载的线程
    //private string m_hotCfg = "http://127.0.0.1/ServerInfo.xml";                        //热更配置表
    private string m_hotCfg = "http://localhost:8081/ServerInfo.xml";
    private string m_ServerXmlPath = Application.persistentDataPath + "/ServerInfo.xml";

    //
    private Dictionary<string, Patch> m_HotFixDic = new Dictionary<string, Patch>();    //所有热更的东西
    private List<Patch> m_DownLoadList = new List<Patch>();                             //所有需要下载的东西
    private Dictionary<string, Patch> m_DownLoadDic = new Dictionary<string, Patch>();  //所有需要下载的东西的Dic
    private Pathces m_CurrentPatches;                                                   //当前热更Patches
    public Action ServerInfoError;                                                      //服务器列表获取错误回调
    public Action<string> ItemError;                                                    //文件下载出错回调
    public Action LoadOver;                                                             //下载完成回调 
    public List<Patch> m_AlreadyDownList = new List<Patch>();                           //储存已经下载的资源  
    public bool StartDownload = false;                                                  //是否开始下载
    private int m_TryDownCount = 0;                                                     //尝试重新下载次数
    private const int DOWNLOADCOUNT = 4;                                                //重复下载几次还失败就报错
    private DownLoadAssetBundle m_CurDownload = null;                                   //当前正在下载的资源

 



    private string m_UnPackPath   =   Application.persistentDataPath + "/Origin";
    private string m_DownLoadPath = Application.persistentDataPath + "/DownLoad";       //m_DownLoadPath = "C:/Users/lenovo/AppData/LocalLow/DefaultCompany/RealFrame_Test/DownLoad"
    private string m_LocalXmlPath = Application.persistentDataPath + "/LocalInfo.xml";  //类似D盘，大版本安装不删除
    //
    private ServerInfo m_ServerInfo;
    private ServerInfo m_LocalInfo;
    //
    private VersionInfo m_GameVersion;
                                                                                                           
    private Dictionary<string, string> m_DownLoadMD5Dic = new Dictionary<string, string>(); //服务器上的资源名对应的MD5，用于下载后MD5校验
    private List<string> m_UnPackedList = new List<string>();
    private Dictionary<string, ABMD5Base> m_PackedMd5 = new Dictionary<string, ABMD5Base>();//原包记录的MD5码
     //
    public int LoadFileCount { get; set; } = 0;     // 需要下载的资源总个数
    public float LoadSumSize { get; set; } = 0;     // 需要下载资源的总大小 KB
     //
    public bool StartUnPack = false;                 //是否开始解压
    private string m_VersionTextName= "Version";
    private string m_AB_InnerPath=DefinePath.OutputAB_InnerPath + Common.GetBuildTarget() + "/";

    public float UnPackSumSize { get; set; } = 0;    //解压文件总大小
    public float AlreadyUnPackSize { get; set; } = 0;//已解压大小

    public Pathces CurrentPatches
    {
        get { return m_CurrentPatches; }
    }
    public string CurVersion
    {
        get { return m_CurVersion; }
    }


    MD5Mgr md5Mgr=MD5Mgr.Instance;

    #endregion




    #region Version



    /// <summary>
    /// 检测版本是否热更
    /// </summary>
    /// <param name="hotCallBack"></param>
    public void CheckVersion(Action<bool> hotCallBack = null)
    {
        m_TryDownCount = 0;
        m_HotFixDic.Clear();
        ReadVersion();
        m_Mono.StartCoroutine(ReadXml(() =>
        {
            if (m_ServerInfo == null)
            {
                if (ServerInfoError != null)  //服务器表检查出错
                {
                    ServerInfoError();
                }
                return;
            }

            foreach (VersionInfo version in m_ServerInfo.GameVersion)
            {
                if (version.Version == m_CurVersion)
                {
                    m_GameVersion = version;
                    break;
                }
            }

            GetHotAB(); 
            if (CheckLocalAndServerPatch())
            {
                ComputeDownloadInfo();
                if (Common.File_Exits(m_ServerXmlPath))
                {
                    Common.File_Delete(m_LocalXmlPath);//删本地
                    Common.File_Move(m_ServerXmlPath, m_LocalXmlPath); //拿服务器的
                }
            }
            else //完全一致
            {
                ComputeDownloadInfo();
            }
            LoadFileCount = m_DownLoadList.Count;
            LoadSumSize = m_DownLoadList.Sum(x => x.Size);
            if (hotCallBack != null)
            {
                hotCallBack(m_DownLoadList.Count > 0);
            }
        }));
    }

    /// <summary>
    /// 读取打包时的版本
    /// </summary>
    void ReadVersion()
    {
        TextAsset versionTex = Resources.Load<TextAsset>(m_VersionTextName);
        if (versionTex == null)
        {
            Debug.LogError("未读到本地版本！");
            return;
        }
        string[] all = versionTex.text.Split('\n');
        if (all.Length > 0)
        {
            string[] infoList = all[0].Split(';');
            if (infoList.Length >= 2)
            {
                m_CurVersion = infoList[0].Split('|')[1];
                m_CurPackName = infoList[1].Split('|')[1];
            }
        }
    }



    IEnumerator ReadXml(Action callBack)
    {
        string xmlUrl = m_hotCfg; //xmlUrl = "http://127.0.0.1/ServerInfo.xml"
        UnityWebRequest webRequest = UnityWebRequest.Get(xmlUrl);
        webRequest.timeout = 30;  //超时时间30s
        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError)//超时错误
        {
            Debug.Log("Download Error" + webRequest.error);
        }
        else//写文件
        {
            Common.File_Create_Write(m_ServerXmlPath, webRequest.downloadHandler.data); //m_ServerXmlPath = "C:/Users/lenovo/AppData/LocalLow/DefaultCompany/RealFrame/ServerInfo.xml"

            if (Common.File_Exits(m_ServerXmlPath))
            {
                m_ServerInfo = FormatTool.Xml2Class(m_ServerXmlPath, typeof(ServerInfo)) as ServerInfo;
            }
            else
            {
                Debug.LogError("热更配置读取错误！");
            }
        }

        if (callBack != null)
        {
            callBack();
        }
    }




    /// <summary>
    /// 获取所有热更包信息
    /// </summary>
    void GetHotAB()
    {
        if (NeedHotfix())   
        {
            Pathces lastPatches = m_GameVersion.Pathces[m_GameVersion.Pathces.Length - 1];  //基于最开始的版本
            if (lastPatches != null && lastPatches.Files != null)
            {
                foreach (Patch patch in lastPatches.Files)
                {
                    m_HotFixDic.Add(patch.Name, patch);
                }
            }
        }
    }


    bool NeedHotfix()
    {
        return m_GameVersion != null          //版本
                 && m_GameVersion.Pathces != null     //热更包
                 && m_GameVersion.Pathces.Length > 0;

    }
    #endregion

    public void InitMgr(MonoBehaviour mono)
    {
        m_Mono = mono;
        ReadMD5();
    }

    /// <summary>
    /// 读取本地资源MD5码
    /// </summary>
    void ReadMD5()
    {
        m_PackedMd5.Clear();
        TextAsset md5 = Resources.Load<TextAsset>("ABMD5");
        if (md5 == null)
        {
            Debug.LogError("未读取到本地MD5");
            return;
        }

        using (MemoryStream stream = new MemoryStream(md5.bytes))
        {
            BinaryFormatter bf = new BinaryFormatter();
            ABMD5 abmd5 = bf.Deserialize(stream) as ABMD5;
            foreach (ABMD5Base abmd5Base in abmd5.ABMD5Lst)
            {
                m_PackedMd5.Add(abmd5Base.Name, abmd5Base);
            }
        }
    }

    /// <summary>
    /// 计算需要解压的文件
    /// </summary>
    /// <returns></returns>
    public bool ComputeUnPackFile()
    {
#if UNITY_ANDROID
        if (!Directory.Exists(m_UnPackPath))
        {
            Directory.CreateDirectory(m_UnPackPath);
        }
        m_UnPackedList.Clear();
        foreach (string fileName in m_PackedMd5.Keys)
        {
            string filePath = m_UnPackPath + "/" + fileName;
            if (File.Exists(filePath))
            {
                string md5 = MD5Manager.Instance.BuildFileMd5(filePath);
                if (m_PackedMd5[fileName].Md5 != md5)
                {
                    m_UnPackedList.Add(fileName);
                }
            }
            else
            {
                m_UnPackedList.Add(fileName);
            }
        }

        foreach (string fileName in m_UnPackedList)
        {
            if (m_PackedMd5.ContainsKey(fileName))
            {
                UnPackSumSize += m_PackedMd5[fileName].Size;
            }
        }

        return m_UnPackedList.Count > 0;
#else
        return false;
#endif
    }

    /// <summary>
    /// 获取解压进度
    /// </summary>
    /// <returns></returns>
    public float GetUnpackProgress()
    {
        return AlreadyUnPackSize / UnPackSumSize;
    }

    /// <summary>
    /// 开始解压文件
    /// </summary>
    /// <param name="callBack"></param>
    public void StartUnackFile(Action callBack)
    {
        StartUnPack = true;
        m_Mono.StartCoroutine(UnPackToPersistentDataPath(callBack));
    }

    /// <summary>
    /// 将包里的原始资源解压到本地
    /// </summary>
    /// <param name="callBack"></param>
    /// <returns></returns>
    IEnumerator UnPackToPersistentDataPath(Action callBack)
    {
        foreach (string fileName in m_UnPackedList)
        {
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(m_AB_InnerPath + fileName);
            unityWebRequest.timeout = 30;
            yield return unityWebRequest.SendWebRequest();
            if (unityWebRequest.isNetworkError)
            {
                Debug.Log("UnPack Error" + unityWebRequest.error);
            }
            else
            {
                byte[] bytes = unityWebRequest.downloadHandler.data;
                Common.File_Create_Write(m_UnPackPath + "/" + fileName, bytes);
            }

            if (m_PackedMd5.ContainsKey(fileName))
            {
                AlreadyUnPackSize += m_PackedMd5[fileName].Size;
            }

            unityWebRequest.Dispose();
        }

        if (callBack != null)
        {
            callBack();
        }

        StartUnPack = false;
    }

    /// <summary>
    /// 计算AB包路径
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string ComputeABPath(string name)
    {
        Patch patch = null;
        m_HotFixDic.TryGetValue(name, out patch);
        if (patch != null)
        {
            return m_DownLoadPath + "/" + name;
        }
        return "";
    }




    /// <summary>
    /// 检查本地热更信息与服务器热更信息比较 (ServerInfo.xml)
    /// </summary>
    /// <returns></returns>
    bool CheckLocalAndServerPatch()
    {
        if (Common.File_Exits(m_LocalXmlPath) == false)//首次热更
        { 
            return true;
        }
        

        m_LocalInfo =FormatTool.Xml2Class(m_LocalXmlPath, typeof(ServerInfo)) as ServerInfo;

        VersionInfo localGameVesion = null;
        if (m_LocalInfo != null)
        {
            foreach (VersionInfo version in m_LocalInfo.GameVersion)
            {
                if (version.Version == m_CurVersion)
                {
                    localGameVesion = version;//获得版本信息
                    break;
                }
            }
        }




        if (localGameVesion != null
            && localGameVesion.Pathces != null
            && NeedHotfix()
            && Pathce_Version(m_GameVersion) != Pathce_Version(localGameVesion))//最后一次Patch的Version是否一致
        { 
             return true;
        }
          

        return false;
    }

    /// <summary>
    /// 返回最后一次Patch的Version
    /// </summary>
    /// <param name="versionInfo"></param>
    /// <returns></returns>
    int Pathce_Version(VersionInfo versionInfo)
    {
        return versionInfo.Pathces[versionInfo.Pathces.Length - 1].Version;
    }




    #region 下载


     /// <summary>
    /// 计算下载的资源信息
    /// </summary>
    void ComputeDownloadInfo()
    {
        m_DownLoadList.Clear();
        m_DownLoadDic.Clear();
        m_DownLoadMD5Dic.Clear();
        if (NeedHotfix())
        {
            m_CurrentPatches = m_GameVersion.Pathces[m_GameVersion.Pathces.Length - 1];
            if (m_CurrentPatches.Files != null && m_CurrentPatches.Files.Count > 0)
            {
                foreach (Patch patch in m_CurrentPatches.Files)
                {
                    if (   IsPlatform( patch, RuntimePlatform.WindowsPlayer,"StandaloneWindows64") )
                    {
                        AddDownLoadList(patch);
                    }
                    else if (IsPlatform(patch, RuntimePlatform.Android, "StandaloneWindows64"))
                    {
                        AddDownLoadList(patch);
                    }
                    else if (IsPlatform(patch, RuntimePlatform.IPhonePlayer, "StandaloneWindows64"))
                    {
                        AddDownLoadList(patch);
                    }
                }
            }
        }
    }

   /// <summary>
   /// 什麼平台
   /// </summary>
   /// <param name="patch"></param>
   /// <param name="runtimePlatform"></param>
   /// <param name="platformName"></param>
   /// <returns></returns>
    bool IsPlatform(Patch patch, RuntimePlatform runtimePlatform, string platformName)
    {
        bool res= ( Application.platform == runtimePlatform || Application.platform == RuntimePlatform.WindowsEditor)
                  && patch.Platform.Contains(platformName);

        return res;
    }


   /// <summary>
   /// 添加到下载队列的情况
   /// </summary>
   /// <param name="patch"></param>
    void AddDownLoadList(Patch patch)
    {
        patch.Url = patch.Url.Replace("http://127.0.0.1","http://localhost:8081");
        string filePath = m_DownLoadPath + "/" + patch.Name;
     
        if (Common.File_Exits(filePath))    //存在这个文件时进行对比看是否与服务器MD5码一致，不一致放到下载队列，如果不存在直接放入下载队列
        {
            string md5 = md5Mgr.BuildFileMD5(filePath);
            if (patch.Md5 != md5)
            {
                AddDownLoad(patch);
            }
        }
        else
        {
            AddDownLoad(patch);
        }
    }

     void AddDownLoad(Patch patch)
    {
        m_DownLoadList.Add(patch);
        m_DownLoadDic.Add(patch.Name, patch);
        m_DownLoadMD5Dic.Add(patch.Name, patch.Md5);
    }
    #endregion




    /// <summary>
    /// 获取下载进度
    /// </summary>
    /// <returns></returns>
    public float GetDownloadProgress()
    {
        return GetLoadSize() / LoadSumSize;
    }

    /// <summary>
    /// 获取已经下载总大小
    /// </summary>
    /// <returns></returns>
    public float GetLoadSize()
    {
        float alreadySize = m_AlreadyDownList.Sum(x => x.Size);
        float curAlreadySize = 0;
        if (m_CurDownload != null)
        {
            Patch patch = FindPatchByGamePath(m_CurDownload.FileName);
            if (patch != null && !m_AlreadyDownList.Contains(patch))
            {
                curAlreadySize = m_CurDownload.GetProcess() * patch.Size;
            }
        }

        return alreadySize + curAlreadySize;
    }

    /// <summary>
    /// 开始下载AB包
    /// </summary>
    /// <param name="callBack"></param>
    /// <returns></returns>
    public IEnumerator StartDownLoadAB(Action callBack, List<Patch> patchLst = null)
    {
        m_AlreadyDownList.Clear();
        StartDownload = true;
        if (patchLst == null)
        {
            patchLst = m_DownLoadList;
        }
        Common.TickPath(m_DownLoadPath);


        List<DownLoadAssetBundle> downLoadAssetBundleLst = new List<DownLoadAssetBundle>();
        foreach (Patch patch in patchLst)
        {
            downLoadAssetBundleLst.Add(new DownLoadAssetBundle(patch.Url, m_DownLoadPath));
        }

        foreach (DownLoadAssetBundle downLoad in downLoadAssetBundleLst)
        {
            m_CurDownload = downLoad;
            yield return m_Mono.StartCoroutine(downLoad.Download());
            Patch patch = FindPatchByGamePath(downLoad.FileName);
            if (patch != null)
            {
                m_AlreadyDownList.Add(patch);
            }
            downLoad.Destory();
        }

        //MD5码校验,
        VerifyMD5(downLoadAssetBundleLst, callBack);
    }

    /// <summary>
    /// Md5码校验,如果校验没通过，自动重新下载没通过的文件，重复下载计数，达到一定次数后，反馈某某文件下载失败
    /// </summary>
    /// <param name="downLoadAssetLst"></param>
    /// <param name="callBack"></param> 
    void VerifyMD5(List<DownLoadAssetBundle> downLoadAssetLst, Action callBack)
    {
        List<Patch> downLoadList = new List<Patch>();
        foreach (DownLoadAssetBundle downLoad in downLoadAssetLst)
        {
            string oldMD5 = "";
            if (m_DownLoadMD5Dic.TryGetValue(downLoad.FileName, out oldMD5))
            {
                string newMD5 = md5Mgr.BuildFileMD5(downLoad.SaveFilePath);
                if (newMD5 != oldMD5)//不一样
                {
                    Debug.LogErrorFormat(string.Format("{0}此文件MD5校验失败，即将重新下载", downLoad.FileName));
                    Patch patch = FindPatchByGamePath(downLoad.FileName);
                    if (patch != null)
                    {
                        downLoadList.Add(patch); //放下载列表
                    }
                }
            }
        }

        if (downLoadList.Count <= 0)//都正确
        {
            m_DownLoadMD5Dic.Clear();
            if (callBack != null)
            {
                StartDownload = false;//下载结束
                callBack();
            }
            if (LoadOver != null)
            {
                LoadOver();
            }
        }
        else
        {
            if (m_TryDownCount >= DOWNLOADCOUNT)
            {
                string allName = "";
                StartDownload = false;
                foreach (Patch patch in downLoadList)
                {
                    allName += patch.Name + ";";
                }
                Debug.LogErrorFormat("资源重复下载{0}次MD5校验都失败，请检查资源{1}" , DOWNLOADCOUNT,  allName);
                if (ItemError != null)
                {
                    ItemError(allName);
                }
            }
            else
            {
                m_TryDownCount++;
                m_DownLoadMD5Dic.Clear();
                foreach (Patch patch in downLoadList)
                {
                    m_DownLoadMD5Dic.Add(patch.Name, patch.Md5);
                }
                m_Mono.StartCoroutine(  StartDownLoadAB(callBack, downLoadList)  ); //自动重新下载校验失败的文件
            }
        }
    }

    /// <summary>
    /// 根据名字查找对象的热更Patch
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Patch FindPatchByGamePath(string name)
    {
        Patch patch = null;
        m_DownLoadDic.TryGetValue(name, out patch);
        return patch;
    }
}
