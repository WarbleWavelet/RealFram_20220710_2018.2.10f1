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
    public class MenuWnd: Window
    {

        public MenuPanel m_MenuPanel;
        AudioSource m_audioSource; 
        AudioClip m_audioClip; 
        public override void OnShow(params object[] paralist)
        {


            m_MenuPanel= m_GameObject.GetComponent<MenuPanel>();
            AddButtonClickListener(m_MenuPanel.m_BtnStart, OnBtnStartClick);
            AddButtonClickListener(m_MenuPanel.m_BtnLoad, OnBtnLoadClick);
            AddButtonClickListener(m_MenuPanel.m_BtnExit, OnBtnExitClick);


            m_audioSource = m_MenuPanel.m_AudioSource;
            m_audioClip = ResourceMgr.Instance.LoadResource<AudioClip>(Constants_Demo14.MP3_SenLin);
            Common.PlayBGMusic(m_audioSource, m_audioClip);

            m_MenuPanel.m_BtnStopAndUnload.onClick.AddListener(() => {
                m_audioSource.Stop();
                m_audioSource.clip = null;
                ResourceMgr.Instance.UnloadResItemByObject(m_audioClip, true);
                //
                m_audioClip = null;    //删引用
            });


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

