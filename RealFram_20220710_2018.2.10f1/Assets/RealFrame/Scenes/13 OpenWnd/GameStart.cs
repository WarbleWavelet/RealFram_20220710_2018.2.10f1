/****************************************************
    文件：AsyncReadResFromMgr.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/14 23:43:33
	功能：挂载在场景中的GanmeStart
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

		public Transform m_RecyclePoolTrans;
		public Transform m_SceneTrans;
		public Toggle toggle;
		public Button btn3;

		void Awake()
		{
			GameObject.DontDestroyOnLoad(gameObject);
			//
			InitMgr();
			//
			toggle.onValueChanged.AddListener((bool state) => {
				toggle.GetComponentInChildren<Text>().text = state == true ? "LoadFromAB" : "LoadFromEditor";
				ResourceMgr.Instance.SetLoadFromAB(state);

				toggle.interactable = false;
			});
			btn3.onClick.AddListener(() => {
				UIMgr.Instance.Register<MenuWnd>(Common.TrimName(DefinePath_Demo13.MenuPanel, TrimNameType.SlashAfter));//xx/xx/a.prefab +> a.prefab
				UIMgr.Instance.OpenWnd(DefinePath_Demo13.MenuPanel);
				btn3.interactable = false;
			});
		}




		void InitMgr()
		{
			AssetBundleMgr.Instance.InitMgr(false);
			ResourceMgr.Instance.InitMgr(this);
			ObjectMgr.Instance.InitMgr(m_RecyclePoolTrans, m_SceneTrans);
			//
			UIMgr.Instance.InitMgr(
				transform.Find("UIRoot") as RectTransform,
				transform.Find("UIRoot/WindowRoot") as RectTransform,
				transform.Find("UIRoot/UICamera").GetComponent<Camera>(),
				transform.Find("UIRoot/EventSystem").GetComponent<EventSystem>()
				);
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

