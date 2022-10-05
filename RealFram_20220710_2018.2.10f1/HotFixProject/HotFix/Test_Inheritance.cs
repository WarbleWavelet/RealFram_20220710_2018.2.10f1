using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    /// <summary>
    /// 测试跨域继承
    /// </summary>
    class Test_Inheritance : Test_ClassBase
    {
        static string m_NamespaceClass="HotFix.Test_Inheritance";
        public override void TestAbstract(int a)
        {

            Debug.LogFormat("{0}.TestAbstract(int a={1})", m_NamespaceClass,a);
        }

        public override void TestVirtual(string str)
        {
            Debug.LogFormat("{0}.TestVirtual(string str={1})", m_NamespaceClass, str);
        }

         /// <summary>
         /// 实现实例的一种方法：通过静态方法
         /// </summary>
         /// <returns></returns>
        public static Test_Inheritance NewObj()
        {
            return new Test_Inheritance();
        }
    }
}
