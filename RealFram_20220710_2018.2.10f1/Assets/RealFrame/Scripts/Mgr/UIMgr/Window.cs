/****************************************************
    文件：Window.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/21 14:45:11
	功能：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Window
{

    //引用GameObject
    public GameObject m_GameObject { get; set; }

    //引用Transform
    public Transform m_Transform { get; set; }

    //名字
    public string m_Name { get; set; }

    //所有的Button

    protected List<Button> m_btnLst = new List<Button>();
    protected List<Slider> m_sliderLst = new List<Slider>();
    protected List<Image> m_imgLst = new List<Image>();

    //所有Toggle
    protected List<Toggle> m_toggleLst = new List<Toggle>();









    #region 生命
    public virtual void OnAwake(params object[] paralist) { }

    /// <summary>窗口显示</summary>
    public virtual void OnShow(params object[] paralist) { }
    /// <summary>隐藏</summary>
    public virtual void Disable() { }

    public virtual void OnUpdate() { }
    /// <summary>关闭</summary> 
    public virtual void Close()
    {
        RemoveAllButtonListener();
        RemoveAllToggleListener();
        m_btnLst.Clear();
        m_imgLst.Clear();
        m_sliderLst.Clear();
    }
    #endregion

    /// <summary>
    /// 收到UIMsg
    /// </summary>
    /// <param name="msgID"></param>
    /// <param name="paralist"></param>
    /// <returns></returns>
    public virtual bool OnReceivevMessage(UIMsgID msgID, params object[] paralist)
    {
        return true;
    }



    #region Sprite



    public void ChangeImageFillAmount(Image imgFg, Text txtPrg,Image imgHandler, float prg)
    {
        if (imgFg == null)
            return ;


        if (m_imgLst.Contains(imgFg)==false)
        {
            m_imgLst.Add(imgFg);

        }
            imgFg.fillAmount = prg;
            txtPrg.text = prg.ToString("0.00");
            imgHandler.GetComponent<RectTransform>().localPosition += new Vector3((1600f * prg), 0f, 0f);
        

        return;
    }
 /// <summary>
    /// 同步替换图片
    /// </summary>
    /// <param name="path"></param>
    /// <param name="image"></param>
    /// <param name="setNativeSize"></param>
    /// <returns></returns>
    public bool ChangeImageSprite(string path, Image image, bool setNativeSize = false)
    {
        if (image == null)
            return false;

        Sprite sp = ResourceMgr.Instance.LoadResource<Sprite>(path);
        if (sp != null)
        {
            if (image.sprite != null)
                image.sprite = null;

            image.sprite = sp;
            if (setNativeSize)
            {
                image.SetNativeSize();
            }
            return true;
        }

        return false;
    }

    /// <summary>
    /// 异步替换图片
    /// </summary>
    /// <param name="path"></param>
    /// <param name="image"></param>
    /// <param name="setNativeSize"></param>
    public void AsyncChangImageSprite(string path, Image image, bool setNativeSize = false)
    {
        if (image == null)
            return;

        ResourceMgr.Instance.AsyncLoadObject(path, OnLoadSpriteFinish, AsyncLoadResPriority.Middle, true, image, setNativeSize);
    }

    /// <summary>
    /// 图片加载完成
    /// </summary>
    /// <param name="path"></param>
    /// <param name="obj"></param>
    /// <param name="param1"></param>
    /// <param name="param2"></param>
    /// <param name="param3"></param>
    void OnLoadSpriteFinish(string path, Object obj, object param1 = null, object param2 = null, object param3 = null)
    {
        if (obj != null)
        {
            Sprite sp = obj as Sprite;
            Image image = param1 as Image;
            bool setNativeSize = (bool)param2;
            if (image.sprite != null)
                image.sprite = null;

            image.sprite = sp;
            if (setNativeSize)
            {
                image.SetNativeSize();
            }
        }
    }
    #endregion


    #region Button
    /// <summary>
    /// 移除所有的button事件
    /// </summary>
    public void RemoveAllButtonListener()
    {
        foreach (Button btn in m_btnLst)
        {
            btn.onClick.RemoveAllListeners();
        }
    }



    /// <summary>
    /// 添加button事件监听
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="action"></param>
    public void AddButtonClickListener(Button btn, UnityEngine.Events.UnityAction action)
    {
        if (btn != null)
        {
            if (!m_btnLst.Contains(btn))
            {
                m_btnLst.Add(btn);
            }
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(action);
            btn.onClick.AddListener(BtnPlaySound);
        }
    }

    /// <summary>
    /// 播放button声音
    /// </summary>
    void BtnPlaySound()
    {

    }
    #endregion


    #region Toggle
    /// <summary>
    /// 移除所有的toggle事件
    /// </summary>
    public void RemoveAllToggleListener()
    {
        foreach (Toggle toggle in m_toggleLst)
        {
            toggle.onValueChanged.RemoveAllListeners();
        }
    }

    /// <summary>
    /// Toggle事件监听
    /// </summary>
    /// <param name="toggle"></param>
    /// <param name="action"></param>
    public void AddToggleClickListener(Toggle toggle, UnityEngine.Events.UnityAction<bool> action)
    {
        if (toggle != null)
        {
            if (!m_toggleLst.Contains(toggle))
            {
                m_toggleLst.Add(toggle);
            }
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(action);
            toggle.onValueChanged.AddListener(TogglePlaySound);
        }
    }

    /// <summary>
    /// 播放toggle声音
    /// </summary>
    /// <param name="isOn"></param>
    void TogglePlaySound(bool isOn)
    {

    }
    #endregion



}
