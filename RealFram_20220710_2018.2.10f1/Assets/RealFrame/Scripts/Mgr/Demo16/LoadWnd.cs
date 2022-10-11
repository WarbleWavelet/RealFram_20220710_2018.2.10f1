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


namespace Demo16
{
    public class LoadWnd: Window 
    {

        public LoadPanel m_LoadPanel;
        public string m_CurSceneName;

        public static bool m_resources=false;

        public override void OnAwake(object param1 = null, object param2 = null, object param3 = null)
        {

            UnityEngine.Debug.LogFormat("本地_LoadWnd.OnAwake()"  );
            m_LoadPanel= m_GameObject.AddComponent<LoadPanel>();
            m_CurSceneName = "Menu16";
            //object[] paralist = new object[3] { param1 ,  param2 ,  param3 };          
            //if (paralist != null && paralist.Length > 0)
            //{
            //    if (paralist[0] is string)
            //    { 
            //        m_CurSceneName = (string)paralist[0] ;
            //    }
            //}
        }



        public override void OnUpdate()
        {
            UnityEngine.Debug.LogFormat("本地_LoadWnd.OnUpdate()");
            //
            if (m_LoadPanel == null)
            { 
                return;
            }

            float prg = SceneMgr.Instance.m_CurPrg / 100.0f;
            m_LoadPanel.m_TxtPrg.text = prg.ToString("0.00%");
            m_LoadPanel.m_Slider.value =prg;
            
            if (SceneMgr.Instance.m_CurPrg >= 100)
            {
                SwitchScene();
            }
        }

        public void SwitchScene()
        {
            if (m_CurSceneName == DefinePath_Demo16.Scene_Menu)
            {
                UIMgr.Instance.Wnd_Open(DefinePath_Demo16.Prefab_MenuPanel,resources:m_resources); //想
            } 
            
            UIMgr.Instance.Wnd_Close(DefinePath_Demo16.Prefab_LoadPanel);//加载完成
    }
    }














}

