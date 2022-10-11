using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appDomain, ILTypeInstance instance)
        {
            m_Appdomain = appDomain;
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

