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


namespace Demo14
{
    public class MenuPanel: MonoBehaviour 
    {
        public Button m_BtnStart;
        public Button m_BtnLoad;
        public Button m_BtnExit;
        public Button m_BtnStopAndUnload;
        public AudioSource m_AudioSource;


        void Awake()
        {
            m_BtnStart = transform.Find("bg/Center/btnExit").GetComponent<Button>();
            m_BtnLoad = transform.Find("bg/Center/m_BtnLoad").GetComponent<Button>();
            m_BtnExit = transform.Find("bg/Center/m_BtnExit").GetComponent<Button>();
            m_BtnStopAndUnload = transform.Find("bg/Center/m_BtnStopAndUnload").GetComponent<Button>();
            m_AudioSource = transform.GetComponent<AudioSource>();
        }
        void Start()
        {

        }
                
    }

} 
