/****************************************************
    文件：Common_Editor.cs
	作者：lenovo
    邮箱: 
    日期：2022/9/19 21:57:20
	功能：Common类，但需要放在Editor下
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Common_Editor
{
    public static string BuildTarget = EditorUserBuildSettings.activeBuildTarget.ToString();
}