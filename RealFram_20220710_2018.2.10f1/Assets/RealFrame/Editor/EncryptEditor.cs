/****************************************************
    文件：EncryptEditor.cs
	作者：lenovo
    邮箱: 
    日期：2022/9/30 9:34:25
	功能：加密/加密
			01 AES
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;



                                       

public class EncryptEditor : Editor
{

    static string m_EncryptPath = DefinePath.RealFramePath + "GameData/Data/Xml/";  //用来测试的一个文件
  public  const string m_PrivateKey = Constants.PrivateKey;   //密钥
    static string m_BunleTargetPath=DefinePath.OutputAB_OutterPath;


    #region MenuItem


    [MenuItem(DefinePath.MenuItem_AB + "测试AES加密", false, DefinePath.MenuItem_Index_AB_Encrypt)]//按钮在菜单栏的位置
     static void MenuItem_Encrypt()
    {
        AES.AESFileEncrypt(m_EncryptPath + "Test_AESData.xml", m_PrivateKey);
    }

    [MenuItem(DefinePath.MenuItem_AB + "测试AES解密", false, DefinePath.MenuItem_Index_AB_Encrypt)]//按钮在菜单栏的位置
     static void MenuItem_Decrypt()
    {
        AES.AESFileDecrypt(m_EncryptPath + "Test_AESData.xml", m_PrivateKey);
    }


    [MenuItem(DefinePath.MenuItem_AB + "加密AB", false, DefinePath.MenuItem_Index_AB_Encrypt)]//按钮在菜单栏的位置
     static void MenuItem_Encrypt_AB()
    {
        EncryptAB(m_BunleTargetPath,m_PrivateKey);

    }



    [MenuItem(DefinePath.MenuItem_AB + "解密AB", false, DefinePath.MenuItem_Index_AB_Encrypt)]//按钮在菜单栏的位置
     static void MenuItem_Decrypt_AB()
    {
        DecryptAB(m_BunleTargetPath, m_PrivateKey);
    }

    #endregion  

  

    public static void EncryptAB(string path, string privateKey)
    {
        FileInfo[] files = Common.Folder_GetAllFileInfo(path);
        for (int i = 0; i < files.Length; i++)
        {
            if (!files[i].Name.EndsWith("meta") && !files[i].Name.EndsWith(".manifest"))
            {
                AES.AESFileEncrypt(files[i].FullName, privateKey);
            }
        }
        Debug.LogFormat("加密完成！\n{0}", path);
    }

    public static void DecryptAB(string path, string privateKey)
    {
        FileInfo[] files = Common.Folder_GetAllFileInfo(path);
        for (int i = 0; i < files.Length; i++)
        {
            if (!files[i].Name.EndsWith("meta") && !files[i].Name.EndsWith(".manifest"))
            {
                AES.AESFileDecrypt(files[i].FullName, privateKey);
            }
        }
        Debug.LogFormat("解密完成！\n{0}", path);
    }
}
