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
using UnityEngine.UI;

namespace Demo06
{
	public class ReadABByMgr : MonoBehaviour
	{
		public Button btn1;
		// Use this for initialization
		void Start()
		{
			btn1.onClick.AddListener(
				() => {
					AssetBundleMgr.Instance.InitMgr();
				});

		}

		// Update is called once per frame
		void Update()
		{

		}



	}
}