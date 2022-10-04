using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    public class Class1
    {
        static string m_NameSpaceClass = "HotFix.Class1.Test_StaticFunction";
        public static  void Test_StaticFunction()
        {
                       
            Debug.Log(m_NameSpaceClass);
        }

        public static void Test_StaticFunction(int a)
        {
            Debug.Log(m_NameSpaceClass+"，参数："+a);
        }
    }
}
