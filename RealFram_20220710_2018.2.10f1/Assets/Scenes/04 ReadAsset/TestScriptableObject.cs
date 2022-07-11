/****************************************************
    文件：TestScriptableObject.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/11 14:55:37
	功能：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScriptableObject : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        ReadTestAssets(DefinePath.path_04_Asset_Read);
	}

    void ReadTestAssets(string path)
    {
        AssetsSerilize assets = UnityEditor.AssetDatabase.LoadAssetAtPath<AssetsSerilize>(path);
        Debug.Log(assets.Id);
        Debug.Log(assets.Name);
        foreach (string str in assets.TestList)
        {
            Debug.Log(str);
        }
    }
}
