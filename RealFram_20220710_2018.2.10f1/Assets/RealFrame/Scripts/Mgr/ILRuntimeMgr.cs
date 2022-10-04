/****************************************************
    文件：ILRuntimeMgr.cs
	作者：lenovo
    邮箱: 
    日期：2022/10/3 23:37:15
	功能：01 获取dll,引用程序集的方法
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ILRuntime.Runtime.Enviorment;
using System.IO;

public class ILRuntimeMgr : Singleton<ILRuntimeMgr>
{
    string m_path_HotFixDll = DefinePath.Path_HotFixDll_Txt;            //读取热更资源的dll
    string m_path_HotFixPdb = DefinePath.Path_HotFixPdb_Txt;            //读取热更资源的pdb
    AppDomain m_AppDomain;

    public void InitMgr()
    {
        LoadHotFixAssembly(ref m_AppDomain, m_path_HotFixDll, m_path_HotFixPdb);
    }



    #region 辅助
    void LoadHotFixAssembly( string m_path_HotFixDll, string m_path_HotFixPdb)
    {
        m_AppDomain = new AppDomain(); //全局唯一
        TextAsset ta_dll = ResourceMgr.Instance.LoadResource<TextAsset>(m_path_HotFixDll);
        TextAsset ta_pdb = ResourceMgr.Instance.LoadResource<TextAsset>(m_path_HotFixPdb); //PBD文件，调试数据可，日志报错

        using (MemoryStream ms_dll = new MemoryStream(ta_dll.bytes))
        {
            using (MemoryStream ms_pdb = new MemoryStream(ta_pdb.bytes))
            {
                m_AppDomain.LoadAssembly(ms_dll, ms_pdb, new Mono.Cecil.Pdb.PdbReaderProvider()); //载入程序集
            }
        }

        InitializeILRuntime();
        OnHotFixLoaded();
    }


    void LoadHotFixAssembly(ref AppDomain m_AppDomain,string m_path_HotFixDll, string m_path_HotFixPdb)
    {
        m_AppDomain = new AppDomain(); //全局唯一
        TextAsset ta_dll = ResourceMgr.Instance.LoadResource<TextAsset>(m_path_HotFixDll);
        TextAsset ta_pdb = ResourceMgr.Instance.LoadResource<TextAsset>(m_path_HotFixPdb); //PBD文件，调试数据可，日志报错

        using (MemoryStream ms_dll = new MemoryStream(ta_dll.bytes))
        {
            using (MemoryStream ms_pdb = new MemoryStream(ta_pdb.bytes))
            {
                m_AppDomain.LoadAssembly(ms_dll, ms_pdb, new Mono.Cecil.Pdb.PdbReaderProvider()); //载入程序集
            }
        }

        InitializeILRuntime();
        OnHotFixLoaded();
    }

    private void OnHotFixLoaded()
    {
        /*
        namespace HoitFix
        {
            public class Class1
            {
                public static void Test_StaticFunction()
                {
                   UnityEngine.Debug.Log("gsdkh");
                }
            }
        }
        */
       //m_AppDomain.Invoke( "HotFix.Class1", "Test_StaticFunction",null,null);//
        AppDomain_Invoke(m_AppDomain, "HotFix.Class1", "Test_StaticFunction",null,null);
        //AppDomain_Invoke(m_AppDomain, "HotFix.Class1", "HotFix.Class1", null,null);//
    }

    private void InitializeILRuntime()
    {
        
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="appDomain"></param>
    /// <param name="type"></param>
    /// <param name="method"></param>
    /// <param name="instance">是否实例</param>
    /// <param name="p">参数</param>
    public static void AppDomain_Invoke(AppDomain appDomain, string type, string method,object instance,params object[] p)
    {
        appDomain.Invoke(type,method, instance, p); //m_AppDomain.Invoke("HotFix.Class1", "Test_StaticFunction", null, null);
    }
    #endregion
  
}


