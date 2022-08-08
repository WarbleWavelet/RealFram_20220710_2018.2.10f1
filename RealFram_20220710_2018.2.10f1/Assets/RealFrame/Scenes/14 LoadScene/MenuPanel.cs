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
        public Image m_Image01_01;
        public Image m_Image01_02;
        public Image m_Image02_01;
        public Image m_Image02_02;
        public Image m_Image03_01;
        public Image m_Image03_02;



        void Awake()
        {
            BindUI();
        }

         void BindUI()
            {
            m_BtnStart  = transform.Find("bg/CenterPin/btnStart").GetComponent<Button>();
            m_BtnLoad   = transform.Find("bg/CenterPin/btnLoad").GetComponent<Button>();
            m_BtnExit   = transform.Find("bg/CenterPin/btnExit").GetComponent<Button>();
            m_Image01_01 = transform.Find("bg/CenterPin/img01_01").GetComponent<Image>();
            m_Image01_02 = transform.Find("bg/CenterPin/img01_02").GetComponent<Image>();
            m_Image02_01 = transform.Find("bg/CenterPin/img02_01").GetComponent<Image>();
            m_Image02_02 = transform.Find("bg/CenterPin/img02_02").GetComponent<Image>();
            m_Image03_01 = transform.Find("bg/CenterPin/img03_01").GetComponent<Image>();
            m_Image03_02 = transform.Find("bg/CenterPin/img03_02").GetComponent<Image>();


            m_BtnStopAndUnload = transform.Find("bg/CenterPin/btnStopAndUnload").GetComponent<Button>();
            m_AudioSource = transform.GetComponent<AudioSource>();
            }
        void Start()
        {

        }
                
    }

} 
