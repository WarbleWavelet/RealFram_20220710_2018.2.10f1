/****************************************************
    文件：ILRuntimeEditor.cs
	作者：lenovo
    邮箱: 
    日期：2022/10/4 19:24:45
	功能：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ILRuntimeEditor : Editor
{
   static   string m_Path_HotFix = DefinePath.Path_HotFix;
   static string m_path_HotFixDll = DefinePath.Path_HotFixDll;            //读取热更资源的dll
   static string m_path_HotFixPdb = DefinePath.Path_HotFixPdb;            //读取热更资源的pdb

    [MenuItem(DefinePath.MenuItem_AB + "路径下清空.txt(RealFrame\\GameData\\Data\\HotFix\\)", false, DefinePath.MenuItem_Index_AB_ILRuntime)]
    static void MenuItem_Clear()
    {

         Common.Folder_Clear_Recursive(m_Path_HotFix);
    }
        
        [MenuItem(DefinePath.MenuItem_AB + "路径下文件加.txt(RealFrame\\GameData\\Data\\HotFix\\)", false, DefinePath.MenuItem_Index_AB_ILRuntime)]
     static void MenuItem_ChangeDllName()
    {
      
        Common.File_Move_Suffix(m_path_HotFixDll, ".txt");
        Common.File_Move_Suffix(m_path_HotFixPdb, ".txt");

        Common.Refresh();
        Common.Selection_ActiveObject(m_path_HotFixDll);
    }
}
