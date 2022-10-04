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
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;

public class ILRuntimeMgr : Singleton<ILRuntimeMgr>
{
    string m_path_HotFixDll = DefinePath.Path_HotFixDll_Txt;            //读取热更资源的dll
    string m_path_HotFixPdb = DefinePath.Path_HotFixPdb_Txt;            //读取热更资源的pdb
    AppDomain m_AppDomain;

 const string m_NameSpaceClass = "HotFix.Class1";
 const string m_Method = "Test_StaticFunction";

    #region 生命
    public void InitMgr()
    {
        LoadHotFixAssembly();
    }

    void LoadHotFixAssembly()
    {
        LoadHotFixAssembly_Test1( m_path_HotFixDll, m_path_HotFixPdb);
        //LoadHotFixAssembly_Test2(ref m_AppDomain, m_path_HotFixDll, m_path_HotFixPdb);

    }
    private void OnHotFixLoaded()
    {
       // OnHotFixLoaded_Test1();
       // OnHotFixLoaded_Test2();
        //OnHotFixLoaded_Test3();
        OnHotFixLoaded_Test4();
    }
    #endregion





    #region 辅助
    void LoadHotFixAssembly_Test1( string m_path_HotFixDll, string m_path_HotFixPdb)
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


    void LoadHotFixAssembly_Test2(ref AppDomain m_AppDomain,string m_path_HotFixDll, string m_path_HotFixPdb)
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


    private void OnHotFixLoaded_Test1()
    {
        /*  源程序
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
        //m_AppDomain.Invoke( m_NameSpaceClass, m_Method,null,null);//
        AppDomain_Invoke(m_AppDomain, m_NameSpaceClass, m_Method, null,null);
    }




    private void OnHotFixLoaded_Test2()
    {
        IType type = m_AppDomain.LoadedTypes[m_NameSpaceClass]; //先单独获取类，之后一直使用这个类来调用

        IMethod method = type.GetMethod(m_Method, 0);//根据方法名称和参数个数获取方法(学习获取函数进行调用)
        m_AppDomain.Invoke(method, null, null);
    }


    /// <summary>
    ///  第一种含参调用
    /// </summary>
    private void OnHotFixLoaded_Test3()
    {
        IType type = m_AppDomain.LoadedTypes[m_NameSpaceClass]; //先单独获取类，之后一直使用这个类来调用

        IMethod method = type.GetMethod(m_Method, 1); //根据获取函数来调用有参的函数
        m_AppDomain.Invoke(method, null, 5);
        //m_AppDomain.Invoke(method, null, null); //Err 参数不匹配，写了1个，结果为null
    }

    /// <summary>
    /// 第二种含参调用
    /// </summary>
    private void OnHotFixLoaded_Test4()
    {
        IType type = m_AppDomain.LoadedTypes[m_NameSpaceClass]; //先单独获取类，之后一直使用这个类来调用

        IType intType = m_AppDomain.GetType(typeof(int));
        List<IType> paraList = new List<IType>();
        paraList.Add(intType);
        IMethod method = type.GetMethod(m_Method, paraList, null); 
        m_AppDomain.Invoke(method, null, 5);
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
        appDomain.Invoke(type,method, instance, p); //m_AppDomain.Invoke(m_NameSpaceClass, m_Method, null, null);
    }
    #endregion
  
}


