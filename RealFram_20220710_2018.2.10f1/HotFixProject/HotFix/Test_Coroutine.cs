using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix
{
    /// <summary>
    /// 测试协程
    /// </summary>
    public class Test_Coroutine
    {
        public static void Start()
        {
            Demo15.GameStart.Instance.StartCoroutine(Coroutine());
        }

        static System.Collections.IEnumerator Coroutine()
        {

            Debug.LogFormat("协程开始，Time：{0}",Time.time);
            yield return new WaitForSeconds(3);


            Debug.LogFormat("协程结束，Time：{0}",Time.time);
        }
    }
}
