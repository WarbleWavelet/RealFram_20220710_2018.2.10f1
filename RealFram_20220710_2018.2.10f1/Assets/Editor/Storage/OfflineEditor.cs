/****************************************************
    文件：StorageEditor.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/20 12:33:7
	功能：存储编辑器
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OfflineEditor 
{
    //放在文件夹Editor下。相邻超过10为一组有分割线
    [MenuItem(Constants.MenuItem+"/生成离线数据", false, 100)]
    static void AssetsCreateOfflineData()
    {
        GameObject[] goArr=Selection.gameObjects;
        for (int i = 0; i < goArr.Length; i++)
        {
            GameObject go = goArr[i];

            string title = "正在添加离线数据";
            string info = "";
            info += "正在修改" + go.name + "....";
            float prg = 1.0f / (i * goArr.Length);
            EditorUtility.DisplayCancelableProgressBar(title, info, prg);
            CreateOfflineData(go);
        }

       EditorUtility.ClearProgressBar();

    }

    public static void CreateOfflineData(GameObject go)
    {
        OfflineData data = go.GetComponent<OfflineData>();
        if (data == null)
        { 
            data=go.AddComponent<OfflineData>();
        }
        data.BindData();   
        //
        Debug.LogFormat("修改了{0}",go.name);
        EditorUtility.SetDirty(go);
        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();
    }
}

