/****************************************************
    文件：HotFixUI.cs
	作者：lenovo
    邮箱: 
    日期：2022/9/16 19:13:5
	功能：相当于Demo14的LoadiingWnd和MenuWnd的集合帮
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Demo15
{

    public class HotfixWnd : Window
    {


        private HotfixPanel m_hotfixPanel;
        private float m_SumTime = 0;    //下载或解压总时间，看你测试什么
        public string m_LocalPath_Origin = DefinePath.LocalPath_Origin;

        public string m_Prefab_Attack = DefinePath_Demo15.Prefab_Attack;

        #region 生命

        public override void OnAwake(object param1 = null, object param2 = null, object param3 = null)//UIMgr中被调用
        {
            m_SumTime = 0;
            m_hotfixPanel = m_GameObject.GetComponent<HotfixPanel>();
            m_hotfixPanel.m_ProgressPg.fillAmount = 0;
            m_hotfixPanel.m_ProgressText.text = string.Format("{0:F}M/S", 0);
            HotPatchMgr.Instance.ServerInfoError += ServerInfoError;
            HotPatchMgr.Instance.ItemError += ItemError;

            Test_Awake();

        }





        public override void OnShow(object param1 = null, object param2 = null, object param3 = null)
        {
            base.OnShow( param1,  param2 ,  param3 );
           
        }

        public override void OnUpdate()
        {

             Test_Update();
        }



        public override void OnClose()
        {
            HotPatchMgr.Instance.ServerInfoError -= ServerInfoError;
            HotPatchMgr.Instance.ItemError -= ItemError;
            SceneMgr.Instance.LoadScene(loadingUI: DefinePath_Demo15.Prefab_HotfixPanel ,tarScene: DefinePath_Demo15.Scene_Menu);  //加载场景
        }


        /// <summary>
        /// 点击开始下载
        /// </summary>
        void OnClickStartDownLoad()
        {
            if (Common.IsAndroidOrIOS())
            {
                if (Common.NetworkStatus() == NetworkStatusType.Traffic)
                {
                    GameStart.OpenCommonConfirm("下载确认",
                        "当前使用的是手机流量，是否继续下载？",
                        Hotfix_Start,
                        OnClickCancleDownLoad);
                }
            }
            else
            {
                Hotfix_Start();
            }
        }


        #endregion



        #region Test

        /// <summary>
        /// 测试的，要放在Awake
        /// </summary>
        void Test_Awake()
        {
            //Test_Hotfix();
            Test_Unpack();
        }

        /// <summary>
        /// 测试的，要放在Awake
        /// </summary>
        void Test_Update()
        {
            if (HotPatchMgr.Instance.Unpack_Start)//解压
            {
                m_SumTime += Time.deltaTime;
                m_hotfixPanel.m_ProgressPg.fillAmount = HotPatchMgr.Instance.Unpack_Prg();
                float speed = (HotPatchMgr.Instance.Unpack_DoneSize / 1024.0f) / m_SumTime;
                m_hotfixPanel.m_ProgressText.text = string.Format("{0:F} M/S", speed);
            }

            if (HotPatchMgr.Instance.Download_Start) //下载
            {
                m_SumTime += Time.deltaTime;
                m_hotfixPanel.m_ProgressPg.fillAmount = HotPatchMgr.Instance.Download_Prg();
                float speed = (HotPatchMgr.Instance.Download_DoneSize() / 1024.0f) / m_SumTime;
                m_hotfixPanel.m_ProgressText.text = string.Format("{0:F} M/S", speed);
            }
        }


        /// <summary>
        /// 直接热更
        /// </summary>
        void Test_Hotfix()
        {
            HotFix();
        }


        void Test_Unpack()
        {

#if true//UNITY_EDITOR
            Hotfix_Update();
#else
            if (HotPatchMgr.Instance.UnpackFile_Compute( m_LocalPath_Origin ))
            {
                m_hotfixPanel.m_ProgressText.text = "解压中...";
                HotPatchMgr.Instance.UnpackFile_Start(() =>
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
            HotPatchMgr.Instance.CheckVersion((hot) =>
            {
                if (hot)
                {
                    string des = string.Format("当前版本为{0},有{1:F}M大小热更包，是否确定下载？", HotPatchMgr.Instance.CurVersion, HotPatchMgr.Instance.LoadSumSize / 1024.0f);
                    //提示玩家是否确定热更下载
                    GameStart.OpenCommonConfirm("热更确定",
                        des,
                        OnClickStartDownLoad,
                        OnClickCancleDownLoad);
                }
                else
                {
                    Hotfix_Update();
                }
            });
        }




        //点击取消下载

        void OnClickCancleDownLoad()
        {
            Application.Quit();
        }




        #endregion



        #region 下载


        /// <summary>
        /// 正式开始下载
        /// </summary>
        void Hotfix_Start()
        {
            m_hotfixPanel.m_ProgressText.text = "下载中...";
            m_hotfixPanel.m_InfoPanel.SetActive(true);
            m_hotfixPanel.m_HotContentText.text = HotPatchMgr.Instance.CurrentPatches.Des;
           GameStart.Instance.StartCoroutine(HotPatchMgr.Instance.StartDownLoadAB(Hotfix_Update));
        }  



        /// <summary>
        /// 下载完成回调，或者没有下载的东西直接进入游戏
        /// </summary>
        void Hotfix_Update()
        {
           GameStart.Instance.StartCoroutine(Hotfix_Finish());
         
        }

        /// <summary>
        /// 全部下载完成, 的回调
        /// </summary>
        /// <returns></returns>
        IEnumerator Hotfix_Finish()
        {
            yield return GameStart.Instance.StartCoroutine(GameStart.Instance.StartGame(m_hotfixPanel.m_ProgressPg, m_hotfixPanel.m_ProgressText) );
            UIMgr.Instance.Wnd_Close(this); 
            //
            //Test_AsyncLoadSprite();
            Test_Instaniate();
        }
        #endregion



        #region Error
        void ServerInfoError()
        {
            GameStart.OpenCommonConfirm(
                "服务器列表获取失败", 
                "服务器列表获取失败，请检查网络链接是否正常？尝试重新下载！", 
                CheckVersion, 
                Application.Quit
            );
        }

        void ItemError(string all)
        {
            GameStart.OpenCommonConfirm(
                "资源下载失败", 
                string.Format("{0}等资源下载失败，请重新尝试下载！", all),
                ANewDownload_CheckVersion, 
                Application.Quit
            );
        }

        void ANewDownload_CheckVersion()
        {
            HotPatchMgr.Instance.CheckVersion((hot) =>
            {
                if (hot)
                {
                    Hotfix_Start();
                }
                else
                {
                    Hotfix_Update();
                }
            });
        }
        #endregion

        #region 异步加载图片
        void Test_AsyncLoadSprite()
        {
           // string path = DefinePath.Demo14_Images_PathPreFixed;
           //string path = @"C:/Users/lenovo/AppData/LocalLow/DefaultCompany/RealFrame_Test/DownLoad/image/";
            string path = "Assets/RealFrame/GameData/Images/fgRed.png";
            bool isSprite = true;
            bool setNativeSize = true;
            ResourceMgr.Instance.AsyncLoadObject(
                path ,
                OnLoadSpriteFinished, 
                AsyncLoadResPriority.Low, isSprite, 
                m_hotfixPanel.m_Image, setNativeSize
            );

        }

        void OnLoadSpriteFinished(string path, UnityEngine.Object obj, object para1, object para2, object para3)
        {
            if (obj != null)
            {
                Sprite sprite = obj as Sprite;

                Image image = para1 as Image;
                if (para1 != null)
                {
                    image.sprite = sprite;
                }

                bool setNativeSize = (bool)para2;
                if (para2 != null && setNativeSize == true)
                {
                    image.SetNativeSize();
                }


            }
        }
        #endregion

        #region 使用预加载的可实例资源
        void Test_Instaniate()
        {
            GameObject go = ObjectMgr.Instance.InstantiateObject( m_Prefab_Attack, true); //
            GameObject go1 = GameObject.Instantiate(go);
            //ObjectMgr.Instance.UnloadGameObject(go);
            //ObjectMgr.Instance.UnloadGameObject(go, 0, true);
        }
        #endregion


    }

}
