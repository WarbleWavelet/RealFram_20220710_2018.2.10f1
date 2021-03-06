/****************************************************
    文件：SceneMgr.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/22 14:15:26
	功能：(也就是SceneMap)，场景跳转
*****************************************************/

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : Singleton<SceneMgr>
{

    #region 字属
    /// <summary>加载前的回调</summary> 
    public Action OnEnter;
    /// <summary>加载完的回调</summary>
    public Action OnExit;

    public string m_CurSceneName { get; set; }
    public int m_CurPrg;
    private MonoBehaviour m_mono;

    /// <summary>加载完成？</summary> 
    private bool m_isDone { get; set; }

    #endregion


    #region 生命
    public void InitMgr(MonoBehaviour mono)
    {
        m_mono = mono;
    }

    /// <summary>
    /// loadPanelFullPath跑到100%，就显示tarSceneName
    /// </summary>
    /// <param name="loadPanelFullPath"></param>
    /// <param name="tarSceneName"></param>
    public void LoadScene(string  loadPanelFullPath, string tarSceneName)
    {
        if ( null == SceneManager.GetSceneByName(tarSceneName) 
          || null == tarSceneName)
        {
            Debug.LogFormat("场景{0}不存在", tarSceneName==null ?"NULL":tarSceneName);
            return;
        }
        m_CurPrg = 0;
        m_mono.StartCoroutine(  LoadSceneAsync(tarSceneName)  );
        UIMgr.Instance.OpenWnd(loadPanelFullPath, true, tarSceneName); //loadPanel跑完了，去sceneName
    }

    void SetSceneSettings(string sceneName)
    {

    }

    public void ClearCache()
    {
        ObjectMgr.Instance.ClearCache();
        ResourceMgr.Instance.ClearAllResItem();
    }

    #endregion



    IEnumerator LoadSceneAsync(string tarSceneName)
    {
        if (OnEnter != null)
        {
            OnEnter();
        }
        ClearCache();
        m_isDone = false;
        AsyncOperation unloadScene = SceneManager.LoadSceneAsync(Constants_Demo14.Scene_Empty, LoadSceneMode.Single);//为内存安全起见，加载一个空的.要卸载的场景
        while (unloadScene != null && unloadScene.isDone == false)//需要时间
        {
            yield return new WaitForEndOfFrame(); //等一帧 
        }



        m_CurPrg = 0;
        int tarPrg = 0;
        AsyncOperation tarScene = SceneManager.LoadSceneAsync(tarSceneName);//加载目标场景
        if (tarScene != null && tarScene.isDone == false)
        {
            tarScene.allowSceneActivation = false;//先不显示
            while (tarScene.progress < 0.9f)//90%以前直接等于
            {
                tarPrg = (int)(tarScene.progress * 100);
                yield return new WaitForEndOfFrame();

                while (m_CurPrg < tarPrg)//平滑过渡
                {
                    ++m_CurPrg;
                    yield return new WaitForEndOfFrame();
                }
            }
        }



        m_CurSceneName = tarSceneName;
        SetSceneSettings(tarSceneName);
        tarPrg = 100;
        while (tarPrg - m_CurPrg > 2)//自行加载剩余的10%,加载到99%
        {
            ++m_CurPrg;
            yield return new WaitForEndOfFrame();
        }
        m_CurPrg = 100;
        tarScene.allowSceneActivation = true;//显示出来
        m_isDone = true;
        if (OnExit != null)
        {
            OnExit();
        }
    }
}
