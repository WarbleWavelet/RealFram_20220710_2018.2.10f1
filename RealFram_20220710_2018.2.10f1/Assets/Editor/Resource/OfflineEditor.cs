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
    [MenuItem(Constants.MenuItem_Offline+ "Create OfflineData", false, 100)]
    static void AssetsCreateOfflineData()
    {
        GameObject[] goArr=Selection.gameObjects;
        for (int i = 0; i < goArr.Length; i++)
        {
            GameObject go = goArr[i];

            string title = "正在添加离线数据";
            string info = "";
            info += "正在修改" + go.name + "....";
            float prg = ( 1.0f * i ) /  goArr.Length;
            EditorUtility.DisplayCancelableProgressBar(title, info, prg);
            CreateOfflineData(go);
        }

       EditorUtility.ClearProgressBar();

    }


    #region UIOfflineData
  [MenuItem(Constants.MenuItem_Offline + "Create UIOfflineData", false, 101)]
    static void AssetsCreateUIOfflineData()
    {
        GameObject[] goArr = Selection.gameObjects;
        for (int i = 0; i < goArr.Length; i++)
        {
            GameObject go = goArr[i];

            string title = "正在添加离线数据";
            string info = "";
            info += "正在修改" + go.name + "....";
            float prg = (1.0f * i) / goArr.Length; ;
            EditorUtility.DisplayCancelableProgressBar(title, info, prg);
            CreateUIOfflineData(go);
        }

        EditorUtility.ClearProgressBar();
    }

    [MenuItem(Constants.MenuItem_Offline + "Create All UIOfflineData", false, 102)]
    static void AssetsCreateAllUIOfflineData()
    {
        string[] guidArr = AssetDatabase.FindAssets(Constans_UIOfflineData.m_Type, new string[] { Constans_UIOfflineData.m_Path });
        for (int i = 0; i < guidArr.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guidArr[i]);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go == null)
            {
                continue;
            }
            //
            string title = "正在添加离线数据";
            string info = "";
            info += "正在修改" + go.name + "....";
            float prg = (1.0f * i) / guidArr.Length; ;
            EditorUtility.DisplayCancelableProgressBar(title, info, prg);
            CreateUIOfflineData(go);
        }

        EditorUtility.ClearProgressBar();

    }


    [MenuItem(Constants.MenuItem_Offline + "Reset All UIOfflineData", false, 103)]
    static void AssetsResetAllUIOfflineData()
    {

        string[] guidArr = AssetDatabase.FindAssets(Constans_OfflineData.m_Type, new string[] { Constans_OfflineData.m_Path });
        for (int i = 0; i < guidArr.Length; i++)
        {
           string path= AssetDatabase.GUIDToAssetPath(guidArr[i]);
            GameObject go=AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go == null)
            {
                continue;
            }
            //
            string title = "正在删除离线数据";
            string info = "";
            info += "正在重置" + go.name + "....";
            float prg = (1.0f * i) / guidArr.Length; ;
           // EditorUtility.DisplayCancelableProgressBar(title, info, prg);
            go.GetComponent<UIOfflineData>().Reset();
        }

        //EditorUtility.ClearProgressBar();

    }

    #endregion




    #region ParticleOfflineData
 [MenuItem(Constants.MenuItem_Offline + "Create All ParticleOfflineData", false, 104)]
    static void AssetsCreateAllParticleOfflineData()
    {

        string[] guidArr = AssetDatabase.FindAssets(Constans_ParticleOfflineData.m_Type, new string[] { Constans_ParticleOfflineData.m_Path });
        for (int i = 0; i < guidArr.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guidArr[i]);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go == null)
            {
                continue;
            }
            //
            string title = "正在添加离线数据";
            string info = "";
            info += "正在修改" + go.name + "....";
            float prg = (1.0f * i) / guidArr.Length; ;
            EditorUtility.DisplayCancelableProgressBar(title, info, prg);

            //
            CreateAllOfflineData<ParticleOfflineData>(go);
        }

        EditorUtility.ClearProgressBar();

    }
    #endregion
   

    #region 辅助
    /// <summary>
    /// 选中添加
    /// </summary>
    /// <param name="go"></param>
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
    

    /// <summary>
    /// 选中添加
    /// </summary>
    /// <param name="go"></param>
    public static void CreateUIOfflineData(GameObject go)
    {
        go.layer = LayerMask.NameToLayer(go.name);
        UIOfflineData data = go.GetComponent<UIOfflineData>();
        if (data == null)
        { 
            data=go.AddComponent<UIOfflineData>();
        }
        data.BindData();   
        //
        Debug.LogFormat("修改了{0}",go.name);
        EditorUtility.SetDirty(go);
        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();
    }  
    

    /// <summary>
    /// 为文件夹下的所有物体添加Offlinedata
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="go"></param>
    public static void CreateAllOfflineData<T>(GameObject go) where  T:OfflineData
    {
        T data = go.GetComponent<T>();
        if (data == null)
        { 
            data=go.AddComponent<T>();
        }
        data.BindData();   
        //
        Debug.LogFormat("修改了{0}",go.name);
        EditorUtility.SetDirty(go);
        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();
    }
    #endregion

}

