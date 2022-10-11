using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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