#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
[System.Reflection.Obfuscation(Exclude = true)]
public class ILRuntimeCLRBinding
{
    static string m_path_HotFixDll = DefinePath.Path_HotFixDll_Txt;            //读取热更资源的dll 。原本用.dll，Ocean用.dll.txt
    static string m_Path_Generated = DefinePath.Path_Generated;
    static string m_Path_ILRuntimeCLRBinding = DefinePath.m_Path_ILRuntimeCLRBinding;


    [MenuItem(DefinePath.MenuItem_ILR + "定位到输出脚本", false, 0)]
    static void MenuItem_ShootScript()
    {
        Common.Selection_ActiveObject(m_Path_ILRuntimeCLRBinding);
    }
    [MenuItem(DefinePath.MenuItem_ILR + "定位到输出路径", false, 0)]
    static void MenuItem_ShootPath()
    {
        Common.Selection_ActiveObject(m_Path_Generated);
    }

    [MenuItem(DefinePath.MenuItem_ILR +"Generate CLR Binding Code", false, 0)]
    static void MenuItem_GenerateCLRBinding()
    {
        List<Type> types = new List<Type>();
        types.Add(typeof(int));
        types.Add(typeof(float));
        types.Add(typeof(long));
        types.Add(typeof(object));
        types.Add(typeof(string));
        types.Add(typeof(Array));
        types.Add(typeof(Vector2));
        types.Add(typeof(Vector3));
        types.Add(typeof(Quaternion));
        types.Add(typeof(GameObject));
        types.Add(typeof(UnityEngine.Object));
        types.Add(typeof(Transform));
        types.Add(typeof(RectTransform));
        //types.Add(typeof(CLRBindingTestClass)); //Ocean命名的类
        types.Add(typeof(Test_CLRBindingClass));  //我要处理的类
        types.Add(typeof(Time));
        types.Add(typeof(Debug));
        types.Add(typeof(List<ILRuntime.Runtime.Intepreter.ILTypeInstance>));//所有DLL内的类型的真实C#类型都是ILTypeInstance

        ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(types, m_Path_Generated);

    }

    [MenuItem(DefinePath.MenuItem_ILR + "Generate CLR Binding Code by Analysis", false, 0)]
    static void MenuItem_GenerateCLRBindingByAnalysis()//用新的分析热更dll调用引用来生成绑定代码
    {
        Common.Folder_Clear_Recursive(m_Path_Generated);
        ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();
        using (System.IO.FileStream fs = new System.IO.FileStream(m_path_HotFixDll, System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            domain.LoadAssembly(fs);
        }
        //Crossbind Adapter is needed to generate the correct binding code
        InitILRuntime(domain);
        ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, m_Path_Generated);

        Common.Refresh();
    }


    /// <summary>
    /// 绑定自己创建的适配器
    /// </summary>
    /// <param name="domain"></param>
    static void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain domain)
    {
        //这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用
        domain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
        domain.RegisterCrossBindingAdaptor(new CoroutineAdapter());  
        domain.RegisterCrossBindingAdaptor(new WindowAdapter());
        domain.RegisterCrossBindingAdaptor(new InheritanceAdapter());
       
    }
}
#endif
