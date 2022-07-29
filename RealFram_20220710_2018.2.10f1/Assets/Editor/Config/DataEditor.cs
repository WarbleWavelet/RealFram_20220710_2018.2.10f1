/****************************************************
    文件：DataEditor.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/28 22:57:57
	功能：
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DataEditor
{
    const string m_path_Xml="Assets/GameData/Data/Xml/";
    const string m_path_Bin="Assets/GameData/Data/Bin/";
    const string m_path_Scripts = "Assets/Scripts/Data/";

   [MenuItem(  "Assets/My Assets/Class2Xml", false, 0)]//按钮在菜单栏的位置
    static void Class2Xml()
    {

        UnityEngine.Object[] objArr = Selection.objects;

        for (int i = 0; i < objArr.Length; i++)
        {



                UnityEngine.Object obj = objArr[i];
                string title = "正在转成Xml";
                string info = "";
                info += "正在转化" + obj.name + "....";
                float prg = (1.0f * i) / objArr.Length; ;
                EditorUtility.DisplayCancelableProgressBar(title, info, prg);
                Name2Class2Xml(obj.name);
    

      

        }    
        
        
         AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();

    }




   /// <summary>
   /// 数量1，防止点错了，污染旧数据
   /// </summary>
   /// <param name="name"></param>
  static  void Name2Class2Xml(string name)
    {
        Type type = null;
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type _type = asm.GetType(name);
            if (_type != null)
            {
                type = _type;
                break;
            }
        }

        if (type != null)
        {
            var temp = Activator.CreateInstance(type);
            if (temp is ExcelBase)
            {
                (temp as ExcelBase).Construction();
            }
            string xmlPath = m_path_Xml + name + ".xml";
            FormatTool.Xml2Class(xmlPath, temp);
            Debug.Log(name + "类转xml成功，xml路径为:" + xmlPath);
        }
    }
}
