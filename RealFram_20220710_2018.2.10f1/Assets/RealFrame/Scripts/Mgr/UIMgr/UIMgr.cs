/****************************************************
    文件：UIMgr.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/21 14:44:38
	功能：UIMgr
        区别于《暗黑破坏神》，UIMgr对Wnd的持有是拖拽的。采用了Lst和Dic
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMgr : Singleton<UIMgr> {


    #region 字属
    //UI节点
    public RectTransform m_UiRoot;
    //窗口节点
    private RectTransform m_wndRoot;
    //UI摄像机
    private Camera m_uiCamera;
    //EventSystem节点
    private EventSystem m_eventSystem;
    //屏幕的宽高比
    private float m_canvasRate = 0;

    private string m_uiPrefabPath = DefinePath.Cfg_UIPrefabPath;
    //注册的字典
    private Dictionary<string, System.Type> m_registerDic = new Dictionary<string, System.Type>();


    //所有打开的窗口
    public Dictionary<string, Window> m_wndDic = new Dictionary<string, Window>();
    //打开的窗口列表
    private List<Window> m_wndLst = new List<Window>();
    #endregion

    public void SetUIPrefabPath(string path)
    {
        m_uiPrefabPath = path;
    }


    #region 生命
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="uiRoot">UI父节点</param>
    /// <param name="wndRoot">窗口父节点</param>
    /// <param name="uiCamera">UI摄像机</param>
    public void InitMgr(RectTransform uiRoot, RectTransform wndRoot, Camera uiCamera, EventSystem eventSystem)
    {
        m_UiRoot = uiRoot;
        m_wndRoot = wndRoot;
        m_uiCamera = uiCamera;
        m_eventSystem = eventSystem;
        m_canvasRate = Screen.height / (m_uiCamera.orthographicSize * 2);
    }

    /// <summary>
    /// 窗口的更新
    /// </summary>
    public void OnUpdate()
    {
        if (m_wndLst == null || m_wndLst.Count <= 0)
        {
            return;
        }
        for (int i = 0; i < m_wndLst.Count; i++)
        {
            if (m_wndLst[i] != null)
            {
                m_wndLst[i].OnUpdate();
            }
        }
    }
    #endregion




    /// <summary>
    /// 显示或者隐藏所有Wnd
    /// </summary>
    public void ShowOrHideUI(bool show)
    {
        if (m_UiRoot != null)
        {
            m_UiRoot.gameObject.SetActive(show);
        }
    }

    /// <summary>
    /// 设置默认选择对象
    /// </summary>
    /// <param name="obj"></param>
    public void SetDefaultSelectObj(GameObject obj)
    {
        if (m_eventSystem == null)
        {
            m_eventSystem = EventSystem.current;
        }
        m_eventSystem.firstSelectedGameObject = obj;
    }



    /// <summary>
    /// 窗口注册方法
    /// </summary>
    /// <typeparam name="T">窗口泛型类</typeparam>
    /// <param name="name">窗口名</param>
   public  void Register<T>(string prefabName) where T : Window
    {
        m_registerDic[prefabName] = typeof(T);
    }

    /// <summary>
    /// 发送消息给窗口
    /// </summary>
    /// <param name="name">窗口名</param>
    /// <param name="msgID">消息ID</param>
    /// <param name="paralist">参数数组</param>
    /// <returns></returns>
    public bool SendMsgToWnd(string name, UIMsgID msgID = 0, params object[] paralist)
    {
        Window wnd = GetWnd<Window>(name);
        if (wnd != null)
        {
            return wnd.OnReceivevMessage(msgID, paralist);
        }
        return false;
    }



    #region Wnd
 /// <summary>
    /// 根据窗口名查找窗口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public T GetWnd<T>(string name) where T : Window
    {
        Window wnd = null;
        if ( m_wndDic.TryGetValue(name, out wnd))
        {
            return (T)wnd;
        }

        return null;
    }

    /// <summary>
    /// 打开窗口
    /// </summary>
    /// <param name="wndName"></param>
    /// <param name="isTop"></param>
    /// <param name="para1"></param>
    /// <param name="para2"></param>
    /// <param name="para3"></param>
    /// <returns></returns>
    public Window OpenWnd(string panelFullPath,  bool isTop = true, params object[] paralist)
    {
        string wndName = Common.TrimName(panelFullPath, TrimNameType.SlashAfter);
        Window wnd = GetWnd<Window>(wndName);
        if ( null == wnd )
        {
            System.Type type = null;
            if ( m_registerDic.TryGetValue(wndName, out type) ==true )
            {
                wnd = System.Activator.CreateInstance(type) as Window;
            }
            else
            {
                Debug.LogError("找不到窗口对应的脚本，窗口名是：" + wndName);
                return null;
            }

            GameObject go = ObjectMgr.Instance.InstantiateObject(panelFullPath , false, false);
            if ( null == go)
            {
                Debug.Log("创建窗口Prefab失败：" + wndName);
                return null;
            }

            if (m_wndDic.ContainsKey(wndName)==false)
            {
                m_wndLst.Add(wnd);
                m_wndDic.Add(wndName, wnd);
            }

            wnd.m_GameObject = go;
            wnd.m_Transform = go.transform;
            wnd.m_Name = wndName;
            wnd.OnAwake(paralist);
            go.transform.SetParent(m_wndRoot, false);

            if (isTop)
            {
                SetWndTop(go);
            }

            wnd.OnShow(paralist);
        }
        else
        {
            ShowWnd(wndName, isTop, paralist);
        }

        return wnd;
    }


    /// <summary>
    /// 最上面
    /// </summary>
    /// <param name="go"></param>
    void SetWndTop( GameObject go)
    {
        go.transform.SetAsLastSibling();
    }


    /// <summary>
    /// 全部关闭，只打开唯一窗口
    /// </summary>
    public void OpenOnlyOneWnd(string name, bool isTop = true, params object[] paralist)
    {
        name = Common.TrimName(name, TrimNameType.SlashAfter);
        CloseAllWnd();
        OpenWnd(name, isTop, paralist);
    }
    /// <summary>
    /// 根据窗口名关闭窗口
    /// </summary>
    /// <param name="name"></param>
    /// <param name="destory"></param>
    public void CloseWnd(string name, bool destory = false)
    {
        name = Common.TrimName(name, TrimNameType.SlashAfter);
        Window wnd = GetWnd<Window>(name);
        CloseWnd(wnd, destory);
    }

    /// <summary>
    /// 根据窗口对象关闭窗口
    /// </summary>
    /// <param name="window"></param>
    /// <param name="destory"></param>
    public void CloseWnd(Window window, bool destory = false)
    {
        if (window != null)
        {
            window.Disable();
            window.Close();
            if (m_wndDic.ContainsKey(window.m_Name))
            {
                m_wndDic.Remove(window.m_Name);
                m_wndLst.Remove(window);
            }

            if (destory)
            {
                ObjectMgr.Instance.UnloadGameObject(window.m_GameObject, 0, true);
            }
            else
            {
                ObjectMgr.Instance.UnloadGameObject(window.m_GameObject, recycleParent: false);
            }
            window.m_GameObject = null;
            window = null;
        }
    }


    
    /// <summary>
    /// 关闭所有窗口
    /// </summary>
    public void CloseAllWnd()
    {
        for (int i = m_wndLst.Count - 1; i >= 0; i--)
        {
            CloseWnd(m_wndLst[i]);
        }
    }



    /// <summary>
    /// 根据名字隐藏窗口
    /// </summary>
    /// <param name="name"></param>
    public void HideWnd(string name)
    {
        name = Common.TrimName(name, TrimNameType.SlashAfter);
        Window wnd = GetWnd<Window>(name);
        HideWnd(wnd);
    }

    /// <summary>
    /// 根据窗口对象隐藏窗口
    /// </summary>
    /// <param name="wnd"></param>

    public void HideWnd(Window wnd)
    {
        if (wnd != null)
        {
            wnd.m_GameObject.SetActive(false);
            wnd.Disable();
        }
    }

    /// <summary>
    /// 根据窗口名字显示窗口
    /// </summary>
    /// <param name="name"></param>
    /// <param name="paralist"></param>
    public void ShowWnd(string name, bool bTop = true, params object[] paralist)
    {
        name = Common.TrimName(name, TrimNameType.SlashAfter);
        Window wnd = GetWnd<Window>(name);
        ShowWnd(wnd, bTop, paralist);
    }

    /// <summary>
    /// 根据窗口对象显示窗口
    /// </summary>
    /// <param name="wnd"></param>
    /// <param name="paralist"></param>
    public void ShowWnd(Window wnd, bool bTop = true, params object[] paralist)
    {
        if (wnd != null)
        {
            if (wnd.m_GameObject != null && !wnd.m_GameObject.activeSelf) wnd.m_GameObject.SetActive(true);
            if (bTop) wnd.m_Transform.SetAsLastSibling();
            wnd.OnShow(paralist);
        }
    }
    #endregion


    #region Slider

    #endregion

}
public enum UIMsgID
{
    None = 0,
}