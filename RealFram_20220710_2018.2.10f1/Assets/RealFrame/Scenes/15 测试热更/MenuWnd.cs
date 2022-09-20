/****************************************************
    文件：MenuWnd.cs
	作者：lenovo
    邮箱: 
    日期：2022/9/18 15:18:14
	功能：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Demo15
{
	public class MenuWnd : MonoBehaviour {

		// Use this for initialization
		void Start() {
			Test_PreloadAndInstaniate();
	}

		// Update is called once per frame
		void Update() {

		}


		void Test_PreloadAndInstaniate()
		{
			GameObject go = ObjectMgr.Instance.InstantiateObject(DefinePath_Demo14.Prefab_Attack, true); //
			ObjectMgr.Instance.UnloadGameObject(go);
			ObjectMgr.Instance.UnloadGameObject(go, 0, true);
		}
	}

}
			
