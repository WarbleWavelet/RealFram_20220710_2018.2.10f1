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
using ILRuntime.Runtime.Intepreter;
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

    const string m_NameSpaceClass3 = "HotFix.Test_Inheritance";
    const string m_Method_31 = "NewObj";
    #endregion  



    #region 生命
    public void InitMgr()
    {
        LoadHotFixAssembly();
    }

    void LoadHotFixAssembly()
    {
        LoadHotFixAssembly_Test1(m_path_HotFixDll, m_path_HotFixPdb);
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
        // OnHotFixLoaded_Test11();
        OnHotFixLoaded_Test12();
        OnHotFixLoaded_Test13();
    }
    #endregion





    #region 辅助
    void LoadHotFixAssembly_Test1(string m_path_HotFixDll, string m_path_HotFixPdb)
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


    void LoadHotFixAssembly_Test2(ref AppDomain m_AppDomain, string m_path_HotFixDll, string m_path_HotFixPdb)
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
        RegisterAdapter_Inheritance();
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
    /// <summary>
    /// 跨域继承的注册
    /// </summary>
    public void RegisterAdapter_Inheritance()
    {
        m_AppDomain.RegisterCrossBindingAdaptor(new InheritanceAdapter());
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
    /// 跨域继承之一
    /// </summary>
    private void OnHotFixLoaded_Test12()
    {
        Test_ClassBase obj = m_AppDomain.Instantiate<Test_ClassBase>(m_NameSpaceClass3);
        obj.TestAbstract(556);
        obj.TestVirtual("Ocean");
    }

    /// <summary>
    /// 跨域继承之二
    /// </summary>
    private void OnHotFixLoaded_Test13()
    {
        Test_ClassBase obj = m_AppDomain.Invoke(m_NameSpaceClass3, m_Method_31, null, null) as Test_ClassBase;
        obj.TestAbstract(721);
        obj.TestVirtual("Ocean123");
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



#region 类


/// <summary>
/// 跨域继承的一个抽象类
/// </summary>
public abstract class Test_ClassBase
{
    public virtual int Value
    {
        get { return 0; }
    }

    public virtual void TestVirtual(string str)
    {
        Debug.LogFormat("Test_ClassBase.TestVirtual(str:{0}) " ,str);
    }

    public abstract void TestAbstract(int a);
}



#endregion


#region 类 配置器


public class InheritanceAdapter : CrossBindingAdaptor
{
    public override System.Type BaseCLRType
    {
        get
        {
            return typeof(Test_ClassBase);//想继承的类
        }
    }

    public override System.Type AdaptorType
    {
        get
        {
            return typeof(Adapter);//实际的适配器类
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adapter(appdomain, instance);
    }

   /// <summary>
   /// 适配器类
   /// </summary>
    class Adapter : Test_ClassBase, CrossBindingAdaptorType
    {
        #region 字属  构造


        private ILRuntime.Runtime.Enviorment.AppDomain m_Appdomain; //
        private ILTypeInstance m_Instance;                          //
        private IMethod m_TestAbstract;                             //
        private IMethod m_TestVirtual;                              //
        private IMethod m_GetValue;                                 //
        private IMethod m_ToString;                                 //
        object[] param1 = new object[1];                            //任何类型的变量
        private bool m_TestVirtualInvoking = false;                 //是否正在执行虚方法。必须要设定一个标识位来表示当前是否在调用中，否则如果脚本类里调用了base.TestVirtual()就会造成无限循环
        private bool m_GetValueInvoking = false;                    //正在获取值

        public ILTypeInstance ILInstance
        {
            get
            {
                return m_Instance;
            }
        }

        public Adapter()
        {

        }

        public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            m_Appdomain = appdomain;
            m_Instance = instance;
        }
        #endregion


        #region override 4个方法


        #region 类的两个方法


    //在适配器中重写所有需要在热更脚本重写的方法，并且将控制权转移到脚本里去（主程=>热更域）
        public override void TestAbstract(int a)
        {
            if (m_TestAbstract == null)
            {
                m_TestAbstract = m_Instance.Type.GetMethod("TestAbstract", 1);
            }

            if (m_TestAbstract != null)
            {
                param1[0] = a;
                m_Appdomain.Invoke(m_TestAbstract, m_Instance, param1);//转移
            }
        }

        public override void TestVirtual(string str)
        {
            if (m_TestVirtual == null)
            {
                m_TestVirtual = m_Instance.Type.GetMethod("TestVirtual", 1);
            }

            
            if (m_TestVirtual != null && !m_TestVirtualInvoking)  
            {
                m_TestVirtualInvoking = true;
                param1[0] = str;
                m_Appdomain.Invoke(m_TestVirtual, m_Instance, param1);//方法，实例，变量
                m_TestVirtualInvoking = false;
            }
            else
            {
                base.TestVirtual(str);//执行
            }
        }
        #endregion
      

        public override int Value
        {
            get
            {
                if (m_GetValue == null)
                {
                    m_GetValue = m_Instance.Type.GetMethod("get_Value", 1);//get_ 属性编译后会变成前面加get_的方法
                }

                if (m_GetValue != null && !m_GetValueInvoking)
                {
                    m_GetValueInvoking = true;
                    int res = (int)m_Appdomain.Invoke(m_GetValue, m_Instance, null);
                    m_GetValueInvoking = false;
                    return res;
                }
                else
                {
                    return base.Value;
                }
            }
        }

        /// <summary>
        /// 理论上在每个适配器是固定的
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (m_ToString == null)
            {
                m_ToString = m_Appdomain.ObjectType.GetMethod("ToString", 0);//ObjectType基本方法
            }
            IMethod m = m_Instance.Type.GetVirtualMethod(m_ToString);
            if (m == null || m is ILMethod)
            {
                return m_Instance.ToString();
            }
            else
            {
                return m_Instance.Type.FullName;
            }
        }
        #endregion
       
    }
}
#endregion



#region 说明
public class CoroutineAdapter : CrossBindingAdaptor
{
    public override System.Type BaseCLRType
    {
        get
        {
            return null;
        }
    }

    public override System.Type AdaptorType
    {
        get
        {
            return typeof(Adaptor);
        }
    }

    public override System.Type[] BaseCLRTypes
    {
        get
        {
            return new System.Type[] { typeof(IEnumerator<object>), typeof(IEnumerator), typeof(System.IDisposable) };
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }

    public class Adaptor : IEnumerator<System.Object>, IEnumerator, System.IDisposable, CrossBindingAdaptorType
    {
        private ILTypeInstance m_Instance;
        private ILRuntime.Runtime.Enviorment.AppDomain m_Appdamain;
        private IMethod m_CurMethod;
        private IMethod m_DisposeMethod;
        private IMethod m_MoveNextMethod;
        private IMethod m_ResetMethod;
        private IMethod m_ToString;

        public Adaptor()
        {

        }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            m_Instance = instance;
            m_Appdamain = appdomain;
        }


        public object Current
        {
            get
            {
                if (m_CurMethod == null)
                {
                    m_CurMethod = m_Instance.Type.GetMethod("get_Current", 0);
                    if (m_CurMethod == null)
                    {
                        m_CurMethod = m_Instance.Type.GetMethod("System.Collections.IEnumerator.get_Current", 0);
                    }
                }

                if (m_CurMethod != null)
                {
                    var res = m_Appdamain.Invoke(m_CurMethod, m_Instance, null);
                    return res;
                }
                else
                {
                    return null;
                }
            }
        }

        public ILTypeInstance ILInstance
        {
            get
            {
                return m_Instance;
            }
        }

        public void Dispose()
        {
            if (m_DisposeMethod == null)
            {
                m_DisposeMethod = m_Instance.Type.GetMethod("Dispose", 0);
                if (m_DisposeMethod == null)
                {
                    m_DisposeMethod = m_Instance.Type.GetMethod("System.IDisposable.Dispose", 0);
                }
            }

            if (m_DisposeMethod != null)
            {
                m_Appdamain.Invoke(m_DisposeMethod, m_Instance, null);
            }
        }

        public bool MoveNext()
        {
            if (m_MoveNextMethod == null)
            {
                m_MoveNextMethod = m_Instance.Type.GetMethod("MoveNext", 0);
            }

            if (m_MoveNextMethod != null)
            {
                return (bool)m_Appdamain.Invoke(m_MoveNextMethod, m_Instance, null);
            }
            else
            {
                return false;
            }
        }

        public void Reset()
        {
            if (m_ResetMethod == null)
            {
                m_ResetMethod = m_Instance.Type.GetMethod("Reset", 0);
            }

            if (m_ResetMethod != null)
            {
                m_Appdamain.Invoke(m_ResetMethod, m_Instance, null);
            }
        }

        public override string ToString()
        {
            if (m_ToString == null)
            {
                m_ToString = m_Appdamain.ObjectType.GetMethod("ToString", 0);
            }
            IMethod m = m_Instance.Type.GetVirtualMethod(m_ToString);
            if (m == null || m is ILMethod)
            {
                return m_Instance.ToString();
            }
            else
            {
                return m_Instance.Type.FullName;
            }
        }
    }
}
#endregion