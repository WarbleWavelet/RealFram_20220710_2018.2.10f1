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
using UnityEngine.UI;

namespace Demo10
{
	public class InstaniateObject : MonoBehaviour
	{

		public	string path = "";
		public Button btnYang;
		public Button btnYin;
		public Button btnYin1;
		public Button btnRefresh;
		GameObject go;
	public	Transform RecyclePoolTrans;

		void Awake()
		{
			//GameObject.DontDestroyOnLoad(gameObject);
			AssetBundleMgr.Instance.LoadABCfg(false);
			ObjectMgr.Instance.InitMgr(RecyclePoolTrans, transform);
		}

		void Start()
		{
			BindUI();
			path = "Assets/GameData/Prefabs/Attack.prefab";
		}

		void Yang()
		{
			go = ObjectMgr.Instance.InstantiateObject(path, true);
		}

		void Yin()
		{
  			ObjectMgr.Instance.ReleaseObject(go);
			go = null;


		}
		void Yin1()
		{
		
			ObjectMgr.Instance.ReleaseObject(go,0, true);
			go = null;
		}






		private void BindUI()
		{
			Transform t = GameObject.FindGameObjectWithTag(Tags.Canvas).transform;
			btnYang = t.Find("btnYang").GetComponent<Button>();
			btnYin = t.Find("btnYin").GetComponent<Button>();
			btnYin1 = t.Find("btnYin1").GetComponent<Button>();
			btnRefresh = t.Find("btnRefresh").GetComponent<Button>();
			//
			btnYang.onClick.AddListener(Yang);
			btnYin.onClick.AddListener(Yin);
			btnYin1.onClick.AddListener(Yin1);
			btnRefresh.onClick.AddListener(Refresh);
		}






		private void OnApplicationQuit()
		{
			Debug.Log("OnApplicationQuit");

#if UNITY_EDITOR//这样写才对，不能括外面
			ResourceMgr.Instance.ClearCache();
			Resources.UnloadUnusedAssets();
			Debug.Log("清存");
#endif
		}


	void Refresh()
	{
#if UNITY_EDITOR//这样写才对，不能括外面
			Resources.UnloadUnusedAssets();	
#endif	
	}

	}

}

