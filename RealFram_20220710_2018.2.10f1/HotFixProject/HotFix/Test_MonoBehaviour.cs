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
        public static void Start(GameObject go)
        {
             go.AddComponent<Test_MonoBehaviourClass>();
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
