/****************************************************
    文件：ReadResFromMgr.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/14 16:2:20
	功能：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadResFromMgr : MonoBehaviour {


	AudioSource audioSource;

    void Awake()
    {
		AssetBundleMgr.Instance.InitMgr();
    }
	// Use this for initialization
	void Start () 
	{
		
		audioSource = GetComponent<AudioSource>();
		string path = "Assets/GameData/Sounds/senlin.mp3";
		audioSource.clip= ResourceMgr.Instance.LoadResource<AudioClip>(path);
		audioSource.Play();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Z))
		{ 
			audioSource.Stop();
			AudioClip clip = audioSource.clip;
			audioSource.clip = null;
			ResourceMgr.Instance.UnloadResItemByObject( clip,true);
		}
	}
}
