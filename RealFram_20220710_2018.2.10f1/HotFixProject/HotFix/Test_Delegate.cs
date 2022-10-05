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
        private static string m_NameSpaceClass="HotFix.Test_Delegate";

        
        #region 生命


        public static void Awake()//借鉴Unity
        {
            delegate_Void = Test_VoidFunction;
            delegate_String = Test_StringFunction;
        }


        public static void Start()
        {
            if (delegate_Void != null)
            {
                delegate_Void(64);
            }
            if (delegate_String != null)
            {
                delegate_String(64);
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



    }

    

}
