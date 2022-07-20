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

namespace Demo11
{
	public class AsyncInstaniateObject : MonoBehaviour
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
			bool log = false;
			AssetBundleMgr.Instance.LoadABCfg(log);
			ResourceMgr.Instance.InitCoroutine(this);

			ObjectMgr.Instance.InitMgr(RecyclePoolTrans, transform);
		}

		void Start()
		{
			BindUI();
			path = "Assets/GameData/Prefabs/Attack.prefab";
		}

		void Yang()
		{
			bool setScene=true;
			bool jmpClr=true;
			ObjectMgr.Instance.AsyncInstaniateGameObject(path, CallBack ,AsyncLoadResPriority.Middle, setScene);
			
		}
		void CallBack(string path, UnityEngine.Object obj, object para1 = null, object para2 = null, object para3 = null)
		{ 
			go= obj as GameObject;
			FixShader(go);
		}
		void Yin()
		{
  			ObjectMgr.Instance.UnloadGameObject(go);
			go = null;


		}
		void Yin1()
		{
		ObjectMgr.Instance.UnloadGameObject(go,0, true);
			go = null;
		}

		void Refresh()
		{

#if UNITY_EDITOR//这样写才对，不能括外面
			ResourceMgr.Instance.ClearAllResItem();
			Resources.UnloadUnusedAssets();
#endif
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
			ResourceMgr.Instance.ClearAllResItem();
			Resources.UnloadUnusedAssets();
			Debug.Log("清存");
#endif
		}





	void FixShader(GameObject go)
	{
		MeshRenderer[] mr = go.GetComponentsInChildren<MeshRenderer>();
		List<Material> lst = new List<Material>();
		for (int i = 0; i < mr.Length; i++)
		{
			lst.AddRange(mr[i].materials);
		}
		SkinnedMeshRenderer[] smr = go.GetComponentsInChildren<SkinnedMeshRenderer>();
		for (int i = 0; i < smr.Length; i++)
		{
			lst.AddRange(smr[i].materials);
		}

		for (int i = 0; i < lst.Count; i++)
		{
			lst[i].shader = Shader.Find("Custom/benghuai");
		}
	}
	}

}

