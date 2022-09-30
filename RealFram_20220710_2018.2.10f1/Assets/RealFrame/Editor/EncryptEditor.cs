/****************************************************
    文件：EncryptEditor.cs
	作者：lenovo
    邮箱: 
    日期：2022/9/30 9:34:25
	功能：加密
			01 AES
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;





public class EncryptEditor : Editor
{

    static string m_EncryptPath = DefinePath.RealFramePath + "GameData/Data/Xml/";

    [MenuItem(DefinePath.MenuItem_AB + "测试加密", false, DefinePath.MenuItem_Index_AB_Encrypt)]//按钮在菜单栏的位置
    public static void MenuItem_Encrypt()
    {
        AES.AESFileEncrypt(m_EncryptPath+ "Test_AESData.xml","WWS");
    }

}
