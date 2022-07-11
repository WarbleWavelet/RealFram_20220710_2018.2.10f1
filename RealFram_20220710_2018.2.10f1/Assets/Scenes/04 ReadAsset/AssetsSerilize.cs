/****************************************************
    文件：AssetsSerilize.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/11 14:42:4
	功能：Assets的序反
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = Constants.MenuAsset,menuName ="CreatAssets",order = 0)]//顺序
public class AssetsSerilize : ScriptableObject
{
    public int Id;
    public string Name;
    public List<string> TestList;
}
