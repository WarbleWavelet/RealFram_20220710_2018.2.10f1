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
//using UnityAction=UnityEngine.Events.UnityAction;//没用，适配的时候

#region 委托


public delegate void Delegate_Void(int a);
public delegate string Delegate_String(int a);

#endregion

public class ILRuntimeMgr : Singleton<ILRuntimeMgr>
{


    #region 字属


    string m_path_HotFixDll = DefinePath.Path_HotFixDll_Txt;            //读取热更资源的dll
    string m_path_HotFixPdb = DefinePath.Path_HotFixPdb_Txt;            //读取热更资源的pdb
   static AppDomain m_AppDomain;


    #region 委托 


    public Delegate_Void delegate_Void; //主程定义，ILRunTime调用
    public Delegate_String delegate_String;
    public System.Action<string> action_String;
    #endregion  


    #region 类.方法


    const string m_NameSpaceClass1 = "HotFix.Class1";
     const string m_Method_11 = "Test_StaticFunction"; //第一个1是命名空间的.类名的最后数字
     const string m_Method_12 = "Test_GenericFunction";
    #endregion


    #region 类.方法


     const string m_NameSpaceClass2 = "HotFix.Test_Delegate";
     const string m_Method_211 = "Awake1";
     const string m_Method_212 = "Awake2";
     const string m_Method_221 = "Start1";
     const string m_Method_222 = "Start2";
    #endregion
    #endregion  



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
        // OnHotFixLoaded_Test01();
        // OnHotFixLoaded_Test02();
        // OnHotFixLoaded_Test03();
        // OnHotFixLoaded_Test04();
        // OnHotFixLoaded_Test05();
        // OnHotFixLoaded_Test06();
        // OnHotFixLoaded_Test07();
        // OnHotFixLoaded_Test08();
        // OnHotFixLoaded_Test09();
        // OnHotFixLoaded_Test10();
         OnHotFixLoaded_Test11();
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

    private void InitializeILRuntime()
    {
        RegisterAdapter();
    }

    /// <summary>
    /// IL2CPP的AOT技术无法在运行时生成新的类型，所以要注册适配器
    /// 同参（数量，类型）定义一次即可
    /// </summary>
    void RegisterAdapter()
    {
        RegisterAdapter_Action();
        RegisterAdapter_Delegate();
        RegisterAdapter_UnityAction();
    }


    /// <summary>
    /// 注册适配器：默认委托注册仅仅支持系统自带的Action以及Function
    /// </summary>
    public void RegisterAdapter_Action()
    {
        m_AppDomain.DelegateManager.RegisterMethodDelegate<string>();//本身是Action<string>,不需要适配
    }


    /// <summary>
    /// 注册适配器：自定义委托或Unity委托注册
    /// </summary>
    public void RegisterAdapter_Delegate()
    {   
        m_AppDomain.DelegateManager.RegisterMethodDelegate<int>(); //配套的
        m_AppDomain.DelegateManager.RegisterDelegateConvertor<Delegate_Void>((action) =>
        {
            return new Delegate_Void((a) =>  //转换器
            {
                ((System.Action<int>)action)(a); //<参数>
            });
        });

        m_AppDomain.DelegateManager.RegisterFunctionDelegate<int, string>();
        m_AppDomain.DelegateManager.RegisterDelegateConvertor<Delegate_String>((action) =>
        {
            return new Delegate_String((a) =>  
            {
                return ((System.Func<int, string>)action)(a);//<参数,返回值>
            });
        });
    }

    public void RegisterAdapter_UnityAction()
    {
        m_AppDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<bool>>((action) =>
        {
            return new UnityEngine.Events.UnityAction<bool>((a) =>
            {
                ((System.Action<bool>)action)(a);
            });
        });

        m_AppDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((action) =>
        {
            return new UnityEngine.Events.UnityAction(() =>
            {
                ((System.Action)action)();
            });
        });
    }

    private void OnHotFixLoaded_Test01()
    {

        #region 源程序
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
        #endregion

        //m_AppDomain.Invoke( m_NameSpaceClass, m_Method1,null,null);//
        AppDomain_Invoke(m_AppDomain, m_NameSpaceClass1, m_Method_11, null,null);
    }




    private void OnHotFixLoaded_Test02()
    {
        IType type = m_AppDomain.LoadedTypes[m_NameSpaceClass1]; //先单独获取类，之后一直使用这个类来调用

        IMethod method = type.GetMethod(m_Method_11, 0);//根据方法名称和参数个数获取方法(学习获取函数进行调用)
        m_AppDomain.Invoke(method, null, null);
    }


    /// <summary>
    ///  第一种含参调用
    /// </summary>
    private void OnHotFixLoaded_Test03()
    {
        IType type = m_AppDomain.LoadedTypes[m_NameSpaceClass1]; //先单独获取类，之后一直使用这个类来调用

        IMethod method = type.GetMethod(m_Method_11, 1); //根据获取函数来调用有参的函数
        m_AppDomain.Invoke(method, null, 5);
        //m_AppDomain.Invoke(method, null, null); //Err 参数不匹配，写了1个，结果为null
    }

    /// <summary>
    /// 第二种含参调用
    /// </summary>
    private void OnHotFixLoaded_Test04()
    {
        IType type = m_AppDomain.LoadedTypes[m_NameSpaceClass1]; //先单独获取类，之后一直使用这个类来调用

        IType intType = m_AppDomain.GetType(typeof(int));
        List<IType> paraList = new List<IType>();
        paraList.Add(intType);
        IMethod method = type.GetMethod(m_Method_11, paraList, null); 
        m_AppDomain.Invoke(method, null, 5);
    }

    /// <summary>
    /// 实例化热更工程里的类
    ///  第一种实例化(可以带参数)
    /// </summary>
    private void OnHotFixLoaded_Test05()
    {
        object obj = m_AppDomain.Instantiate(m_NameSpaceClass1, new object[] { 15 });
    }

    /// <summary>
    /// 第二种实例化（不带参数）
    /// </summary>
    private void OnHotFixLoaded_Test06()
    {
        IType type = m_AppDomain.LoadedTypes[m_NameSpaceClass1];

        object obj = ((ILType)type).Instantiate();
        int id = (int)m_AppDomain.Invoke(m_NameSpaceClass1, "get_ID", obj, null);//实例
    }
   /// <summary>
   /// 第一种泛型方法调用
   /// </summary>
    private void OnHotFixLoaded_Test07()
    {
        IType type = m_AppDomain.LoadedTypes[m_NameSpaceClass1];

        IType stringType = m_AppDomain.GetType(typeof(string));
        IType[] genericArguments = new IType[] { stringType };
        m_AppDomain.InvokeGenericMethod(m_NameSpaceClass1, m_Method_12, genericArguments, null, "Demasia");
    }

    /// <summary>
    /// 第二种泛型方法调用
    /// </summary>
    private void OnHotFixLoaded_Test08()
    {
        IType type = m_AppDomain.LoadedTypes[m_NameSpaceClass1];

        IType stringType = m_AppDomain.GetType(typeof(string));
        IType[] genericArguments = new IType[] { stringType };
        List<IType> paraList = new List<IType>();
        paraList.Add(stringType);
        IMethod method = type.GetMethod(m_Method_12, paraList, genericArguments);
        m_AppDomain.Invoke(method, null, "Demasia");

    }

    /// <summary>
    /// 委托调用之一：热更内部(热更域定义，热更域使用)
    /// </summary>
    private void OnHotFixLoaded_Test09()
    {
        AppDomain_Invoke(m_NameSpaceClass2,m_Method_211,null,null);
        AppDomain_Invoke(m_NameSpaceClass2,m_Method_221,null,null);

    }

    /// <summary>
    /// /委托调用之二：主程域定义，热更域使用
    /// </summary>
    private void OnHotFixLoaded_Test10()
    {
        AppDomain_Invoke(m_NameSpaceClass2, m_Method_212, null, null);
        AppDomain_Invoke(m_NameSpaceClass2, m_Method_222, null, null);

    }

    /// <summary>
    /// 热更域定义，主程调用
    /// </summary>
    private void OnHotFixLoaded_Test11()
    {
        AppDomain_Invoke(m_NameSpaceClass2, m_Method_212, null, null);//热更域定义

        if (delegate_Void != null)
        {
            delegate_Void(64);
        }

        if (delegate_String != null)
        {
            delegate_String(64);
        }

        if (action_String != null)
        {
            action_String("64");
        }
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

    public static void AppDomain_Invoke( string type, string method, object instance, params object[] p)
    {
        m_AppDomain.Invoke(type, method, instance, p); //m_AppDomain.Invoke(m_NameSpaceClass, m_Method, null, null);
    }
    #endregion

}


