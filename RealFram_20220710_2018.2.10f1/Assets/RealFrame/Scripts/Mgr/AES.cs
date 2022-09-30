/****************************************************

	文件：
	作者：WWS
	日期：2022/09/29 23:52:23
	功能：AES加密

*****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;




public class AES
{
    private static string m_AESHead = "AESEncrypt";   //加密头



    #region 加密



   /// <summary>
    /// 文件加密，传入文件路径
    /// </summary>
    /// <param name="path"></param>
    /// <param name="EncrptyKey"></param>
    public static void AESFileEncrypt(string path, string EncrptyKey)
    {

        if (!Common.File_Exits(path))
        { 
           return;
        }
           


        try
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (fs != null)
                {
                   string headTag= Common.FileStream_Read(fs,10);   //读取字节头，判断是否已经加密过了
                   
                    if (headTag == m_AESHead)
                    {
#if UNITY_EDITOR
                        Debug.LogFormat(  "已经加密过了！{0}",path);
#endif
                        return;
                    }

                    byte[] buffer= Common.FileStream_Read(fs );
                     Common.FileStream_Write( fs, m_AESHead);
                    byte[] EncBuffer = AESEncrypt(buffer, EncrptyKey);      //内容+密钥
                    Common.FileStream_Write(fs, EncBuffer);               //写入密钥+内容
                     Debug.LogFormat(  "加密成功！{0}",path);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("加密失败！{0}\n{1}", path,e);
        }
    }

    #endregion



    #region 解密


    /// <summary>
    /// 文件解密，传入文件路径（会改动加密文件，不适合运行时）
    /// </summary>
    /// <param name="path"></param>
    /// <param name="EncrptyKey"></param>
    public static void AESFileDecrypt(string path, string EncrptyKey)
    {
        if (!Common.File_Exits(path))
        {
            return;
        }

        try
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                if (fs != null)
                {
                    byte[] headBuff = new byte[10];
                    string headTag = Common.FileStream_Read(fs, headBuff.Length);
                    if (headTag == m_AESHead)
                    {
                        byte[] buffer =  Common.FileStream_Read(fs, (long)headBuff.Length,fs.Length);
                        byte[] DecBuffer = AESDecrypt(buffer, EncrptyKey);
                        Common.FileStream_Write(fs,DecBuffer);
                        Debug.LogFormat("解密成功！{0}", path);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("解密失败！{0}\n{1}", path, e);
        }
    }

    /// <summary>
    /// 文件界面，传入文件路径，返回字节（AB包主用）
    /// </summary>
    /// <returns></returns>
    public static byte[] AESFileByteDecrypt(string path, string EncrptyKey)
    {
        if (!Common.File_Exits(path))
        {
            return null;
        }
        byte[] DecBuffer = null;
        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (fs != null)
                {
                    byte[] headBuff = new byte[10];
                    string headTag = Common.FileStream_Read(fs, headBuff);
                    if (headTag == m_AESHead)
                    {
                        byte[] buffer = new byte[fs.Length - headBuff.Length];
                        Common.FileStream_Write(fs,buffer,0, (fs.Length - headBuff.Length) );
                        DecBuffer = AESDecrypt(buffer, EncrptyKey);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogError(e);
        }

        return DecBuffer;
    }

    #endregion




    #region 一开始就有的



  /// <summary>
    /// AES 加密(高级加密标准，是下一代的加密算法标准，速度快，安全级别高，目前 AES 标准的一个实现是 Rijndael 算法)
    /// </summary>
    /// <param name="EncryptString">待加密密文</param>
    /// <param name="EncryptKey">加密密钥</param>
    public static string AESEncrypt(string EncryptString, string EncryptKey)
    {
        return Convert.ToBase64String(AESEncrypt(Encoding.Default.GetBytes(EncryptString), EncryptKey));
    }

    /// <summary>
    /// AES 加密(高级加密标准，是下一代的加密算法标准，速度快，安全级别高，目前 AES 标准的一个实现是 Rijndael 算法)
    /// </summary>
    /// <param name="EncryptString">待加密密文</param>
    /// <param name="EncryptKey">加密密钥</param>
    public static byte[] AESEncrypt(byte[] EncryptByte, string EncryptKey)
    {
        if (EncryptByte.Length == 0) { throw (new Exception("明文不得为空")); }
        if (string.IsNullOrEmpty(EncryptKey)) { throw (new Exception("密钥不得为空")); }
        byte[] m_strEncrypt;
        byte[] m_btIV = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");
        byte[] m_salt = Convert.FromBase64String("gsf4jvkyhye5/d7k8OrLgM==");
        Rijndael m_AESProvider = Rijndael.Create();
        try
        {
            MemoryStream m_stream = new MemoryStream();
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(EncryptKey, m_salt);
            ICryptoTransform transform = m_AESProvider.CreateEncryptor(pdb.GetBytes(32), m_btIV);
            CryptoStream m_csstream = new CryptoStream(m_stream, transform, CryptoStreamMode.Write);
            m_csstream.Write(EncryptByte, 0, EncryptByte.Length);
            m_csstream.FlushFinalBlock();
            m_strEncrypt = m_stream.ToArray();
            m_stream.Close(); m_stream.Dispose();
            m_csstream.Close(); m_csstream.Dispose();
        }
        catch (IOException ex) { throw ex; }
        catch (CryptographicException ex) { throw ex; }
        catch (ArgumentException ex) { throw ex; }
        catch (Exception ex) { throw ex; }
        finally { m_AESProvider.Clear(); }
        return m_strEncrypt;
    }


    /// <summary>
    /// AES 解密(高级加密标准，是下一代的加密算法标准，速度快，安全级别高，目前 AES 标准的一个实现是 Rijndael 算法)
    /// </summary>
    /// <param name="DecryptString">待解密密文</param>
    /// <param name="DecryptKey">解密密钥</param>
    public static string AESDecrypt(string DecryptString, string DecryptKey)
    {
        return Convert.ToBase64String(AESDecrypt(Encoding.Default.GetBytes(DecryptString), DecryptKey));
    }

    /// <summary>
    /// AES 解密(高级加密标准，是下一代的加密算法标准，速度快，安全级别高，目前 AES 标准的一个实现是 Rijndael 算法)
    /// </summary>
    /// <param name="DecryptString">待解密密文</param>
    /// <param name="DecryptKey">解密密钥</param>
    public static byte[] AESDecrypt(byte[] DecryptByte, string DecryptKey)
    {
        if (DecryptByte.Length == 0) { throw (new Exception("密文不得为空")); }
        if (string.IsNullOrEmpty(DecryptKey)) { throw (new Exception("密钥不得为空")); }
        byte[] m_strDecrypt;
        byte[] m_btIV = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");
        byte[] m_salt = Convert.FromBase64String("gsf4jvkyhye5/d7k8OrLgM==");
        Rijndael m_AESProvider = Rijndael.Create();
        try
        {
            MemoryStream m_stream = new MemoryStream();
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(DecryptKey, m_salt);
            ICryptoTransform transform = m_AESProvider.CreateDecryptor(pdb.GetBytes(32), m_btIV);
            CryptoStream m_csstream = new CryptoStream(m_stream, transform, CryptoStreamMode.Write);
            m_csstream.Write(DecryptByte, 0, DecryptByte.Length);
            m_csstream.FlushFinalBlock();
            m_strDecrypt = m_stream.ToArray();
            m_stream.Close(); m_stream.Dispose();
            m_csstream.Close(); m_csstream.Dispose();
        }
        catch (IOException ex) { throw ex; }
        catch (CryptographicException ex) { throw ex; }
        catch (ArgumentException ex) { throw ex; }
        catch (Exception ex) { throw ex; }
        finally { m_AESProvider.Clear(); }
        return m_strDecrypt;
    }
    #endregion  

  


}
