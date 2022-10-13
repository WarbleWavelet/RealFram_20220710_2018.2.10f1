/****************************************************
    文件：RealFramConfig.cs
	作者：lenovo
    邮箱: 
    日期：2022/8/7 13:33:21
	功能：RealFrame总的路径配置
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;




[CreateAssetMenu(fileName = "RealFrameCfg", menuName = DefinePath.Assets_MyAssets + "Create RealFrameCfg", order = 0)] //Constants.MenuAssets="My Assets"
public class RealFrameCfgSO : ScriptableObject
{
    //打包时生成AB包配置表的二进制路径
    public string m_ABBinPath;
    //xml文件夹路径
    public string m_XmlPath;
    //二进制文件夹路径
    public string m_BinPath;
    //脚本文件夹路径
    public string m_ScriptsPath;
    //Protobuf
    public string m_ProtobufPath;
}

[CustomEditor(typeof(RealFrameCfgSO))]
public class RealFramConfigInspector : Editor
{
    public SerializedProperty m_ABBinPath;
    public SerializedProperty m_XmlPath;
    public SerializedProperty m_BinPath;
    public SerializedProperty m_ScriptsPath;
    public SerializedProperty m_ProtobufPath;

    private void OnEnable()
    {
        m_ABBinPath = serializedObject.FindProperty("m_ABBinPath");
        m_XmlPath = serializedObject.FindProperty("m_XmlPath");
        m_BinPath = serializedObject.FindProperty("m_BinPath");
        m_ScriptsPath = serializedObject.FindProperty("m_ScriptsPath");
        m_ProtobufPath = serializedObject.FindProperty("m_ProtobufPath");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_ABBinPath, new GUIContent("ab包二进制路径"));
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(m_XmlPath, new GUIContent("Xml路径"));
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(m_BinPath, new GUIContent("二进制路径"));
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(m_ScriptsPath, new GUIContent("配置表脚本路径"));
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(m_ProtobufPath, new GUIContent("Protobuf配置表路径"));
        GUILayout.Space(5);
        serializedObject.ApplyModifiedProperties();


    }
}

public class RealFrameCfg
{
    private const string RealFramPath = DefinePath.RealFrameCfgSOPath;

    public static RealFrameCfgSO GetRealFram()
    {
        RealFrameCfgSO realConfig = AssetDatabase.LoadAssetAtPath<RealFrameCfgSO>(RealFramPath);
        return realConfig;
    }
}
