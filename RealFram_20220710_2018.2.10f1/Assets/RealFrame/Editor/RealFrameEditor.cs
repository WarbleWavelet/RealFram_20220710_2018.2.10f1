/****************************************************
    文件：RealFrameEditor.cs
	作者：lenovo
    邮箱: 
    日期：2022/8/19 22:7:59
	功能：初始化创建的RealFrameCfg (需要放在Editor下，不然会报错)
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static RealFrameCfgSO;

public class RealFrameEditor : Editor
{

    #region 字属
    public static string m_realFrameCfgSOPath = DefinePath.RealFrameCfgSOPath;
    static RealFrameCfgSO m_realFrameCfgSO;
    const string m_realFrame = DefinePath.RealFrameName;
    #endregion



    [MenuItem(DefinePath.MenuItem_RealFrame + "/定位配置表", false, 0)] //Alt+R打开资源路径 ,Unity上的路径
    static void MenuItem_ShootRealFrameCfg()
    {
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(m_realFrameCfgSOPath);
    }


    [MenuItem(DefinePath.MenuItem_RealFrame + "/初始化配置表", false, 0)] //Alt+R打开资源路径 ,Unity上的路径
    static void MenuItem_InitRealFrameCfg()
    {
        m_realFrameCfgSO = AssetDatabase.LoadAssetAtPath<RealFrameCfgSO>(m_realFrameCfgSOPath);

        m_realFrameCfgSO.m_ABBinPath = "Assets/" + m_realFrame + "/GameData/Data/ABData";//不用加/
        m_realFrameCfgSO.m_BinPath = "Assets/" + m_realFrame + "/GameData/Data/Bin";//不用加/
        m_realFrameCfgSO.m_ScriptsPath = "Assets/" + m_realFrame + "/Editor/SO";//不用加/
        m_realFrameCfgSO.m_XmlPath = "Assets/" + m_realFrame + "/GameData/Data/Xml";//不用加/
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

}
