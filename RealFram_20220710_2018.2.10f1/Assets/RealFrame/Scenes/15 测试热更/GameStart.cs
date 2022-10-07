/****************************************************
    文件：GameStart.cs
	作者：lenovo
    邮箱: 
    日期：2022/9/17 11:1:2
	功能：
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Demo15
{
	public class GameStart : MonoSingleton<GameStart>
	{
		#region 字属
		 

		public Transform m_RecyclePoolTrans;
		public Transform m_SceneTrans;
		public bool LoadFromAB = false;
		public const string m_CommonConfirm = DefinePath_Demo15.CommonConfirm;
		public const string m_Hotfix = DefinePath_Demo15.Hotfix;



		#endregion



		#region 生命



		public override void Awake()
		{
			base.Awake();
			GameObject.DontDestroyOnLoad(gameObject);


#if UNITY_EDITOR
			   Common.SetBuildTarget(UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString());
#endif


			//
			ResourceMgr.Instance.SetLoadFromAB(LoadFromAB);
			InitMgr();
			RegisterUI();
			BindUI();
		}

		void Start()
		{
			UIMgr.Instance.OpenWnd(m_Hotfix,resources:true);

			
		}



		void Update()
		{
			UIMgr.Instance.OnUpdate();

		}


		private void OnApplicationQuit()
		{
			//Debug.Log("OnApplicationQuit");

			//#if UNITY_EDITOR//这样写才对，不能括外面
			ResourceMgr.Instance.ClearAllResItem();
			Resources.UnloadUnusedAssets();
			//Debug.Log("清存");
			//#endif
		}

		#endregion




		#region 辅助		

		void BindUI()
		{
		}


		void InitMgr()
		{


			bool log = true;
			//AssetBundleMgr.Instance.InitMgr(log);//HotPatchMgr来控制,调用StartGame
			ResourceMgr.Instance.InitMgr(this);
			ObjectMgr.Instance.InitMgr(m_RecyclePoolTrans, m_SceneTrans);
			HotPatchMgr.Instance.InitMgr(this);			
			UIMgr.Instance.InitMgr(
				transform.Find("UIRoot") as RectTransform,
				transform.Find("UIRoot/WindowRoot") as RectTransform,
				transform.Find("UIRoot/UICamera").GetComponent<Camera>(),
				transform.Find("UIRoot/EventSystem").GetComponent<EventSystem>()
				);

			//SceneMgr.Instance.InitMgr(this);//HotPatchMgr来控制,调用StartGame
		}

		void RegisterUI()
		{
			UIMgr.Instance.Register<HotfixWnd>(m_Hotfix);
		
		}
		#endregion






		public static void OpenCommonConfirm(string title,
			string des,
			UnityEngine.Events.UnityAction confirmAction,
			UnityEngine.Events.UnityAction cancleAction)
		{

			GameObject commonObj = GameObject.Instantiate(Resources.Load<GameObject>(m_CommonConfirm)) as GameObject;
			commonObj.transform.SetParent(UIMgr.Instance.m_wndRoot, false);
			CommonConfirm commonItem = commonObj.GetComponent<CommonConfirm>();
			commonItem.Show(title, des, confirmAction, cancleAction);
		}


		public IEnumerator StartGame(Image image, Text text) //其它加载放在InitMgr
		{
			image.fillAmount = 0;
			yield return null;

			text.text = "加载AB数据... ...";
			AssetBundleMgr.Instance.InitMgr(); 
			image.fillAmount = 0.5f;
			yield return null;

 			text.text = "加载ILRuntime... ...";
			ILRuntimeMgr.Instance.InitMgr();
			image.fillAmount = 0.7f;
			yield return null;





			text.text = "加载数据表... ...";
			//LoadConfiger();
			image.fillAmount = 0.8f;
			yield return null;

			text.text = "加载配置... ...";
			image.fillAmount = 0.9f;
			yield return null;

			text.text = "初始化地图... ...";
			SceneMgr.Instance.InitMgr(this);
			image.fillAmount = 1f;
			yield return null;
		}




	}

}
