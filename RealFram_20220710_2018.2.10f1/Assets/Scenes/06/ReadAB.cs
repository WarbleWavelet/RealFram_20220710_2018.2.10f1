/****************************************************
    文件：ReadAB.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/13 15:55:52
	功能：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Demo06
{
	public class ReadAB : MonoBehaviour
	{

		// Use this for initialization
		void Start()
		{
			AssetBundleMgr.Instance.LoadABCfg();
		}

		// Update is called once per frame
		void Update()
		{

		}
	}

}
