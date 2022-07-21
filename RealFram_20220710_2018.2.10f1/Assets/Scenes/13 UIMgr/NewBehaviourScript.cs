/****************************************************
    文件：NewBehaviourScript.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/21 21:53:34
	功能：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UnityEngine.Debug.Log(this.GetType().ToString());//类名
		UnityEngine.Debug.Log(new StackTrace().GetFrame(0).GetMethod());//方法名



    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
