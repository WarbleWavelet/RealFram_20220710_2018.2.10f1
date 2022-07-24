/****************************************************
    文件：AsyncReadResFromMgr.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/14 23:43:33
	功能：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Demo08
{
	public class AsyncReadResFromMgr : MonoBehaviour
	{

		AudioSource audioSource;

		void Awake()
		{
			AssetBundleMgr.Instance.InitMgr();
			ResourceMgr.Instance.InitMgr(this);
		}

		void Start()
		{

			audioSource = GetComponent<AudioSource>();
			string path = "Assets/GameData/Sounds/senlin.mp3";
			 ResourceMgr.Instance.AsyncLoadObject(path, Callback, AsyncLoadResPriority.Middle);

		}

		// Update is called once per frame
		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Z))
			{
				
				audioSource.Stop();
				AudioClip clip = audioSource.clip;
				ResourceMgr.Instance.UnloadResItemByObject(clip, true);
				audioSource.clip = null;
				clip = null;
				
			}
		}

		void Callback(string path, Object obj, object para1, object para2, object para3)
		{
			audioSource.clip = obj as AudioClip;
			audioSource.Play();
		}


#if UNITY_EDITOR
		private void OnApplicationQuit()
		{
			Debug.Log("OnApplicationQuit");
		ResourceMgr.Instance.ClearAllResItem();
			Resources.UnloadUnusedAssets();
		}
#endif




    }
}

