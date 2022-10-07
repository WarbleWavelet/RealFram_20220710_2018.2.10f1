using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    class Test_MonoBehaviour
    {
        public static void Start1(GameObject go)
        {
             go.AddComponent<Test_MonoBehaviourClass>();
        }

        public static void Start2(GameObject go)
        {
            go.AddComponent<Test_MonoBehaviourClass>();
            Test_MonoBehaviourClass test_CLRBindingClass = go.GetComponent<Test_MonoBehaviourClass>();
            test_CLRBindingClass.Test();

        }
    }

    class Test_MonoBehaviourClass : MonoBehaviour
    {
        static string m_NamespaceClass = "HotFix.Test_MonoBehaviourClass";


        void Awake()
        {

            Debug.LogFormat("{0}.Awake()", m_NamespaceClass);
        }

        void Start()
        {
            Debug.LogFormat("{0}.Start()", m_NamespaceClass);
        }

        void Update()
        {
            Debug.LogFormat("{0}.Update()", m_NamespaceClass);
        }

        public void Test()
        {
            Debug.LogFormat("{0}.Test()", m_NamespaceClass);
        }
    }
}
