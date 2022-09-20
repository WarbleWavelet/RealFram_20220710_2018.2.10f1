/****************************************************
    文件：HotFixUI.cs
	作者：lenovo
    邮箱: 
    日期：2022/9/16 19:13:5
	功能：
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Demo15
{

    public class HotfixWnd : Window
    {


        private HotfixPanel m_hotfixPanel;
        private float m_SumTime = 0;//下载总时间
        HotPatchMgr hotPatchMgr;
        GameStart gameStart;


        #region 生命

        public override void OnAwake(params object[] paralist)
        {
            m_SumTime = 0;
            m_hotfixPanel = m_GameObject.GetComponent<HotfixPanel>();
            hotPatchMgr = HotPatchMgr.Instance;
            gameStart = GameStart.Instance;
            m_hotfixPanel.m_ProgressPg.fillAmount = 0;
            m_hotfixPanel.m_ProgressText.text = string.Format("{0:F}M/S", 0);
            hotPatchMgr.ServerInfoError += ServerInfoError;
            hotPatchMgr.ItemError += ItemError;

#if false//UNITY_EDITOR
            StartOnFinish();
#else
        if (hotPatchMgr.ComputeUnPackFile())
        {
            m_hotfixPanel.m_ProgressText.text = "解压中...";
            hotPatchMgr.StartUnackFile(()=> 
            {
                m_SumTime = 0;
                HotFix();
            });
        }
        else
        {
            HotFix();
        }
#endif
        }


        public override void OnUpdate()
        {
            if (hotPatchMgr.StartUnPack)//解压
            {

                Debug.Log("解压");
                m_SumTime += Time.deltaTime;
                m_hotfixPanel.m_ProgressPg.fillAmount = hotPatchMgr.GetUnpackProgress();
                float speed = (hotPatchMgr.AlreadyUnPackSize / 1024.0f) / m_SumTime;
                m_hotfixPanel.m_ProgressText.text = string.Format("{0:F} M/S", speed);
            }

            if (hotPatchMgr.StartDownload) //下载
            {
                Debug.Log("下载");
                m_SumTime += Time.deltaTime;
                m_hotfixPanel.m_ProgressPg.fillAmount = hotPatchMgr.GetDownloadProgress();
                float speed = (hotPatchMgr.GetLoadSize() / 1024.0f) / m_SumTime;
                m_hotfixPanel.m_ProgressText.text = string.Format("{0:F} M/S", speed);
            }
        }

        public override void OnClose()
        {
            hotPatchMgr.ServerInfoError -= ServerInfoError;
            hotPatchMgr.ItemError -= ItemError;
            SceneMgr.Instance.LoadScene(DefinePath_Demo15.Scene_Menu);  //加载场景
        }



        #endregion  





        #region CheckVersion 
        void HotFix()
        {
            if (Common.NetworkStatus()==NetworkStatusType.NotReachable)
            {
                //提示网络错误，检测网络链接是否正常
                GameStart.OpenCommonConfirm("网络链接失败",
                    "网络链接失败，请检查网络链接是否正常？",
                    () => { Application.Quit(); },
                    () => { Application.Quit(); }
                );
            }
            else
            {
                CheckVersion();
            }
        }


        void CheckVersion()
        {
            hotPatchMgr.CheckVersion((hot) =>
            {
                if (hot)
                {
                    string des = string.Format("当前版本为{0},有{1:F}M大小热更包，是否确定下载？", hotPatchMgr.CurVersion, hotPatchMgr.LoadSumSize / 1024.0f);
                    //提示玩家是否确定热更下载
                    GameStart.OpenCommonConfirm("热更确定",
                        des,
                        OnClickStartDownLoad,
                        OnClickCancleDownLoad);
                }
                else
                {
                    StartOnFinish();
                }
            });
        }


      /// <summary>
      /// 点击开始下载
      /// </summary>
        void OnClickStartDownLoad()
        {
            if ( Common.IsAndroidOrIOS() )
            {
                if (Common.NetworkStatus()==NetworkStatusType.Traffic)
                {
                    GameStart.OpenCommonConfirm("下载确认", 
                        "当前使用的是手机流量，是否继续下载？", 
                        StartDownLoad, 
                        OnClickCancleDownLoad);
                }
            }
            else
            {
                StartDownLoad();
            }
        }

        //点击取消下载

        void OnClickCancleDownLoad()
        {
            Application.Quit();
        }

        /// <summary>
        /// 正式开始下载
        /// </summary>
        void StartDownLoad()
        {
            m_hotfixPanel.m_ProgressText.text = "下载中...";
            m_hotfixPanel.m_InfoPanel.SetActive(true);
            m_hotfixPanel.m_HotContentTex.text = hotPatchMgr.CurrentPatches.Des;
           gameStart.StartCoroutine(hotPatchMgr.StartDownLoadAB(StartOnFinish));
        }  



        /// <summary>
        /// 下载完成回调，或者没有下载的东西直接进入游戏
        /// </summary>
        void StartOnFinish()
        {
           gameStart.StartCoroutine(OnFinish());
        }

        /// <summary>
        /// 全部下载完成
        /// </summary>
        /// <returns></returns>
        IEnumerator OnFinish()
        {
            yield return gameStart.StartCoroutine(gameStart.StartGame(m_hotfixPanel.m_ProgressPg, m_hotfixPanel.m_ProgressText) );
            UIMgr.Instance.CloseWnd(this);
        }
        #endregion




        #region Error
        void ServerInfoError()
        {
            GameStart.OpenCommonConfirm("服务器列表获取失败", 
                "服务器列表获取失败，请检查网络链接是否正常？尝试重新下载！", 
                CheckVersion, 
                Application.Quit);
        }

        void ItemError(string all)
        {
            GameStart.OpenCommonConfirm("资源下载失败", 
                string.Format("{0}等资源下载失败，请重新尝试下载！", all),
                ANewDownload, 
                Application.Quit);
        }

        void ANewDownload()
        {
            hotPatchMgr.CheckVersion((hot) =>
            {
                if (hot)
                {
                    StartDownLoad();
                }
                else
                {
                    StartOnFinish();
                }
            });
        }
        #endregion



    }

}

