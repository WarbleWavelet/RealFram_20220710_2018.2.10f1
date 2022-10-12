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
    public RectTransform m_wndRoot;
    //UI摄像机
    private Camera m_uiCamera;
    //EventSystem节点
    private EventSystem m_eventSystem;
    //屏幕的宽高比
    private float m_canvasRate = 0;

    private string m_uiPrefabPath = DefinePath.Cfg_UIPrefabPath;
    /// <summary><Panel,Wnd>注册的字典</summary> 
    private Dictionary<string, System.Type> m_registerDic = new Dictionary<string, System.Type>();


    //所有打开的窗口
    public Dictionary<string, Window> m_wndDic = new Dictionary<string, Window>();
    //打开的窗口列表
    private List<Window> m_wndLst = new List<Window>();
    #endregion



    public static string m_HotFix_NamespaceClass = DefinePath.m_HotFix_NamespaceClass;


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
            Window wnd = m_wndLst[i];
            if (wnd != null)
            {
                if (wnd.IsHotFix)
                {
                    AppDomain_Invoke_Window(wnd, Window.m_OnUpdate,null);
                }
                else
                {
                    wnd.OnUpdate();
                }
                
            }
        }
    }
    #endregion




    /// <summary>
    /// 显示或者隐藏所有Wnd
    /// </summary>
    public void Wnd_ShowAll(bool show)
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
    /// <param name="name">窗口名xxx.prefab</param>
   public  void Register<T>(string prefabName) where T : Window
    {
        m_registerDic[prefabName] = typeof(T); //xxui.prefab,xxWnd
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
        Window wnd = Wnd_Get<Window>(name);
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
    public T Wnd_Get<T>(string name) where T : Window
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
    public Window Wnd_Open(
        string uiPrefabPath, 
        bool resources=false,
        bool isTop = true,
        params object[] paralist
        )
    {
        string uiName= Common.TrimName(uiPrefabPath, TrimNameType.SlashAfter);//  /xxx.prefab
        Window wnd = Wnd_Get<Window>(uiName);
        if (null == wnd)
        {
            System.Type wndName = null;
            if (m_registerDic.TryGetValue(uiName, out wndName) == true) 
            {
                if ( resources )//ources) //编辑器加载
                {
                    wnd = System.Activator.CreateInstance(wndName) as Window;      
                }
                else //ILR热更  Demo16xxxPanel.prefab => xxxWnd
                {
                    string hotwndName = uiName.Replace("Panel.prefab", "Wnd").Substring(6); //写死了Demo16, length=6
                    
                    try
                    {
                         wnd = ILRuntimeMgr.Instance.ILRunAppDomain.Instantiate<Window>(m_HotFix_NamespaceClass + "." +hotwndName);
                    }
                    catch (System.Exception e)
                    {
                         Debug.LogErrorFormat("ILR未能生成实例：{0}", hotwndName);
                    }
                  

                    wnd.IsHotFix = true;
                    wnd.HotFix_NamespaceClassName = uiName;
                }
            }
            else
            {
                Debug.LogError("找不到窗口对应的脚本，窗口名是：" + uiName);
                return null;
            }


            GameObject go = null;
            if (resources)
            {
                string panelNameNoFix = Common.TrimName(uiName, TrimNameType.PointPre);
                go = GameObject.Instantiate(Resources.Load<GameObject>(panelNameNoFix)); 
            }
            else
            {
               go = ObjectMgr.Instance.InstantiateObject(uiPrefabPath, false, false);             
            }

          
            if (null == go)
            {
                Debug.Log("创建窗口Prefab失败：" + uiName);
                return null;
            }

            if (m_wndDic.ContainsKey(uiName) == false)
            {
                m_wndLst.Add(wnd);
                m_wndDic.Add(uiName, wnd);
            }

            wnd.m_GameObject = go;
            wnd.m_Transform = go.transform;
            wnd.m_Name = uiName;
            //
            if (wnd.IsHotFix)
            {
                //目前paralist无法作为参数传递
                AppDomain_Invoke_Window(wnd,Window.m_OnAwake,wnd,paralist);
            }
            else
            { 
                 wnd.OnAwake(paralist);
            }
          
            //
            wnd.Resources = resources;
            go.transform.SetParent(m_wndRoot, false);

            if (isTop)
            {
                Wnd_SetTop(go);
            }

            if (wnd.IsHotFix)
            {
                //目前paralist无法作为参数传递
                AppDomain_Invoke_Window(wnd, Window.m_OnShow, wnd, paralist);
            }
            else
            {
                wnd.OnShow(paralist);
            }
        }
        else
        {
            Wnd_Show(uiName, isTop, paralist);
        }

        return wnd;
    }


    public Window Wnd_Open(
   string uiPrefabPath,
   bool resources = false,
   bool isTop = true,
    object para1=null,
    object para2=null,
    object para3=null
   )
    {
        string uiName = Common.TrimName(uiPrefabPath, TrimNameType.SlashAfter);//  /xxx.prefab
        Window wnd = Wnd_Get<Window>(uiName);
        if (null == wnd)
        {
            System.Type wndName = null;
            if (m_registerDic.TryGetValue(uiName, out wndName) == true)
            {
                if (resources)//ources) //编辑器加载
                {
                    wnd = System.Activator.CreateInstance(wndName) as Window;
                }
                else //ILR热更  Demo16xxxPanel.prefab => xxxWnd
                {
                    string hotwndName = m_HotFix_NamespaceClass + "." +uiName.Replace("Panel.prefab", "Wnd").Substring(6); //写死了Demo16, length=6

                    try
                    {
                        wnd = ILRuntimeMgr.Instance.ILRunAppDomain.Instantiate<Window>( hotwndName);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogErrorFormat("ILR未能生成实例：{0}", hotwndName);
                    }


                    wnd.IsHotFix = true;
                    wnd.HotFix_NamespaceClassName = hotwndName;
                }
            }
            else
            {
                Debug.LogError("找不到窗口对应的脚本，窗口名是：" + uiName);
                return null;
            }


            GameObject go = null;
            if (resources)
            {
                string panelNameNoFix = Common.TrimName(uiName, TrimNameType.PointPre);
                go = GameObject.Instantiate(Resources.Load<GameObject>(panelNameNoFix));
            }
            else
            {
                go = ObjectMgr.Instance.InstantiateObject(uiPrefabPath, false, false);
            }


            if (null == go)
            {
                Debug.Log("创建窗口Prefab失败：" + uiName);
                return null;
            }

            if (m_wndDic.ContainsKey(uiName) == false)
            {
                m_wndLst.Add(wnd);
                m_wndDic.Add(uiName, wnd);
            }

            wnd.m_GameObject = go;
            wnd.m_Transform = go.transform;
            wnd.m_Name = uiName;
            //
            if (wnd.IsHotFix)
            {
                //目前paralist无法作为参数传递
                AppDomain_Invoke_Window(wnd.HotFix_NamespaceClassName, Window.m_OnAwake,wnd, para1,para2,para3);
            }
            else
            {
                wnd.OnAwake( para1, para2, para3);
            }

            //
            wnd.Resources = resources;
            go.transform.SetParent(m_wndRoot, false);

            if (isTop)
            {
                Wnd_SetTop(go);
            }

            if (wnd.IsHotFix)
            {
                //目前paralist无法作为参数传递
                AppDomain_Invoke_Window(wnd.HotFix_NamespaceClassName, Window.m_OnShow, wnd, para1, para2, para3);
            }
            else
            {
                wnd.OnShow(param1:para1,param2: para2,param3: para3);
            }
        }
        else
        {
            if (wnd.IsHotFix)
            {
                //目前paralist无法作为参数传递
                AppDomain_Invoke_Window(wnd, Window.m_OnShow, wnd, para1, para2, para3);
            }
            else
            {
                Wnd_Show(name:uiName,bTop: isTop,para1: para1, para2: para2, para3: para3);
            }
        }

        return wnd;
    }

    /// <summary>
    /// 最上面
    /// </summary>
    /// <param name="go"></param>
    void Wnd_SetTop( GameObject go)
    {
        go.transform.SetAsLastSibling();
    }





    /// <summary>
    /// 全部关闭，只打开唯一窗口
    /// </summary>
    public void Wnd_OpenOnlyOne(string name,bool resources = false, bool isTop = true, params object[] paralist)
    {
        name = Common.TrimName(name, TrimNameType.SlashAfter);
        Wnd_CloseAll();
        Wnd_Open(name, isTop, resources,   paralist);
    }
    /// <summary>
    /// 根据窗口名关闭窗口
    /// </summary>
    /// <param name="name"></param>
    /// <param name="destory"></param>
    public void Wnd_Close(string name, bool destory = false)
    {
        name = Common.TrimName(name, TrimNameType.SlashAfter);
        Window wnd = Wnd_Get<Window>(name);
        Wnd_Close(wnd, destory);
    }

    /// <summary>
    /// 根据窗口对象关闭窗口
    /// </summary>
    /// <param name="wnd"></param>
    /// <param name="destory"></param>
    public void Wnd_Close(Window wnd, bool destory = false)
    {
        if (wnd != null)
        {
            if (wnd.IsHotFix)
            {
                AppDomain_Invoke_Window(wnd, Window.m_OnDisable, null);
                AppDomain_Invoke_Window(wnd, Window.m_OnAwake, null);
            }
            else
            { 
                wnd.OnDisable();
                wnd.OnClose();            
            }

            if (m_wndDic.ContainsKey(wnd.m_Name))
            {
                m_wndDic.Remove(wnd.m_Name);
                m_wndLst.Remove(wnd);
            }


            if (wnd.Resources == false)
            {
                if (destory)
                {
                    ObjectMgr.Instance.UnloadGameObject(wnd.m_GameObject, 0, true);
                }
                else
                {
                    ObjectMgr.Instance.UnloadGameObject(wnd.m_GameObject, recycleParent: false);
                }
            }
            else
            {
                GameObject.Destroy(wnd.m_GameObject);
            }
            wnd.m_GameObject = null;
            wnd = null;
        }
    }




    /// <summary>
    /// 关闭所有窗口
    /// </summary>

    public void Wnd_CloseAll()
    {
        for (int i = m_wndLst.Count - 1; i >= 0; i--)
        {
            Wnd_Close(m_wndLst[i]);
        }
    }



    /// <summary>
    /// 根据名字隐藏窗口
    /// </summary>
    /// <param name="name"></param>
    public void Wnd_Hide(string name)
    {
        name = Common.TrimName(name, TrimNameType.SlashAfter);
        Window wnd = Wnd_Get<Window>(name);
        Wnd_Hide(wnd);
    }

    /// <summary>
    /// 根据窗口对象隐藏窗口
    /// </summary>
    /// <param name="wnd"></param>

    public void Wnd_Hide(Window wnd)
    {
        if (wnd != null)
        {
            wnd.m_GameObject.SetActive(false);
            wnd.OnDisable();
        }
    }

    public void Wnd_Show(string name, bool bTop = true,  object para1=null,object para2=null,object para3=null)
    {
        object[] paralist = new object[3] { para1,para2,para3};
        name = Common.TrimName(name, TrimNameType.SlashAfter);
        Window wnd = Wnd_Get<Window>(name);
        Wnd_Show(wnd, bTop, paralist);
    }

    /// <summary>
    /// 根据窗口名字显示窗口
    /// </summary>
    /// <param name="name"></param>
    /// <param name="paralist"></param>
    public void Wnd_Show(string name, bool bTop = true, params object[] paralist)
    {
        name = Common.TrimName(name, TrimNameType.SlashAfter);
        Window wnd = Wnd_Get<Window>(name);
        Wnd_Show(wnd, bTop, paralist);
    }

    /// <summary>
    /// 根据窗口对象显示窗口
    /// </summary>
    /// <param name="wnd"></param>
    /// <param name="paralist"></param>
    public void Wnd_Show(Window wnd, bool bTop = true, params object[] paralist)
    {
        if (wnd != null)
        {
            if (wnd.m_GameObject != null && !wnd.m_GameObject.activeSelf) wnd.m_GameObject.SetActive(true);
            if (bTop) wnd.m_Transform.SetAsLastSibling();

            if (wnd.IsHotFix)
            {
                ILRuntimeMgr.Instance.ILRunAppDomain.Invoke(wnd.HotFix_NamespaceClassName, Window.m_OnShow, wnd, paralist);
            }
            else
            { 
                wnd.OnShow(paralist);
            }
            
        }
    }
    #endregion


    #region ILR热更
    /// <summary>
    /// Window,IsHotFix==true时，调用自身方法的方式
    /// </summary>
    /// <param name="wnd"></param>
    /// <param name="method"></param>
    /// <param name="paralist"></param>
    static void AppDomain_Invoke_Window(Window wnd,string method,  params object[] paralist)
    {
        string namespaceClass = wnd.HotFix_NamespaceClassName;
        try
        {
             ILRuntimeMgr.Instance.ILRunAppDomain.Invoke(namespaceClass, method, wnd,  paralist);
            //ILRuntimeMgr.Instance.ILRunAppDomain.Invoke(namespaceClass, method, wnd, paralist[0], paralist[1], paralist[2]);
            //AppDomain_Invoke_Window(namespaceClass, method, wnd, paralist[0], paralist[1], paralist[2]);
        }
        catch (System.Exception)
        {
            throw new System.Exception("AppDomain_Invoke异常");
        }
       
    }

    static void AppDomain_Invoke_Window(string namespaceClass ,string method,Window wnd,object para1, object para2, object para3)
    {
        try
        {
            ILRuntimeMgr.Instance.ILRunAppDomain.Invoke(namespaceClass, method, wnd,  para1,  para2,  para3);
        }
        catch (System.Exception)
        {
            throw new System.Exception("AppDomain_Invoke异常");
        }

    }
    #endregion

}
public enum UIMsgID
{
    None = 0,
}