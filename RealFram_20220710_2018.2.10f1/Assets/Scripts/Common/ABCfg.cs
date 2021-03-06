/****************************************************
    文件：ABConfig.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/12 20:48:41
	功能：Editor 和 Mono 都用到
*****************************************************/


using System;
using System.Collections.Generic;
using System.Xml.Serialization;



/// <summary>
/// AB配置类 List<ABBase>
/// </summary>
[Serializable]
public class ABCfg
{
	[XmlElement("ABLst")]

	public List<ABBase> ABLst { get; set; }


}



/// <summary>
/// AB基类
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


[System.Serializable]
public class XmlCfg
{
	[XmlAttribute("Id")]
	public int Id { get; set; }
	[XmlAttribute("Name")]
	public string Name { get; set; }
	[XmlElement("List")]
	public List<int> Lst { get; set; }
}