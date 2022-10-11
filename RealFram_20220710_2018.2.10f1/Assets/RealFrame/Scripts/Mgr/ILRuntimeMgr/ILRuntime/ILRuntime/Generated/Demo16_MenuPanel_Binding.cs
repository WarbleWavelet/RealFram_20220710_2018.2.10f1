using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class Demo16_MenuPanel_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Demo16.MenuPanel);

            field = type.GetField("m_Image01_01", flag);
            app.RegisterCLRFieldGetter(field, get_m_Image01_01_0);
            app.RegisterCLRFieldSetter(field, set_m_Image01_01_0);
            field = type.GetField("m_Image01_02", flag);
            app.RegisterCLRFieldGetter(field, get_m_Image01_02_1);
            app.RegisterCLRFieldSetter(field, set_m_Image01_02_1);
            field = type.GetField("m_Image02_01", flag);
            app.RegisterCLRFieldGetter(field, get_m_Image02_01_2);
            app.RegisterCLRFieldSetter(field, set_m_Image02_01_2);
            field = type.GetField("m_Image02_02", flag);
            app.RegisterCLRFieldGetter(field, get_m_Image02_02_3);
            app.RegisterCLRFieldSetter(field, set_m_Image02_02_3);
            field = type.GetField("m_Image03_01", flag);
            app.RegisterCLRFieldGetter(field, get_m_Image03_01_4);
            app.RegisterCLRFieldSetter(field, set_m_Image03_01_4);
            field = type.GetField("m_Image03_02", flag);
            app.RegisterCLRFieldGetter(field, get_m_Image03_02_5);
            app.RegisterCLRFieldSetter(field, set_m_Image03_02_5);
            field = type.GetField("m_BtnStart", flag);
            app.RegisterCLRFieldGetter(field, get_m_BtnStart_6);
            app.RegisterCLRFieldSetter(field, set_m_BtnStart_6);
            field = type.GetField("m_BtnLoad", flag);
            app.RegisterCLRFieldGetter(field, get_m_BtnLoad_7);
            app.RegisterCLRFieldSetter(field, set_m_BtnLoad_7);
            field = type.GetField("m_BtnExit", flag);
            app.RegisterCLRFieldGetter(field, get_m_BtnExit_8);
            app.RegisterCLRFieldSetter(field, set_m_BtnExit_8);


        }



        static object get_m_Image01_01_0(ref object o)
        {
            return ((Demo16.MenuPanel)o).m_Image01_01;
        }
        static void set_m_Image01_01_0(ref object o, object v)
        {
            ((Demo16.MenuPanel)o).m_Image01_01 = (UnityEngine.UI.Image)v;
        }
        static object get_m_Image01_02_1(ref object o)
        {
            return ((Demo16.MenuPanel)o).m_Image01_02;
        }
        static void set_m_Image01_02_1(ref object o, object v)
        {
            ((Demo16.MenuPanel)o).m_Image01_02 = (UnityEngine.UI.Image)v;
        }
        static object get_m_Image02_01_2(ref object o)
        {
            return ((Demo16.MenuPanel)o).m_Image02_01;
        }
        static void set_m_Image02_01_2(ref object o, object v)
        {
            ((Demo16.MenuPanel)o).m_Image02_01 = (UnityEngine.UI.Image)v;
        }
        static object get_m_Image02_02_3(ref object o)
        {
            return ((Demo16.MenuPanel)o).m_Image02_02;
        }
        static void set_m_Image02_02_3(ref object o, object v)
        {
            ((Demo16.MenuPanel)o).m_Image02_02 = (UnityEngine.UI.Image)v;
        }
        static object get_m_Image03_01_4(ref object o)
        {
            return ((Demo16.MenuPanel)o).m_Image03_01;
        }
        static void set_m_Image03_01_4(ref object o, object v)
        {
            ((Demo16.MenuPanel)o).m_Image03_01 = (UnityEngine.UI.Image)v;
        }
        static object get_m_Image03_02_5(ref object o)
        {
            return ((Demo16.MenuPanel)o).m_Image03_02;
        }
        static void set_m_Image03_02_5(ref object o, object v)
        {
            ((Demo16.MenuPanel)o).m_Image03_02 = (UnityEngine.UI.Image)v;
        }
        static object get_m_BtnStart_6(ref object o)
        {
            return ((Demo16.MenuPanel)o).m_BtnStart;
        }
        static void set_m_BtnStart_6(ref object o, object v)
        {
            ((Demo16.MenuPanel)o).m_BtnStart = (UnityEngine.UI.Button)v;
        }
        static object get_m_BtnLoad_7(ref object o)
        {
            return ((Demo16.MenuPanel)o).m_BtnLoad;
        }
        static void set_m_BtnLoad_7(ref object o, object v)
        {
            ((Demo16.MenuPanel)o).m_BtnLoad = (UnityEngine.UI.Button)v;
        }
        static object get_m_BtnExit_8(ref object o)
        {
            return ((Demo16.MenuPanel)o).m_BtnExit;
        }
        static void set_m_BtnExit_8(ref object o, object v)
        {
            ((Demo16.MenuPanel)o).m_BtnExit = (UnityEngine.UI.Button)v;
        }


    }
}
