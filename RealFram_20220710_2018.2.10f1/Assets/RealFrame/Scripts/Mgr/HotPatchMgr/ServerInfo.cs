/****************************************************
    文件：ServerInfo.cs
	作者：lenovo
    邮箱: 
    日期：2022/9/14 16:54:31
	功能：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[System.Serializable]
public class ServerInfo
{
    [XmlElement("GameVersion")]
    public VersionInfo[] GameVersion;
}

//当前游戏版本对应的所有补丁
[System.Serializable]
public class VersionInfo
{
    [XmlAttribute]
    public string Version;
    [XmlElement]
    public Pathces[] Pathces;
}


[System.Serializable]
public class Pathces//一个总补丁包
{
    [XmlAttribute]
    public int Version;

    [XmlAttribute]
    public string Des;

    [XmlElement]
    public List<Patch> Files;//所有热更文件
}


[System.Serializable]
public class Patch// 单个补丁包
{
    [XmlAttribute]
    public string Name;

    [XmlAttribute]
    public string Url;//下载路径

    [XmlAttribute]
    public string Platform;//平台

    [XmlAttribute]
    public string Md5;

    [XmlAttribute]
    public float Size;
}
