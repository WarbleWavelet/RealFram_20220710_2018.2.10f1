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
    unsafe class SceneMgr_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(SceneMgr);

            field = type.GetField("m_CurPrg", flag);
            app.RegisterCLRFieldGetter(field, get_m_CurPrg_0);
            app.RegisterCLRFieldSetter(field, set_m_CurPrg_0);


        }



        static object get_m_CurPrg_0(ref object o)
        {
            return ((SceneMgr)o).m_CurPrg;
        }
        static void set_m_CurPrg_0(ref object o, object v)
        {
            ((SceneMgr)o).m_CurPrg = (System.Int32)v;
        }


    }
}
