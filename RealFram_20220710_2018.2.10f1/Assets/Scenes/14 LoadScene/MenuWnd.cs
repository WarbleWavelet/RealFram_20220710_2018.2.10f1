/****************************************************
    文件：MenuWnd.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/21 16:56:12
	功能：处理Panel上的组件代码
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;


namespace Demo14
{
    public class MenuWnd : Window
    {

        public MenuPanel m_MenuPanel;
        AudioSource m_audioSource;
        AudioClip m_audioClip;
        public override void OnShow(params object[] paralist)
        {


            m_MenuPanel = m_GameObject.AddComponent<MenuPanel>();
            AddButtonClickListener(m_MenuPanel.m_BtnStart, OnBtnStartClick);
            AddButtonClickListener(m_MenuPanel.m_BtnLoad, OnBtnLoadClick);
            AddButtonClickListener(m_MenuPanel.m_BtnExit, OnBtnExitClick);
            BindUI();
            //使用预加载的不可实例资源         
            //m_audioClip = ResourceMgr.Instance.LoadResource<AudioClip>(Constants_Demo14.MP3_SenLin);
            //Common.PlayBGMusic(m_audioSource, m_audioClip);

            m_MenuPanel.m_BtnStopAndUnload.onClick.AddListener(() => {
                m_audioSource.Stop();
                m_audioSource.clip = null;
                ResourceMgr.Instance.UnloadResItemByObject(m_audioClip, true);
                //
                m_audioClip = null;    //删引用
            });


           // Test_PreloadAndInstaniate();
          

            Test_AsyncLoadSprite();
        }



        #region 使用预加载的可实例资源
        void Test_PreloadAndInstaniate()
        {
            GameObject go = ObjectMgr.Instance.InstantiateObject(Constants_Demo14.Prefab_Attack, true); //
            ObjectMgr.Instance.UnloadGameObject(go);
            ObjectMgr.Instance.UnloadGameObject(go, 0, true);
        }
        #endregion
        #region 异步加载图片
        void Test_AsyncLoadSprite()
        {
            string path = "Assets/GameData/Images/";
            bool isSprite = true;
            bool setNativeSize=true;
            ResourceMgr.Instance.AsyncLoadObject ( path + "fgBlue.png", OnLoadSpriteFinished, AsyncLoadResPriority.High, isSprite,  m_MenuPanel.m_Image01_01 , setNativeSize);
            ResourceMgr.Instance.AsyncLoadObject(path + "fgGray.png", OnLoadSpriteFinished, AsyncLoadResPriority.Middle, isSprite, m_MenuPanel.m_Image01_02, setNativeSize);
            ResourceMgr.Instance.AsyncLoadObject(path + "fgRed.png", OnLoadSpriteFinished, AsyncLoadResPriority.Low, isSprite, m_MenuPanel.m_Image02_01, setNativeSize);
            ResourceMgr.Instance.AsyncLoadObject(path + "fgYellow.png", OnLoadSpriteFinished, AsyncLoadResPriority.High, isSprite, m_MenuPanel.m_Image02_02, setNativeSize);
            ResourceMgr.Instance.AsyncLoadObject(path + "fgGreen.png", OnLoadSpriteFinished, AsyncLoadResPriority.Middle, isSprite, m_MenuPanel.m_Image03_01, setNativeSize);
            ResourceMgr.Instance.AsyncLoadObject(path + "fgPurple.png", OnLoadSpriteFinished, AsyncLoadResPriority.Low, isSprite, m_MenuPanel.m_Image03_02, setNativeSize);

        }

        void OnLoadSpriteFinished(string path, UnityEngine.Object obj, object para1, object para2, object para3)
        {
            if (obj != null)
            {
                Sprite sprite = obj as Sprite;
                          
                Image image=para1 as Image;
                if (para1 != null)
                { 
                    image.sprite = sprite;
                }

                bool setNativeSize =(bool)para2 ;
                if (para2 != null && setNativeSize==true)
                {
                    image.SetNativeSize();
                }

                
            }
        }
        #endregion

        void BindUI()
        {
            m_audioSource = m_MenuPanel.m_AudioSource;
        }

        void OnBtnStartClick()
        {
            UnityEngine.Debug.Log(new StackTrace().GetFrame(0).GetMethod());//方法名

        }
        void OnBtnLoadClick()
        {
            UnityEngine.Debug.Log(new StackTrace().GetFrame(0).GetMethod());//方法名
        }
        void OnBtnExitClick()
        {
            UnityEngine.Debug.Log(new StackTrace().GetFrame(0).GetMethod());//方法名



        }
    }














}

