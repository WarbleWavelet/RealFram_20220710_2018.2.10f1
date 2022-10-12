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

    private string m_VersionTextName = "Version";   //解析出下面2个
    private string m_CurVersion="";                 //当前版本号
    private string m_CurPackName="";                //当前包名
    string m_ABMD5_Name_Inner = "ABMD5";

    //
    private MonoBehaviour m_Mono;                                                               //下载的线程
    //private string m_ServerInfoXml_Server = "http://127.0.0.1/ServerInfo.xml";                //热更配置表 =="http://127.0.0.1:80/ServerInfo.xml
    private string m_ServerInfoXml_Server = "http://localhost:8081/ServerInfo.xml";
    private string m_LocalPath_SerevrInfoXml = Application.persistentDataPath + "/ServerInfo.xml";  //本地路径
    private string m_LocalPath_LocalInfoXml = Application.persistentDataPath + "/LocalInfo.xml";    //类似D盘，大版本安装不删除
    private string m_LocalPath_DownLoad = Application.persistentDataPath + "/DownLoad";         //m_DownLoadPath = "C:/Users/lenovo/AppData/LocalLow/DefaultCompany/RealFrame_Test/DownLoad"

    private ServerInfo m_ServerInfo_Server=new ServerInfo();
    private ServerInfo m_ServerInfo_Local= new ServerInfo();

    //
    private Dictionary<string, Patch> m_PatchDic = new Dictionary<string, Patch>();    //所有热更的东西
    private List<Patch> m_DownLoadList = new List<Patch>();                             //所有需要下载的东西
    private Dictionary<string, Patch> m_DownLoadDic = new Dictionary<string, Patch>();  //所有需要下载的东西的Dic
    private Patches m_CurrentPatches;                                                   //当前热更Patches
    public Action ServerInfoError;                                                      //服务器列表获取错误回调
    public Action<string> ItemError;                                                    //文件下载出错回调
    public Action LoadOver;                                                             //下载完成回调 
    public List<Patch> m_AlreadyDownList = new List<Patch>();                           //储存已经下载的资源  
    public bool Download_Start = false;                                                  //是否开始下载
    private int m_TryDownCount = 0;                                                     //尝试重新下载次数
    private const int DOWNLOADCOUNT = 4;                                                //重复下载几次还失败就报错
    private DownLoadAssetBundle m_CurDownload = null;                                   //当前正在下载的资源
     //
    //
    private VersionInfo m_GameVersion;
                                                                                                           
    private Dictionary<string, string> m_DownLoadMD5Dic = new Dictionary<string, string>(); //服务器上的资源名对应的MD5，用于下载后MD5校验
     //
    public int LoadFileCount { get; set; } = 0;                                                         // 需要下载的资源总个数
    public float LoadSumSize { get; set; } = 0;                                                         // 需要下载资源的总大小 KB
     //


    private string m_AB_InnerPath=DefinePath.OutputAB_InnerPath + Common.BuildTarget ;



    public Patches CurrentPatches
    {
        get { return m_CurrentPatches; }
    }
    public string CurVersion
    {
        get { return m_CurVersion; }
    }


    #region 解压


    private string m_LocalPath_Origin = Application.persistentDataPath + "/Origin";                     //解压路径
    private Dictionary<string, ABMD5Base> m_primaryPackedMd5Dic = new Dictionary<string, ABMD5Base>();  //原包记录的MD5码,最初的
    private List<string> m_UnpackLst = new List<string>();                                              //需要解压的文件

    public bool Unpack_Start = false;                                                                    //是否开始解压
    public float UnpackSumSize { get; set; } = 0;                                                       //解压文件总大小
    public float Unpack_DoneSize { get; set; } = 0;                                                      //已解压大小
    #endregion  


    #endregion




    #region 生命


    public void InitMgr(MonoBehaviour mono)
    {
        m_Mono = mono;
        TextAsset md5 = Resources.Load<TextAsset>(m_ABMD5_Name_Inner);
        MD5_Read(md5, out m_primaryPackedMd5Dic);
    }


    /// <summary>
    /// 检测版本是否热更
    /// 01 读取本地版本
    /// 02 下载服务器Xml
    /// </summary>
    /// <param name="hotCallBack"></param>
    public void CheckVersion(Action<bool> hotCallBack = null)
    {
        m_TryDownCount = 0; //重新下载的次数
        m_PatchDic.Clear();
        //
        ReadVersion_ByResources(Resources.Load<TextAsset>(m_VersionTextName), out m_CurVersion, out m_CurPackName);
        m_Mono.StartCoroutine(
            ReadXml(
                m_ServerInfoXml_Server,
                30,
                m_LocalPath_SerevrInfoXml,
                m_ServerInfo_Local,
                () =>
                {
                    if (m_ServerInfo_Local == null)
                    {
                        if (ServerInfoError != null)  //服务器表检查出错
                        {
                            ServerInfoError();
                        }
                        return;
                    }

                    foreach (VersionInfo version in m_ServerInfo_Local.GameVersion)
                    {
                        if (version.Version == m_CurVersion)
                        {
                            m_GameVersion = version;
                            break;
                        }
                    }

                     m_PatchDic=GetHotAB(m_GameVersion);

                    if (CheckLocalAndServerPatch( m_LocalPath_LocalInfoXml,  m_ServerInfo_Local,  m_CurVersion,  m_GameVersion))
                    {
                        ComputeDownloadInfo(m_GameVersion, out m_CurrentPatches, m_LocalPath_DownLoad, m_DownLoadList, m_DownLoadDic, m_DownLoadMD5Dic);//不out会null
                        if (Common.File_Exits(m_LocalPath_SerevrInfoXml))
                        {                                                                       
                            Common.File_Delete(m_LocalPath_LocalInfoXml);//删本地
                            Common.File_Move(m_LocalPath_SerevrInfoXml, m_LocalPath_LocalInfoXml); //拿服务器的
                        }
                    }
                    else //完全一致
                    {
                        ComputeDownloadInfo(m_GameVersion, out  m_CurrentPatches, m_LocalPath_DownLoad, m_DownLoadList, m_DownLoadDic, m_DownLoadMD5Dic);
                    }
                    LoadFileCount = m_DownLoadList.Count;
                    LoadSumSize = m_DownLoadList.Sum(x => x.Size);
                    if (hotCallBack != null)
                    {
                        hotCallBack(m_DownLoadList.Count > 0);
                    }
                })
            );
    }







    /// <summary>
    /// 获取所有热更包信息
    /// </summary>
    Dictionary<string, Patch> GetHotAB(VersionInfo gameVersion)
    {
        Dictionary<string, Patch> patchDic = new Dictionary<string, Patch>();
        if (NeedHotfix(m_GameVersion))
        {
            Patches lastPatches = gameVersion.Pathces[m_GameVersion.Pathces.Length - 1];  //基于最开始的版本
            if (lastPatches != null && lastPatches.Files != null)
            {
                foreach (Patch patch in lastPatches.Files)
                {
                    patchDic.Add(patch.Name, patch);
                }
            }
        }
        return patchDic;
    }


    bool NeedHotfix(VersionInfo m_GameVersion)
    {
        return m_GameVersion != null          //版本
                 && m_GameVersion.Pathces != null     //热更包
                 && m_GameVersion.Pathces.Length > 0;

    }
    #endregion




    #region 解压


    /// <summary>
    /// 读取本地资源MD5码(在编辑器内：D:\Data\Projects\Unity\Ocean_RealFram_20220710_2018.2.10f1\RealFram_20220710_2018.2.10f1\Assets\RealFrame\Resources)
    /// </summary>
    void MD5_Read(TextAsset md5,out Dictionary<string , ABMD5Base> m_PackedMd5Dic)
    {
        m_PackedMd5Dic = new Dictionary<string, ABMD5Base>();
        if (md5 == null)
        {
            Debug.LogError("未读取到本地MD5");
            return;
        }




        using (MemoryStream ms = new MemoryStream(md5.bytes))
        {
            BinaryFormatter bf = new BinaryFormatter();
            ABMD5 abmd5 = bf.Deserialize(ms) as ABMD5;
            foreach (ABMD5Base abmd5Base in abmd5.ABMD5Lst)
            {
                m_PackedMd5Dic.Add(abmd5Base.Name, abmd5Base);
            }
        }
    }

    /// <summary>
    /// 计算需要解压的文件（也就是AES加密的）
    /// </summary>
    /// <returns></returns>
    public bool UnpackFile_Compute(string path)
    {
#if true// UNITY_ANDROID//先测试一遍


        Common.Folder_New(path);
        m_UnpackLst.Clear();

        foreach (string fileName in m_primaryPackedMd5Dic.Keys)
        {
            string filePath = path + "/" + fileName;
            if (Common.File_Exits(filePath))
            {
                string md5 = MD5Mgr.Instance.BuildFileMD5(filePath);
                if (m_primaryPackedMd5Dic[fileName].MD5 != md5)//防止有人改动文件
                {
                    m_UnpackLst.Add(fileName);
                }
            }
            else
            {
                m_UnpackLst.Add(fileName);
            }
        }

        foreach (string fileName in m_UnpackLst)
        {
            if (m_primaryPackedMd5Dic.ContainsKey(fileName))
            {
                UnpackSumSize += m_primaryPackedMd5Dic[fileName].Size;
            }
        }

        return m_UnpackLst.Count > 0;
#else
        return false;
#endif
    }

    /// <summary>
    /// 获取解压进度
    /// </summary>
    /// <returns></returns>
    public float Unpack_Prg()
    {
        return Unpack_DoneSize / UnpackSumSize;
    }

    /// <summary>
    /// 开始解压文件
    /// </summary>
    /// <param name="callBack"></param>
    public void UnpackFile_Start(Action callBack)
    {
        Unpack_Start = true;
        m_Mono.StartCoroutine(Unpack_2PersistentDataPath(m_AB_InnerPath, m_LocalPath_Origin,30,callBack));
    }

    /// <summary>
    /// 将包里的原始资源解压到本地
    /// </summary>
    /// <param name="callBack"></param>
    /// <returns></returns>
    IEnumerator Unpack_2PersistentDataPath(string downloadPath,string unpackPath,int timeout,Action callBack)
    {
        foreach (string fileName in m_UnpackLst)
        {
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(downloadPath +"/"+ fileName);//(用内部路径模拟服务器)
            unityWebRequest.timeout = timeout;
            yield return unityWebRequest.SendWebRequest();


            if (unityWebRequest.isNetworkError)
            {
                Debug.Log("UnPack Error" + unityWebRequest.error);
            }
            else
            {
                byte[] bytes = unityWebRequest.downloadHandler.data;
                Common.File_Create_Write(unpackPath + "/" + fileName, bytes);
            }

            if (m_primaryPackedMd5Dic.ContainsKey(fileName))
            {
                Unpack_DoneSize += m_primaryPackedMd5Dic[fileName].Size;
            }

            unityWebRequest.Dispose();
        }

        if (callBack != null)
        {
            callBack();
        }

        Unpack_Start = false;
    }
    #endregion





    #region 辅助

  /// <summary>
    /// 计算AB包路径,本地路径是否存在C:\Users\lenovo\AppData\LocalLow\DefaultCompany\RealFrame_Test\DownLoad
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string Exist_assetbundleconfig_Hotfix(string name)
    {
        Patch patch = null;
        m_PatchDic.TryGetValue(name, out patch);
        if (patch != null)
        {
            //return m_LocalPath_DownLoad + "/" + name;
            return m_LocalPath_Origin + "/" + name;
        }
        return "";
    }




    /// <summary>
    /// 检查本地热更信息与服务器热更信息比较 (ServerInfo.xml)
    /// </summary>
    /// <returns></returns>
    bool CheckLocalAndServerPatch(string m_LocalInfoXml_Local,ServerInfo m_ServerInfo_Local,string m_CurVersion,VersionInfo m_GameVersion)
    {
        if (Common.File_Exits(m_LocalInfoXml_Local) == false)//首次热更
        { 
            return true;
        }
        

        m_ServerInfo_Local =FormatTool.Xml2Class(m_LocalInfoXml_Local, typeof(ServerInfo)) as ServerInfo;

        VersionInfo localGameVesion = null;
        if (m_ServerInfo_Local != null)
        {
            foreach (VersionInfo version in m_ServerInfo_Local.GameVersion)
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
            && NeedHotfix(m_GameVersion)
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

    #endregion
  




    #region 下载


    /// <summary>
    /// 计算下载的资源信息不out会null
    /// </summary>
    void ComputeDownloadInfo(
        VersionInfo m_GameVersion,
        out Patches m_CurrentPatches,
        string m_DownLoadPath_Local, 
        List<Patch> m_DownLoadList, 
        Dictionary<string, Patch> m_DownLoadDic, 
        Dictionary<string, string> m_DownLoadMD5Dic
        )
    {
        m_DownLoadList.Clear();
        m_DownLoadDic.Clear();
        m_DownLoadMD5Dic.Clear();
        m_CurrentPatches = null;
        if (NeedHotfix(m_GameVersion))
        {
            m_CurrentPatches = m_GameVersion.Pathces[m_GameVersion.Pathces.Length - 1];
            if (m_CurrentPatches.Files != null && m_CurrentPatches.Files.Count > 0)
            {
                foreach (Patch patch in m_CurrentPatches.Files)
                {
                    if (   IsPlatform( patch, RuntimePlatform.WindowsPlayer,Common.BuildTarget) )
                    {
                        AddDownLoadList(patch, m_DownLoadPath_Local, m_DownLoadList, m_DownLoadDic, m_DownLoadMD5Dic);
                    }
                    else if (IsPlatform(patch, RuntimePlatform.Android, Common.BuildTarget))
                    {
                        AddDownLoadList(patch, m_DownLoadPath_Local, m_DownLoadList, m_DownLoadDic, m_DownLoadMD5Dic);
                    }
                    else if (IsPlatform(patch, RuntimePlatform.IPhonePlayer, Common.BuildTarget))
                    {
                        AddDownLoadList(patch, m_DownLoadPath_Local, m_DownLoadList, m_DownLoadDic, m_DownLoadMD5Dic);
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
    void AddDownLoadList(Patch patch,string m_DownLoadPath_Local, List<Patch> m_DownLoadList, Dictionary<string, Patch> m_DownLoadDic, Dictionary<string, string> m_DownLoadMD5Dic)
    {
        //patch.Url = patch.Url.Replace("http://127.0.0.1","http://localhost:8081");
        string filePath = m_DownLoadPath_Local + "/" + patch.Name;
     
        if (Common.File_Exits(filePath))    //存在这个文件时进行对比看是否与服务器MD5码一致，不一致放到下载队列，如果不存在直接放入下载队列
        {
            string md5 = MD5Mgr.Instance.BuildFileMD5(filePath);
            if (patch.Md5 != md5)
            {
                AddDownLoad(patch, m_DownLoadList, m_DownLoadDic, m_DownLoadMD5Dic);
            }
        }
        else
        {
            AddDownLoad(patch,m_DownLoadList, m_DownLoadDic,m_DownLoadMD5Dic);
        }
    }

     void AddDownLoad(Patch patch,List<Patch> m_DownLoadList,Dictionary<string,Patch> m_DownLoadDic,Dictionary<string,string> m_DownLoadMD5Dic)
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
    public float Download_Prg()
    {
        return Download_DoneSize() / LoadSumSize;
    }

    /// <summary>
    /// 获取已经下载总大小
    /// </summary>
    /// <returns></returns>
    public float Download_DoneSize()
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
        Download_Start = true;
        if (patchLst == null)
        {
            patchLst = m_DownLoadList;
        }
        Common.Folder_New(m_LocalPath_DownLoad);


        List<DownLoadAssetBundle> downLoadAssetBundleLst = new List<DownLoadAssetBundle>();
        foreach (Patch patch in patchLst)
        {
            downLoadAssetBundleLst.Add(new DownLoadAssetBundle(patch.Url, m_LocalPath_DownLoad));
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
        MD5_Verify(downLoadAssetBundleLst, callBack);
    }

    /// <summary>
    /// Md5码校验,如果校验没通过，自动重新下载没通过的文件，重复下载计数，达到一定次数后，反馈某某文件下载失败
    /// </summary>
    /// <param name="downLoadAssetLst"></param>
    /// <param name="downloadCallback"></param> 
    void MD5_Verify(List<DownLoadAssetBundle> downLoadAssetLst, Action downloadCallback)
    {
        List<Patch> downLoadLst = new List<Patch>();
        foreach (DownLoadAssetBundle downLoad in downLoadAssetLst)
        {
            string oldMD5 = "";
            if (m_DownLoadMD5Dic.TryGetValue(downLoad.FileName, out oldMD5))
            {
                string newMD5 = MD5Mgr.Instance.BuildFileMD5(downLoad.SaveFilePath);
                if (newMD5 != oldMD5)//不一样
                {
                    Debug.LogErrorFormat(string.Format("{0}此文件MD5校验失败，即将重新下载", downLoad.FileName));
                    Patch patch = FindPatchByGamePath(downLoad.FileName);
                    if (patch != null)
                    {
                        downLoadLst.Add(patch); //放下载列表
                    }
                }
            }
        }

        if (downLoadLst.Count <= 0)//都正确
        {
            m_DownLoadMD5Dic.Clear();
            if (downloadCallback != null)
            {
                Download_Start = false;//下载结束
                downloadCallback();
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
                Download_Start = false;
                foreach (Patch patch in downLoadLst)
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
                foreach (Patch patch in downLoadLst)
                {
                    m_DownLoadMD5Dic.Add(patch.Name, patch.Md5);
                }
                m_Mono.StartCoroutine(  StartDownLoadAB(downloadCallback, downLoadLst)  ); //自动重新下载校验失败的文件
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


    #region 辅助
    /// <summary>
    /// 读取打包时的版本
    /// </summary>
    void ReadVersion_ByResources(TextAsset versionTextAsset, out string curVersion, out string curPackName)
    {
        curVersion = "";
        curPackName = "";
        if (versionTextAsset == null)
        {
            Debug.LogError("未读到本地版本！");
            return;
        }
        string[] all = versionTextAsset.text.Split('\n');// "Version|0.1;PackageName|com.Company.ProductName;"
        if (all.Length > 0)
        {
            string[] infoList = all[0].Split(';'); //  "Version|0.1   PackageName|com.Company.ProductName"
            if (infoList.Length >= 2)//读两行，版本号，包名
            {
                curVersion = infoList[0].Split('|')[1];   //0.1
                curPackName = infoList[1].Split('|')[1];  // com.Company.ProductName
            }
        }

    }


    /// <summary>
    /// 将Client下载到Local，然后解析为serverInfo
    /// </summary>
    /// <param name="serverPath_ServerInfoXml"></param>
    /// <param name="localPath_ServerInfoXml"></param>
    /// <param name="serverInfo"></param>
    /// <param name="callBack"></param>
    /// <returns></returns>
    IEnumerator ReadXml(string serverPath_ServerInfoXml,int timeout, string localPath_ServerInfoXml, ServerInfo serverInfo, Action callBack)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(serverPath_ServerInfoXml);//m_ServerInfoXml_Client , "http://127.0.0.1/ServerInfo.xml" （默认是80可以省略不写，但是改成其它端口比如8081后，必须加上去）
        webRequest.timeout = timeout;  //超时时间30s
        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError)//超时错误  ,一般是没开启服务器（Ocean用的是Apache）
        {
            Debug.Log("Download Error（没开启服务器？）：" + webRequest.error);
        }
        else if (webRequest.isHttpError)
        {
            Debug.Log("404 NotFound" + webRequest.error);
        }
        else//下载到本地，写文件
        {
            Common.File_Create_Write(localPath_ServerInfoXml, webRequest.downloadHandler.data); //m_ServerInfoXml_Local = "C:/Users/lenovo/AppData/LocalLow/DefaultCompany/RealFrame/ServerInfo.xml"

            if (Common.File_Exits(localPath_ServerInfoXml))
            {
                serverInfo = FormatTool.Xml2Class(localPath_ServerInfoXml, typeof(ServerInfo)) as ServerInfo;
                m_ServerInfo_Local = serverInfo;
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


    #endregion
}
