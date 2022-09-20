/****************************************************
    文件：CommonConfirm.cs
	作者：lenovo
    邮箱: 
    日期：2022/9/16 22:16:5
	功能：确定是否热更的弹出框
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonConfirm : BaseItem
{
    public Text m_Title;
    public Text m_Des;
    public Button m_ConfirmBtn;//确定
    public Button m_CancleBtn;//取消


    void Start()
    {
         BindUI();

    }
            

    public override void BindUI()
    {

        m_Title = transform.Find("TopPin/TitleTxt").GetComponent<Text>();
        m_Des = transform.Find("CenterPin/Scroll View/Viewport/Content/DesText").GetComponent<Text>();
        m_ConfirmBtn = transform.Find("BottomPin/okBtn").GetComponent<Button>();
        m_CancleBtn = transform.Find("BottomPin/cancelBtn").GetComponent<Button>();


    }



    public void Show(string title, 
        string str, 
        UnityEngine.Events.UnityAction confirmAction, 
        UnityEngine.Events.UnityAction cancleAction)
    {
        m_Title.text = title;
        m_Des.text = str;
        AddButtonClickListener(m_ConfirmBtn, () =>
        {
            confirmAction();
            Destroy(gameObject);
        });
        AddButtonClickListener(m_CancleBtn, () =>
        {
            cancleAction();
            Destroy(gameObject);
        });
    }
}
