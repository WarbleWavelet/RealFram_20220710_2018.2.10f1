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

namespace Demo09
{
	public class PreLoad : MonoBehaviour
	{
		public bool usePreLoad=true;
		 AudioSource audioSource;
	public	string path = "";

		public Button btnPlay;
		public Button btnRelease;
		AudioClip clip;
		void Awake()
		{
			AssetBundleMgr.Instance.LoadABCfg(false);
		}

		void Start()
		{
			BindUI();
			path = "Assets/GameData/Sounds/senlin.mp3";
			clip = null;
			//
			if (usePreLoad)
			{ 
				ResourceMgr.Instance.PreLoadObject(path) ;			
			}

		}

		void Play()
		{
			if (clip == null)
			{
				long lastTime = DateTime.Now.Ticks;
				clip = ResourceMgr.Instance.LoadResource<AudioClip>(path);
				Debug.LogFormat("预加载：{0}，加载时间：{1}",usePreLoad, (DateTime.Now.Ticks - lastTime));
			}

			audioSource.clip = clip;
			audioSource.Play();
		}

		void StopAndRelease()
		{
			ResourceMgr.Instance.UnloadResItemByObject(clip, true);
			audioSource.clip=null;//防止miss情况
			clip = null;
		}


		private void OnApplicationQuit()
		{
			Debug.Log("OnApplicationQuit");
			ResourceMgr.Instance.ClearAllResItem();

#if UNITY_EDITOR//这样写才对，不能括外面
			Resources.UnloadUnusedAssets();
#endif
		}



		private void BindUI()
		{
			audioSource = GetComponent<AudioSource>();
			Transform t = GameObject.FindGameObjectWithTag(Tags.Canvas).transform;
			btnPlay = t.Find("btnPlay").GetComponent<Button>();
			btnRelease = t.Find("btnRelease").GetComponent<Button>();
			//
			btnPlay.onClick.AddListener(Play);
			btnRelease.onClick.AddListener(StopAndRelease);
		}

	}
}

