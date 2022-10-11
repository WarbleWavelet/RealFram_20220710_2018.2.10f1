using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region �� InheritanceAdapter


public class InheritanceAdapter : CrossBindingAdaptor
{
    public override System.Type BaseCLRType
    {
        get
        {
            return typeof(Test_ClassBase);//��̳е���
        }
    }

    public override System.Type AdaptorType
    {
        get
        {
            return typeof(Adapter);//ʵ�ʵ���������
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adapter(appdomain, instance);
    }

    /// <summary>
    /// ��������
    /// </summary>
    class Adapter : Test_ClassBase, CrossBindingAdaptorType
    {
        #region ����  ����


        private ILRuntime.Runtime.Enviorment.AppDomain m_Appdomain; //
        private ILTypeInstance m_Instance;                          //
        private IMethod m_TestAbstract;                             //
        private IMethod m_TestVirtual;                              //
        private IMethod m_GetValue;                                 //
        private IMethod m_ToString;                                 //
        object[] param1 = new object[1];                            //�κ����͵ı���
        private bool m_TestVirtualInvoking = false;                 //�Ƿ�����ִ���鷽��������Ҫ�趨һ����ʶλ����ʾ��ǰ�Ƿ��ڵ����У���������ű����������base.TestVirtual()�ͻ��������ѭ��
        private bool m_GetValueInvoking = false;                    //���ڻ�ȡֵ

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


        #region override 4������


        #region �����������


        //������������д������Ҫ���ȸ��ű���д�ķ��������ҽ�����Ȩת�Ƶ��ű���ȥ������=>�ȸ���
        public override void TestAbstract(int a)
        {
            if (m_TestAbstract == null)
            {
                m_TestAbstract = m_Instance.Type.GetMethod("TestAbstract", 1);
            }

            if (m_TestAbstract != null)
            {
                param1[0] = a;
                m_Appdomain.Invoke(m_TestAbstract, m_Instance, param1);//ת��
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
                m_Appdomain.Invoke(m_TestVirtual, m_Instance, param1);//������ʵ��������
                m_TestVirtualInvoking = false;
            }
            else
            {
                base.TestVirtual(str);//ִ��
            }
        }
        #endregion


        public override int Value
        {
            get
            {
                if (m_GetValue == null)
                {
                    m_GetValue = m_Instance.Type.GetMethod("get_Value", 1);//get_ ���Ա�������ǰ���get_�ķ���
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
        /// ��������ÿ���������ǹ̶���
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (m_ToString == null)
            {
                m_ToString = m_Appdomain.ObjectType.GetMethod("ToString", 0);//ObjectType��������
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
