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
	public class GameStart : MonoSingleton<GameStart>
	{

		#region 字属
		public Transform m_RecyclePoolTrans;
		public Transform m_SceneTrans;

		public Button btnLoadScene;
		public Toggle toggle;
		//
		public Button btnInstaniate;
		public Button btnUnload;
		public Button btnUnloadResource;//不能实例的Object
		public Button btnPreload;
		public Button btnPreloadResObj;
		public Button btnPlay;
		AudioClip clip;
		AudioSource source;
		#endregion

		#region 生命
		public override	void Awake()
		{
			base.Awake();
            GameObject.DontDestroyOnLoad(gameObject);

			InitMgr();
			RegisterUI();
			BindUI();

			ObjectMgr.Instance.InstantiateObject("Assets/GameData/Prefabs/Cube.prefab",true);

		}

		void BindUI()
        {

            #region Mgr中资源的加载方式、切场景
             toggle.onValueChanged.AddListener((bool state) =>
			{
				toggle.GetComponentInChildren<Text>().text = state == true ? "LoadFromAB" : "LoadFromEditor";
				ResourceMgr.Instance.SetLoadFromAB(state);
			});




			Common.BindBtn(btnLoadScene, () =>
			{
				SceneMgr.Instance.LoadScene(Constants_Demo14.Prefab_LoadPanel, Constants_Demo14.Scene_Menu);
				btnLoadScene.interactable = false;
			});
            #endregion


            #region 测试实例
             GameObject go = null;
			Common.BindBtn(btnInstaniate, () =>
			{
				go = ObjectMgr.Instance.InstantiateObject(Constants_Demo14.Prefab_Attack, true, true);
			});


			Common.BindBtn(btnUnload, () =>
			{
				ObjectMgr.Instance.UnloadGameObject(go);
				go = null; //引用置空

			});
            #endregion



            #region 预加载
             Common.BindBtn(btnPreload, () =>
			{
				ObjectMgr.Instance.PreloadGameObject(Constants_Demo14.Prefab_Attack,5);


			});				
			Common.BindBtn(btnPreloadResObj, () =>
			{
				ResourceMgr.Instance.PreLoadObject(Constants_Demo14.MP3_SenLin);


			});
            #endregion


            #region mp3等，在切换场景后要用，MenuWnd
			Common.BindBtn(btnPlay, () =>
			{
				 clip = ResourceMgr.Instance.LoadResource<AudioClip>(Constants_Demo14.MP3_SenLin);

			});
			Common.BindBtn(btnUnloadResource, () =>
			{
				if (ResourceMgr.Instance.UnloadResItemByObject(clip, false) == true) //false，来测试切换场景
				{ 
					clip = null;	//删引用				
				}


			});
            #endregion

		}

		void B(Action curSceneAction, Action tarSceneAction)
		{
			curSceneAction();
			SceneMgr.Instance.LoadScene(Constants_Demo14.Prefab_LoadPanel, Constants_Demo14.Scene_Menu);
			MenuWnd menuWnd =  UIMgr.Instance.GetWnd<MenuWnd>(Common.TrimName(Constants_Demo14.Prefab_MenuPanel, TrimNameType.SlashAfter));
			menuWnd.OnShow(tarSceneAction);
		}
		void A()
		{

			B(() => {
				ObjectMgr.Instance.PreloadGameObject(Constants_Demo14.Prefab_Attack, 5);
			},
			() => {
				//使用预加载的可实例资源
				GameObject go = ObjectMgr.Instance.InstantiateObject(Constants_Demo14.Prefab_Attack, true);
				ObjectMgr.Instance.UnloadGameObject(go);
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
			UIMgr.Instance.Register<MenuWnd>(Common.TrimName(Constants_Demo14.Prefab_MenuPanel, TrimNameType.SlashAfter));
			UIMgr.Instance.Register<LoadWnd>(Common.TrimName(Constants_Demo14.Prefab_LoadPanel, TrimNameType.SlashAfter));
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

