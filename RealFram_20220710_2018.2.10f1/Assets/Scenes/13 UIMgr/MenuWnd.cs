/****************************************************
    文件：MenuWnd.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/21 16:56:12
	功能：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;


namespace Demo13
{
    public class MenuWnd: Window 
    {

        public MenuPanel m_MenuPanel;
        public override void Start(params object[] paralist)
        {
            m_MenuPanel= m_GameObject.GetComponent<MenuPanel>();
            AddButtonClickListener(m_MenuPanel.m_BtnStart, OnBtnStartClick);
            AddButtonClickListener(m_MenuPanel.m_BtnLoad, OnBtnLoadClick);
            AddButtonClickListener(m_MenuPanel.m_BtnExit, OnBtnExitClick);
        }


        void OnBtnStartClick()
        {
            UnityEngine.Debug.Log(new StackTrace().GetFrame(0).GetMethod());//方法名
        }        void OnBtnLoadClick()
        {
            UnityEngine.Debug.Log(new StackTrace().GetFrame(0).GetMethod());//方法名
        }        void OnBtnExitClick()
        {
            UnityEngine.Debug.Log(new StackTrace().GetFrame(0).GetMethod());//方法名
        }
    }

} 
