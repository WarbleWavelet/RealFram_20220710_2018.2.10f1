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
        public string m_CurSceneName;//当前要进入的场景 ,相当于tarScene

        public override void OnAwake(object param1 = null, object param2 = null, object param3 = null)
        {
            object[] paralist = new object[3]; //之前用paralist做参数，现在不改了
            paralist[0] = param1;
            paralist[1] = param2;
            paralist[2] = param3;
            m_LoadPanel= m_GameObject.AddComponent<LoadPanel>();
            
            if (paralist != null && paralist.Length > 0)
            {                                      
               // m_CurSceneName = (string)param1; //我这样写报错了
                m_CurSceneName = "Menu14"; 
            }
        }



        public override void OnUpdate()
        {
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
        
             if (m_CurSceneName == DefinePath_Demo14.Scene_Menu)
             {
                UIMgr.Instance.Wnd_Open(
                    uiPrefabPath: DefinePath_Demo14.Prefab_MenuPanel,
                    resources:true,//resource直接走Editor 
                    isTop:true,
                    paralist:m_CurSceneName);
               //UIMgr.Instance.OpenWnd(uiPrefab, true, sceneName);

             } 
            
            UIMgr.Instance.Wnd_Close(DefinePath_Demo14.Prefab_LoadPanel,destory:true);//加载完成 
        }
    }














}

