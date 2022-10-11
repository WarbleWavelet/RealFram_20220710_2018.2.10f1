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
    unsafe class ILRuntimeMgr_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(ILRuntimeMgr);

            field = type.GetField("delegate_Void", flag);
            app.RegisterCLRFieldGetter(field, get_delegate_Void_0);
            app.RegisterCLRFieldSetter(field, set_delegate_Void_0);
            field = type.GetField("delegate_String", flag);
            app.RegisterCLRFieldGetter(field, get_delegate_String_1);
            app.RegisterCLRFieldSetter(field, set_delegate_String_1);
            field = type.GetField("action_String", flag);
            app.RegisterCLRFieldGetter(field, get_action_String_2);
            app.RegisterCLRFieldSetter(field, set_action_String_2);


        }



        static object get_delegate_Void_0(ref object o)
        {
            return ((ILRuntimeMgr)o).delegate_Void;
        }
        static void set_delegate_Void_0(ref object o, object v)
        {
            ((ILRuntimeMgr)o).delegate_Void = (Delegate_Void)v;
        }
        static object get_delegate_String_1(ref object o)
        {
            return ((ILRuntimeMgr)o).delegate_String;
        }
        static void set_delegate_String_1(ref object o, object v)
        {
            ((ILRuntimeMgr)o).delegate_String = (Delegate_String)v;
        }
        static object get_action_String_2(ref object o)
        {
            return ((ILRuntimeMgr)o).action_String;
        }
        static void set_action_String_2(ref object o, object v)
        {
            ((ILRuntimeMgr)o).action_String = (System.Action<System.String>)v;
        }


    }
}
