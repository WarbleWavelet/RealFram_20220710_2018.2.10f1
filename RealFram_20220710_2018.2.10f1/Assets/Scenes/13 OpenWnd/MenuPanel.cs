/****************************************************
    文件：MenuWnd.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/21 16:56:12
	功能：挂载到预制体 Assets/GameData/Prefabs/UGUI/Panel/MenuPanel.prefab
         （父节点是WindowRoot）
          被 PanelWnd:Wnd引用,作为组件输入端
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Demo13
{
    public class MenuPanel: MonoBehaviour 
    {
        public Button m_BtnStart;
        public Button m_BtnLoad;
        public Button m_BtnExit;


        void Awake()
        {
            m_BtnStart= transform.Find("bg/Center/btnStart").GetComponent<Button>();
            m_BtnLoad= transform.Find("bg/Center/btnLoad").GetComponent<Button>();
            m_BtnExit= transform.Find("bg/Center/btnExit").GetComponent<Button>();    
        }
        void Start()
        {

        }

                
    }

} 
