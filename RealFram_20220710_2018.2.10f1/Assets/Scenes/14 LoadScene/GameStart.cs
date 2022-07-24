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
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Demo14
{
	public class GameStart : MonoBehaviour
	{

		public	Transform m_RecyclePoolTrans;
		public	Transform m_SceneTrans;

		public Button btn3;
		public Toggle toggle;

		#region 生命
		void Awake()
		{

         



            GameObject.DontDestroyOnLoad(gameObject);

			InitMgr();
			RegisterUI();	

			//
			toggle.onValueChanged.AddListener((bool state) => {
				toggle.GetComponentInChildren<Text>().text = state == true ? "LoadFromAB" : "LoadFromEditor";
				ResourceMgr.Instance.SetLoadFromAB(state);

				toggle.interactable = false;
			});




			btn3.onClick.AddListener(() => { 

				//UIMgr.Instance.OpenWnd(Constants_Demo14.Prefab_Menu);
				//UIMgr.Instance.OpenWnd(Constants_Demo14.Prefab_Load,true, Constants_Demo14.Scene_Menu);//tarSceneName=Constants_Demo14.Scene_Empty
				//SceneMgr.Instance.LoadScene(Constants_Demo14.Scene_Menu);//会让Unity卡死
				//SceneMgr.Instance.LoadScene(Constants_Demo14.Scene_Menu);
				//SceneMgr.Instance.LoadScene(Constants_Demo14.LoadPanel, Constants_Demo14.Scene_Menu );
				SceneMgr.Instance.LoadScene(Constants_Demo14.LoadPanel, Constants_Demo14.Scene_Menu); 
				btn3.interactable = false;
			});
																							  

		}





        void Update()
        {
			UIMgr.Instance.OnUpdate();

        }
        #endregion




        #region 辅助
        void InitMgr()
		{
			AssetBundleMgr.Instance.InitMgr(false);
			ResourceMgr.Instance.InitMgr(this);

			ObjectMgr.Instance.InitMgr(m_RecyclePoolTrans, m_SceneTrans);
			UIMgr.Instance.InitMgr(
				transform.Find("UIRoot") as RectTransform,
				transform.Find("UIRoot/WindowRoot") as RectTransform,
				transform.Find("UIRoot/UICamera").GetComponent<Camera>(),
				transform.Find("UIRoot/EventSystem").GetComponent<EventSystem>()
				);

			SceneMgr.Instance.InitMgr(this);
		}

		void RegisterUI()
		{
			UIMgr.Instance.Register<MenuWnd>(Common.TrimName(Constants_Demo14.MenuPanel, TrimNameType.Slash));
			UIMgr.Instance.Register<LoadWnd>(Common.TrimName(Constants_Demo14.LoadPanel, TrimNameType.Slash));
		}
        #endregion



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

