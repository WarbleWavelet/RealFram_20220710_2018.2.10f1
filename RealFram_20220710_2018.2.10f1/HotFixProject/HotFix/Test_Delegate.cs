using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Debug= UnityEngine.Debug;

namespace HotFix
{
   public class Test_Delegate
    {
        static Delegate_Void delegate_Void;//在UNity中引用ILRunTImeMgr中的（在Manager.dll内）
        static Delegate_String delegate_String;
        static Action<string> action_String;
        private static string m_NameSpaceClass="HotFix.Test_Delegate";

        
        #region 生命


        public static void Awake1()//借鉴Unity。热更域定义，热更域使用
        {
            delegate_Void = Test_VoidFunction;
            delegate_String = Test_StringFunction;
            action_String = Test_VoidFunction;//从这里可以看到方法重载时，后续根据实际参数调用具体方法
        }

        public static void Start1()
        {
            if (delegate_Void != null)
            {
                delegate_Void(64);
            }
            if (delegate_String != null)
            {
                delegate_String(64);
            }
            if (action_String != null)
            {
                action_String("64");
            }
        }

        public static void Awake2()//主程域定义，热更域使用
        {
            ILRuntimeMgr.Instance.delegate_Void = Test_VoidFunction;
            ILRuntimeMgr.Instance.delegate_String = Test_StringFunction;
            ILRuntimeMgr.Instance.action_String = Test_VoidFunction;
        }

        public static void Start2()
        {
            if (ILRuntimeMgr.Instance.delegate_Void != null)
            {
                ILRuntimeMgr.Instance.delegate_Void(64);
            }
            if (ILRuntimeMgr.Instance.delegate_String != null)
            {
                ILRuntimeMgr.Instance.delegate_String(64);
            }
            if (ILRuntimeMgr.Instance.action_String != null)
            {
                ILRuntimeMgr.Instance.action_String("64");
            }
        }
        #endregion


        public static void Test_VoidFunction(int a)
        {
            Debug.Log(m_NameSpaceClass + ".Test_VoidFunction，参数：" + a);
        }


        public static string Test_StringFunction(int a)
        {
            Debug.Log(m_NameSpaceClass + ".Test_StringFunction，参数：" + a);
            return a.ToString();
        }

        public static void Test_VoidFunction(string a)
        {
            Debug.Log(m_NameSpaceClass + ".Test_VoidFunction，参数：" + a);
        }

    }

    

}
