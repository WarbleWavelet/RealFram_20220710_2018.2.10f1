/****************************************************
    文件：AssetBundleHotFixEditor.cs
	作者：lenovo
    邮箱: 
    日期：2022/9/14 15:58:35
	功能：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetBundleHotFixEditor2
{

    [MenuItem(DefinePath.MenuItem_AB + "AB热更编辑器", false, 100)]
    static void MenuItem_AB_Init()
    {
        AssetBundleHotFixEWnd.GetWnd();
    }
}
