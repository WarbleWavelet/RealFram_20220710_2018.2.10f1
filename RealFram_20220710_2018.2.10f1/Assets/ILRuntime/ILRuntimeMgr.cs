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
using ILRuntime.Runtime.Stack;
//using UnityAction=UnityEngine.Events.UnityAction;//没用，适配的时候


public class ILRuntimeMgr : Singleton<ILRuntimeMgr>
{


    #region 字属


    string m_path_HotFixDll = DefinePath.Path_HotFixDll_Txt;            //读取热更资源的dll
    string m_path_HotFixPdb = DefinePath.Path_HotFixPdb_Txt;            //读取热更资源的pdb
    static AppDomain m_AppDomain;
    GameObject m_GameStart = new GameObject();

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

    #region 类.方法

    const string m_NameSpaceClass3 = "HotFix.Test_Inheritance";
    const string m_Method_31 = "NewObj";
    #endregion

    #region 类.方法


    const string m_NameSpaceClass4 = "HotFix.Test_CLRBinding";
    const string m_Method_41 = "Start";

    #endregion


    #region 类.方法


    const string m_NameSpaceClass5 = "HotFix.Test_Coroutine";
    const string m_Method_51 = "Start";

    #endregion

    #region 类.方法


    const string m_NameSpaceClas6 = "HotFix.Test_MonoBehaviour";
    const string m_Method_61 = "Start";

    #endregion
    #endregion



    #region 生命
    public void InitMgr(GameObject gameObject)
    {
        m_GameStart = gameObject;
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
        // OnHotFixLoaded_Test12();
        // OnHotFixLoaded_Test13();
        // OnHotFixLoaded_Test14();
        // OnHotFixLoaded_Test15();
        OnHotFixLoaded_Test16();
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
        RegisterAdapter_Coroutine();
        RegisterAdapter_MonoBehaviour();
        RegisterAdapter_CLRAddCompontent();
        RegisterAdapter_CLRGetCompontent();
        
        
        RegisterAdapter_CLRBinding();
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

    /// <summary>
    /// CLRBinding，放最后，起码在CLR重定向之后
    /// </summary>
    public void RegisterAdapter_CLRBinding()
    {
        //类是自动生成的，默认不是public，有时需要改
        ILRuntime.Runtime.Generated.CLRBindings.Initialize(m_AppDomain);
    }


    public void RegisterAdapter_Coroutine()
    {
        m_AppDomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
    }


    public void RegisterAdapter_MonoBehaviour()
    {
        m_AppDomain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
    }




    #region CLR重定向
    unsafe void RegisterAdapter_CLRAddCompontent()
    {
        var arr = typeof(GameObject).GetMethods();
        foreach (var i in arr)
        {
            if (i.Name == "AddComponent" && i.GetGenericArguments().Length == 1)
            {
                m_AppDomain.RegisterCLRMethodRedirection(i, AddCompontent);
            }
        }
    }


    /// <summary>
    /// CLR重定向
    /// </summary>
    unsafe void RegisterAdapter_CLRGetCompontent()
    {
        var arr = typeof(GameObject).GetMethods();
        foreach (var i in arr)
        {
            if (i.Name == "GetCompontent" && i.GetGenericArguments().Length == 1)
            {
                m_AppDomain.RegisterCLRMethodRedirection(i, GetCompontent);
            }
        }
    }

    /// <summary>
    /// 重定向
    /// </summary>
    /// <param name="__intp">包含了</param>
    /// <param name="__esp"></param>
    /// <param name="__mStack"></param>
    /// <param name="__method"></param>
    /// <param name="isNewObj"></param>
    /// <returns></returns>
    /// <exception cref="System.NullReferenceException"></exception>
    private unsafe StackObject* AddCompontent(
        ILIntepreter __intp,
        StackObject* __esp,
        IList<object> __mStack,
        CLRMethod __method,
        bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain; //获取程序集

        var ptr = __esp - 1;
        GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject; //获取值
        if (instance == null)
        {
            throw new System.NullReferenceException();
        }
        __intp.Free(ptr); //释放

        var genericArgument = __method.GenericArguments;  //获取泛型变量
        if (genericArgument != null && genericArgument.Length == 1) //
        {
            var type = genericArgument[0];
            object res;
            if (type is CLRType)//CLRType表示这个类型是Unity工程里的类型，ILType表示是热更dll里面的类型
            {
                res = instance.AddComponent(type.TypeForCLR);//Unity主工程的类，不需要做处理
            }
            else
            {
                //创建出来MonoTest,
                //热更类型的类
                //MonoBehaviour不允许new，所以false
                var ilInstance = new ILTypeInstance(type as ILType, false);
                var clrInstance = instance.AddComponent<MonoBehaviourAdapter.Adaptor>();//适配器实例
                clrInstance.ILInstance = ilInstance;//替换
                clrInstance.AppDomain = __domain;
                //这个实例默认创建的CLRInstance不是通过AddCompontent出来的有效实例，所以要替换
                ilInstance.CLRInstance = clrInstance;//替换
                res = clrInstance.ILInstance;
                clrInstance.Awake();//补掉Awake。ILInstance未赋值不会真正调用Awake
            }
            return ILIntepreter.PushObject(ptr, __mStack, res);
        }

        return __esp;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="__intp"></param>
    /// <param name="__esp"></param>
    /// <param name="__mStack"></param>
    /// <param name="__method"></param>
    /// <param name="isNewObj"></param>
    /// <returns></returns>
    /// <exception cref="System.NullReferenceException"></exception>
    private unsafe StackObject* GetCompontent(
        ILIntepreter __intp, 
        StackObject* __esp, 
        IList<object> __mStack, 
        CLRMethod __method, 
        bool isNewObj)
    {
        ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;//获取程序集

        var ptr = __esp - 1;
        GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;//获取值
        if (instance == null)
            throw new System.NullReferenceException();

        __intp.Free(ptr);//释放

        var genericArgument = __method.GenericArguments; //获取泛型变量
        if (genericArgument != null && genericArgument.Length == 1) //参数只有一个
        {
            var type = genericArgument[0]; //取参数
            object res = null;
            //是Unity主工程的东西，不需要做处理；
            //CLRType表示这个类型是Unity工程里的类型，ILType表示是热更dll里面的类型
            if (type is CLRType)
            {
                res = instance.GetComponent(type.TypeForCLR);//获取组件，
            }
            else
            {
                var clrInstances = instance.GetComponents<MonoBehaviourAdapter.Adaptor>();
                foreach (var clrInstance in clrInstances)
                {
                    if (clrInstance.ILInstance != null)
                    {
                        if (clrInstance.ILInstance.Type == type)
                        {
                            res = clrInstance.ILInstance;
                            break;
                        }
                    }
                }
            }

            return ILIntepreter.PushObject(ptr, __mStack, res);
        }

        return __esp;
    }




   
    #endregion



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
    /// 测试绑定前后的时间差
    /// </summary>
    private void OnHotFixLoaded_Test14()
    {
        Common.Time_During(() => { 
                m_AppDomain.Invoke(m_NameSpaceClass4,m_Method_41,null,null);
            }, 
            TimeUnit.mSeconds
        );
    }

    void HoldComponent()
    { 
    
    }

    /// <summary>
    /// 测试协程
    /// </summary>
    private void OnHotFixLoaded_Test15()
    {
        m_AppDomain.Invoke(m_NameSpaceClass5, m_Method_51, null, null);
    }

    /// <summary>
    /// 测试MonoBehaviour
    /// </summary>
    private void OnHotFixLoaded_Test16()
    {
        m_AppDomain.Invoke(m_NameSpaceClas6,m_Method_61,null,m_GameStart); //我用dll来调用，挺麻烦的
    }



    #region AppDomain_Invoke


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

    #endregion

}



//**
#region 委托


public delegate void Delegate_Void(int a);
public delegate string Delegate_String(int a);

#endregion


#region 类 Test_CLRBindingClass

//因引用顺序，在Data程序集内

#endregion

#region 类 Test_ClassBase


public abstract class Test_ClassBase
{
    public virtual int Value
    {
        get { return 0; }
    }

    public virtual void TestVirtual(string str)
    {
        Debug.LogFormat("Test_ClassBase.TestVirtual(str:{0}) ", str);
    }

    public abstract void TestAbstract(int a);
}
#endregion


#region 类 InheritanceAdapter


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


#region 类 CoroutineAdapter 协程适配器


/// <summary>
/// 协程适配器
/// </summary>
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

    /// <summary>适配器</summary>
    public class Adaptor : IEnumerator<System.Object>, IEnumerator, System.IDisposable, CrossBindingAdaptorType
    {
        private ILTypeInstance m_Instance;
        private ILRuntime.Runtime.Enviorment.AppDomain m_Appdomain;
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
            m_Appdomain = appdomain;
        }


        public object Current
        {
            get
            {
                if (m_CurMethod == null)
                {
                    m_CurMethod = m_Instance.Type.GetMethod("get_Current", 0);
                    if (m_CurMethod == null)//可能取不到
                    {
                        m_CurMethod = m_Instance.Type.GetMethod("System.Collections.IEnumerator.get_Current", 0);
                    }
                }

                if (m_CurMethod != null)
                {
                    var res = m_Appdomain.Invoke(m_CurMethod, m_Instance, null);
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
                m_Appdomain.Invoke(m_DisposeMethod, m_Instance, null);
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
                return (bool)m_Appdomain.Invoke(m_MoveNextMethod, m_Instance, null);
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
                m_Appdomain.Invoke(m_ResetMethod, m_Instance, null);
            }
        }

        public override string ToString()
        {
            if (m_ToString == null)
            {
                m_ToString = m_Appdomain.ObjectType.GetMethod("ToString", 0);
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


#region 类 MonoBehaviourAdapter

public class MonoBehaviourAdapter : CrossBindingAdaptor
{
    public override System.Type BaseCLRType
    {
        get
        {
            return typeof(MonoBehaviour);
        }
    }

    public override System.Type AdaptorType
    {
        get { return typeof(Adaptor); }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }

    public class Adaptor : MonoBehaviour, CrossBindingAdaptorType
    {
        private ILRuntime.Runtime.Enviorment.AppDomain m_Appdomain;
        private ILTypeInstance m_Instance;
        private IMethod m_AwakeMethod;//只写了3个生命函数
        private IMethod m_StartMethod;
        private IMethod m_UpdateMethod;
        private IMethod m_ToString;

        public Adaptor() { }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            m_Appdomain = appdomain;
            m_Instance = instance;
        }

        public ILTypeInstance ILInstance
        {
            get
            {
                return m_Instance;
            }
            set
            {
                m_Instance = value;
                m_AwakeMethod = null;//有可能变了，所以置成空
                m_StartMethod = null;
                m_UpdateMethod = null;
            }
        }

        public ILRuntime.Runtime.Enviorment.AppDomain AppDomain
        {
            get { return m_Appdomain; }
            set { m_Appdomain = value; }
        }

        public void Awake()
        {
            if (m_Instance != null)
            {
                if (m_AwakeMethod == null)
                {
                    m_AwakeMethod = m_Instance.Type.GetMethod("Awake", 0);
                }

                if (m_AwakeMethod != null)
                {
                    m_Appdomain.Invoke(m_AwakeMethod, m_Instance, null);
                }
            }
        }

        void Start()
        {
            if (m_StartMethod == null)
            {
                m_StartMethod = m_Instance.Type.GetMethod("Start", 0);
            }

            if (m_StartMethod != null)
            {
                m_Appdomain.Invoke(m_StartMethod, m_Instance, null);
            }
        }


        void Update()
        {
            if (m_UpdateMethod == null)
            {
                m_UpdateMethod = m_Instance.Type.GetMethod("Update", 0);
            }

            if (m_UpdateMethod != null)
            {
                m_Appdomain.Invoke(m_UpdateMethod, m_Instance, null);
            }
        }

        public override string ToString()
        {
            if (m_ToString == null)
            {
                m_ToString = m_Appdomain.ObjectType.GetMethod("ToString", 0);
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

//**/