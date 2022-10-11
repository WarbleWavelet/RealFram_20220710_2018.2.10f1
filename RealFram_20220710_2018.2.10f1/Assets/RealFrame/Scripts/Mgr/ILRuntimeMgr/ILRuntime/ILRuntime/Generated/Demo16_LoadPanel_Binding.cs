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
    unsafe class Demo16_LoadPanel_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Demo16.LoadPanel);

            field = type.GetField("m_TxtPrg", flag);
            app.RegisterCLRFieldGetter(field, get_m_TxtPrg_0);
            app.RegisterCLRFieldSetter(field, set_m_TxtPrg_0);
            field = type.GetField("m_Slider", flag);
            app.RegisterCLRFieldGetter(field, get_m_Slider_1);
            app.RegisterCLRFieldSetter(field, set_m_Slider_1);


        }



        static object get_m_TxtPrg_0(ref object o)
        {
            return ((Demo16.LoadPanel)o).m_TxtPrg;
        }
        static void set_m_TxtPrg_0(ref object o, object v)
        {
            ((Demo16.LoadPanel)o).m_TxtPrg = (UnityEngine.UI.Text)v;
        }
        static object get_m_Slider_1(ref object o)
        {
            return ((Demo16.LoadPanel)o).m_Slider;
        }
        static void set_m_Slider_1(ref object o, object v)
        {
            ((Demo16.LoadPanel)o).m_Slider = (UnityEngine.UI.Slider)v;
        }


    }
}
