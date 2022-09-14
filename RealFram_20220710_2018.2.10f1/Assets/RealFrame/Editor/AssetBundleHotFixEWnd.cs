/****************************************************
    文件：AssetBundleHotFix.cs
	作者：lenovo
    邮箱: 
    日期：2022/8/28 19:28:59
	功能：AB热更
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;

public class AssetBundleHotFixEWnd : EditorWindow
{

    #region 字属

    string m_abMD5_OutterPath="";
    string m_hotCnt = "1"; //热更次数
    OpenFileName m_OpenFileName = null;


    #endregion




    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        m_abMD5_OutterPath = EditorGUILayout.TextField("ABMD5路径： ", m_abMD5_OutterPath, GUILayout.Width(350), GUILayout.Height(20));//文本框
        if (GUILayout.Button("选择版本ABMD5文件", GUILayout.Width(150), GUILayout.Height(30)))//按钮
        {
            m_OpenFileName = new OpenFileName();
            m_OpenFileName.structSize = Marshal.SizeOf(m_OpenFileName);
            m_OpenFileName.filter = "ABMD5文件(*.bytes)\0*.bytes";
            m_OpenFileName.file = new string(new char[256]);
            m_OpenFileName.maxFile = m_OpenFileName.file.Length;
            m_OpenFileName.fileTitle = new string(new char[64]);
            m_OpenFileName.maxFileTitle = m_OpenFileName.fileTitle.Length;
            m_OpenFileName.initialDir = (DefinePath.ABMD5_OutterPath).Replace("/", "\\");//默认路径
            m_OpenFileName.title = "选择MD5窗口";
            m_OpenFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
            if (LocalDialog.GetSaveFileName(m_OpenFileName))
            {
                Debug.Log(m_OpenFileName.file);
                m_abMD5_OutterPath = m_OpenFileName.file;
            }
        }


        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        m_hotCnt = EditorGUILayout.TextField("热更补丁版本：", m_hotCnt, GUILayout.Width(350), GUILayout.Height(20));//文本框
        GUILayout.EndHorizontal();
        if (GUILayout.Button("开始打热更包", GUILayout.Width(100), GUILayout.Height(50)))//按钮
        {
            if (false==string.IsNullOrEmpty(m_abMD5_OutterPath) && true==m_abMD5_OutterPath.EndsWith(".bytes"))
            {
                AssetBundleHotFixEditor.BuildAB(true, m_abMD5_OutterPath, m_hotCnt);
            }
        }
    }


    public static void GetWnd()
    {
        AssetBundleHotFixEWnd window = (AssetBundleHotFixEWnd)GetWindow(typeof(AssetBundleHotFixEWnd), false, "热更包界面", true);
        window.Show();
    }
}
