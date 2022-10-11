using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
