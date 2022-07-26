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
using System.Xml.Serialization;
using UnityEngine;


/// <summary>
/// AB包配置 : ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "ABConfig", menuName = "Create ABConfig", order = 0)]

public class ABCfgSO : ScriptableObject
{
	//名字 唯一性
	/// <summary>存储prefab的路径</summary>
	public List<string> m_PrefabPathLst = new List<string>();
    /// <summary>非prefab的路径</summary>
    public List<AB2Path> m_FolderPathLst = new List<AB2Path>();



    [Serializable]
	public struct AB2Path
	{ 
		public string m_ABName;
		public string m_Path;
	
	}
}
