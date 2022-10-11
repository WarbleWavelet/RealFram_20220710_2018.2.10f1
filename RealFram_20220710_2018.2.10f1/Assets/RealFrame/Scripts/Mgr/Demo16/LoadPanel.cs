/****************************************************
    文件：LoadPanel.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/22 13:48:29
	功能：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Demo16
{
    public class LoadPanel : MonoBehaviour
    {

        // public Slider m_SliderPrg;
        public Text m_TxtPrg;
        public Slider m_Slider;


        void Awake()
        {
            m_Slider = transform.Find("Slider").GetComponent<Slider>();
            m_TxtPrg = transform.Find("Slider/Text").GetComponent<Text>();
        }

        void Start()
        {

        }
    }

}
