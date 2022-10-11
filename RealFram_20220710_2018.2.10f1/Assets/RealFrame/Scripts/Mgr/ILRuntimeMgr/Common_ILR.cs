using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Common_ILR
{
    public static string ToString(
        ILRuntime.Runtime.Enviorment.AppDomain m_AppDomain,
        ILTypeInstance m_Instance,
        IMethod m_ToString
    )
    {
        if (m_ToString == null)
        {
            m_ToString = m_AppDomain.ObjectType.GetMethod("ToString", 0);
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


    /// <summary>
    ///    Unity�������У��޷�ͨ��Type.GetType��ȡ���ȸ�DLL�ڲ��������
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    public static Type Class_Get(
        ILRuntime.Runtime.Enviorment.AppDomain m_AppDomain,
        string typeName,
      string methodName
    )
    {
        IType type = m_AppDomain.LoadedTypes[typeName];

        return type.ReflectionType;
    }

    public static T Class_New<T>(string namespaceClasssName)
    {
        if (ILRuntimeMgr.Instance.ILRunAppDomain == null)
        {

            Debug.LogErrorFormat("ILRuntimeMgr未初始化" );
        }
        T t= ILRuntimeMgr.Instance.ILRunAppDomain.Instantiate<T>(namespaceClasssName);
        if (t == null)
        {

            Debug.LogErrorFormat("ILR未能生成实例");
            return default(T);
        }
        return t;//namespace.Classs
    }


    /// <summary>
    /// ILRuntime�Ľӿڣ����÷���
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="m_AppDomain"></param>
    /// <param name="typeName"></param>
    /// <param name="methodName"></param>
    public static void Invoke<T>(
        ILRuntime.Runtime.Enviorment.AppDomain m_AppDomain, 
        string  typeName,
        string methodName
    )
    {
        IType t = m_AppDomain.LoadedTypes[typeName];
        Type type = t.ReflectionType;
        object instance = m_AppDomain.Instantiate(typeName);
        IMethod m = t.GetMethod(methodName, 0);

        m_AppDomain.Invoke(m, instance, null);
    }

    /// <summary>
    /// ���ȸ�DLL��Unity�������л�ȡ������Field��ֵ
    /// </summary>
    /// <param name="t"></param>
    /// <param name="instance"></param>
    /// <param name="valueName"></param>
    public static void Value_Get(Type t, object instance,string valueName)
    {
        FieldInfo fi = t.GetField(valueName);

        object val = fi.GetValue(instance);

        fi.SetValue(instance, val);
    }

    /// <summary>
    /// ���ȸ�DLL��Unity�������л�ȡAttribute��ע
    /// </summary>
    /// <param name="t"></param>
    /// <param name="instance"></param>
    /// <param name="valueName"></param>
    public static object[] Attribute_GetAll<TAttributeType>(Type t, string valueName)
    {
        FieldInfo fi = t.GetField(valueName);

        object[] attributeArr = fi.GetCustomAttributes(typeof(TAttributeType), false);
        return attributeArr;

    }




}
