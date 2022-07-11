/****************************************************
    文件：ABConfig.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/11 18:23:19
	功能：AB包配置
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "ABConfig", menuName = "Create ABConfig", order = 0)]

public class ABConfig : ScriptableObject
{
	//名字 唯一性
	/// <summary>存储prefab的路径</summary>
	public List<string> m_AllPrefabPath = new List<string>();
    /// <summary>非prefab的路径</summary>
    public List<FileDirABName> m_AllFileDirAB = new List<FileDirABName>();



    [Serializable]
	public struct FileDirABName
	{ 
		public string ABName;
		public string Path;
	
	}
}
