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
/// AB包配置
/// </summary>
[CreateAssetMenu(fileName = "ABConfig", menuName = "Create ABConfig", order = 0)]

public class ABCfg : ScriptableObject
{
	//名字 唯一性
	/// <summary>存储prefab的路径</summary>
	public List<string> prefabPathLst = new List<string>();
    /// <summary>非prefab的路径</summary>
    public List<AB2Path> folderPathLst = new List<AB2Path>();



    [Serializable]
	public struct AB2Path
	{ 
		public string ABName;
		public string Path;
	
	}
}

/// <summary>
/// cfg
/// </summary>
[Serializable]
public class ABConfig
{
	[XmlElement("ABLst")]
	
	public List<ABBase> ABLst { get; set; }


}



/// <summary>
/// AB
/// </summary>
[Serializable]
public class ABBase
{
	[XmlElement("Path")]
	public string Path { get; set; }

	[XmlElement("Crc")]
	/// <summary>冗余校验</summary>
	public uint Crc { get; set; }

	[XmlElement("ABName")]
	public string ABName { get; set; }

	[XmlElement("ABDependce")]
	public List<string> ABDependce { get; set; }

	[XmlElement("AssetName")]
	public string AssetName { get; set; }

}
