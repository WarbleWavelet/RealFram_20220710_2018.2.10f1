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

namespace Demo16
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
		public	bool LoadFromAB = true;


		#endregion


		#region 生命
		public override void Awake()
		{
			base.Awake();
			GameObject.DontDestroyOnLoad(gameObject);

			//

			ResourceMgr.Instance.SetLoadFromAB(LoadFromAB);
			InitMgr();
			RegisterUI();
			BindUI();



		}

        void Start()
        {
			//Test_AddABAndInstance();
			//Test_LoadDataCfg();
        }

		void Update()
		{
			UIMgr.Instance.OnUpdate();

		}

		private void OnApplicationQuit()
		{
			Debug.Log("OnApplicationQuit");

			//#if UNITY_EDITOR//这样写才对，不能括外面
			ResourceMgr.Instance.ClearAllResItem();
			Resources.UnloadUnusedAssets();
			Debug.Log("清存");
			//#endif
		}




		#endregion


		#region 辅助
		void BindUI()
		{
			BindUI_Toggle();
			BindUI_Button();
		}

		#region 测试 能新增AB包并且可以实例	 
		void Test_AddABAndInstance()
		{
			ObjectMgr.Instance.InstantiateObject(DefinePath_Demo16.Prefab_Attack, true);
		}
		#endregion


		#region 测试 数据配置
		void Test_LoadDataCfg()
		{
			CfgMgr.Instance.LoadData<MonsterData>(DefinePath.Cfg_MonsterData_Inner);
			CfgMgr.Instance.LoadData<BuffData>(DefinePath.Cfg_BuffData); //拆分类列表
																		 //CfgMgr.Instance.LoadData<BuffData>(DefinePath.Cfg_BuffData2); //不拆分类列表
		}
		#endregion


		void BindUI_Toggle()
		{ 
			toggle.GetComponentInChildren<Text>().text = LoadFromAB.ToString();
			toggle.isOn = LoadFromAB;
			toggle.onValueChanged.AddListener((bool _state) =>
			{
				ResourceMgr.Instance.SetLoadFromAB(_state);
				toggle.GetComponentInChildren<Text>().text = ResourceMgr.Instance.GetLoadFromAB().ToString();
			});		
		}
		void BindUI_Button()
		{ 
			#region Mgr中资源的加载方式、切场景


			Common.BindBtn(btnLoadScene, () =>
			{
				bool allResources = false; //一次控制两个Wnd
				Demo16.LoadWnd.m_resources = allResources;//控制LoadWnd的下一个WndMenuWnd
				SceneMgr.Instance.LoadScene(
					loadingUI: DefinePath_Demo16.Prefab_LoadPanel,
					tarScene: DefinePath_Demo16.Scene_Menu,
					resource: allResources,
					isTop:true
				);
				btnLoadScene.interactable = false;
			});
			#endregion


			#region 测试实例
			//GameObject go = null;
			//Common.BindBtn(btnInstaniate, () =>
			//{
			//	go = ObjectMgr.Instance.InstantiateObject(DefinePath_Demo16.Prefab_Attack, true, true);
			//});


			//Common.BindBtn(btnUnload, () =>
			//{
			//	ObjectMgr.Instance.UnloadGameObject(go);
			//	go = null; //引用置空
			//});
			#endregion



			#region 预加载
			Common.BindBtn(btnPreload, () =>
			{
				ObjectMgr.Instance.PreloadGameObject(DefinePath_Demo16.Prefab_Attack, 5);


			});
			Common.BindBtn(btnPreloadResObj, () =>
			{
				ResourceMgr.Instance.PreLoadObject(DefinePath_Demo16.MP3_SenLin);


			});
			#endregion


			#region mp3等，在切换场景后要用，MenuWnd
			Common.BindBtn(btnPlay, () =>
			{
				clip = ResourceMgr.Instance.LoadResource<AudioClip>(DefinePath_Demo16.MP3_SenLin);

			});
			Common.BindBtn(btnUnloadResource, () =>
			{
				if (ResourceMgr.Instance.UnloadResItemByObject(clip, false) == true) //false，来测试切换场景
				{
					clip = null;    //删引用				
				}


			});
			#endregion		
		}


		void Btn_LoadScene(Action curSceneAction, Action tarSceneAction)
		{
			curSceneAction();
			SceneMgr.Instance.LoadScene(DefinePath_Demo16.Prefab_LoadPanel, DefinePath_Demo16.Scene_Menu,resource:true,isTop:true);
			//MenuWnd menuWnd = UIMgr.Instance.Wnd_Get<MenuWnd>(Common.TrimName(DefinePath_Demo16.Prefab_MenuPanel, TrimNameType.SlashAfter));
			//menuWnd.OnShow(tarSceneAction);
		}	


		void Btn_PreLoadGameObject()
		{

			Btn_LoadScene(() => {
				ObjectMgr.Instance.PreloadGameObject(DefinePath_Demo16.Prefab_Attack, cnt:5);
			},
			() => {
				//使用预加载的可实例资源
				GameObject go = ObjectMgr.Instance.InstantiateObject(DefinePath_Demo16.Prefab_Attack, true);
				ObjectMgr.Instance.UnloadGameObject(go);
			});
		}


		void InitMgr()
		{
			bool log = true;			
			AssetBundleMgr.Instance.InitMgr(log);
			ResourceMgr.Instance.InitMgr(this);			
			ObjectMgr.Instance.InitMgr(m_RecyclePoolTrans, m_SceneTrans);
			UIMgr.Instance.InitMgr(
				transform.Find("UIRoot") as RectTransform,
				transform.Find("UIRoot/WindowRoot") as RectTransform,
				transform.Find("UIRoot/UICamera").GetComponent<Camera>(),
				transform.Find("UIRoot/EventSystem").GetComponent<EventSystem>()
				);

			ILRuntimeMgr.Instance.InitMgr(GameStart: gameObject);


			SceneMgr.Instance.InitMgr(this);
		}

		void RegisterUI()
		{
			UIMgr.Instance.Register<MenuWnd>(Common.TrimName(DefinePath_Demo16.Prefab_MenuPanel, TrimNameType.SlashAfter));
			UIMgr.Instance.Register<LoadWnd>(Common.TrimName(DefinePath_Demo16.Prefab_LoadPanel, TrimNameType.SlashAfter));
		}
        #endregion




	}

}

