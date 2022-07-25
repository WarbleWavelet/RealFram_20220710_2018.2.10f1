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
    public class LoadWnd: Window 
    {

        public LoadPanel m_LoadPanel;
        public string m_CurSceneName;

        public override void OnAwake(params object[] paralist)
        {

            m_LoadPanel= m_GameObject.GetComponent<LoadPanel>();
          
            if (paralist != null && paralist.Length > 0)
            {
                m_CurSceneName = (string)paralist[0];

            }
        }



        public override void OnUpdate()
        {
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
            if (m_CurSceneName == Constants_Demo14.Scene_Menu)
            {
                UIMgr.Instance.OpenWnd(Constants_Demo14.Prefab_MenuPanel);
               //UIMgr.Instance.OpenWnd(uiPrefab, true, sceneName);

            } 
            
            UIMgr.Instance.CloseWnd(Constants_Demo14.Prefab_LoadPanel);//加载完成
    }
    }














}

