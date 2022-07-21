/****************************************************
    文件：AsyncReadResFromMgr.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/14 23:43:33
	功能：
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Demo13
{
	public class GameStart : MonoBehaviour
	{

	public	Transform m_RecyclePoolTrans;
	public	Transform m_SceneTrans;

		void Awake()
		{
			//GameObject.DontDestroyOnLoad(gameObject);
			bool log = false;
			AssetBundleMgr.Instance.LoadABCfg(log);
			ResourceMgr.Instance.InitCoroutine(this);
			ResourceMgr.Instance.LoadFromAB(false);

			ObjectMgr.Instance.InitMgr(m_RecyclePoolTrans, m_SceneTrans);
			UIMgr.Instance.Init(
				transform.Find("UIRoot") as RectTransform,
				transform.Find("UIRoot/WindowRoot") as RectTransform,
				transform.Find("UIRoot/UICamera").GetComponent<Camera>(),
				transform.Find("UIRoot/EventSystem").GetComponent<EventSystem>()
				);


			string _wndName = "MenuPanel.prefab";
			UIMgr.Instance.Register<MenuWnd>(_wndName);
			UIMgr.Instance.OpenWnd(_wndName);
		}







		private void OnApplicationQuit()
		{
			Debug.Log("OnApplicationQuit");

#if UNITY_EDITOR//这样写才对，不能括外面
			ResourceMgr.Instance.ClearAllResItem();
			Resources.UnloadUnusedAssets();
			Debug.Log("清存");
#endif
		}






	}

}

