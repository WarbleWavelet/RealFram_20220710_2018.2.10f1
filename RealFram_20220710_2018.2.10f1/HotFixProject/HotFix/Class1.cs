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


        #region 字属


        static string m_NameSpaceClass = "HotFix.Class1";
        private int m_Id=0;
        public int ID { get => m_Id; set => m_Id = value; }

        #endregion



        #region 构造

        public Class1()
        {
            m_Id = 100;
            Debug.Log(m_NameSpaceClass+".构造" + "，参数：" + m_Id);
        }

        public Class1(int a)
        {
             m_Id = a;
            Debug.Log(m_NameSpaceClass + ".构造" + "，参数：" + m_Id);
        }

       
        #endregion


        public static  void Test_StaticFunction()
        {
                       
            Debug.Log(m_NameSpaceClass+ ".Test_StaticFunction");
        }

        public static void Test_StaticFunction(int a)
        {
            Debug.Log(m_NameSpaceClass+ ".Test_StaticFunction" + "，参数："+a);
        }

        public static void Test_GenericFunction<T>(T a)
        {
            Debug.Log(m_NameSpaceClass + ".Test_GenericFunction" + "，参数：" + a);
        }
    }
}
