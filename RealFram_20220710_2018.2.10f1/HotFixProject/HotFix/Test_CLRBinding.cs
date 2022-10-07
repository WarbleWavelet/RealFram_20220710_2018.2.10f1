/****************************************************

	文件：
	作者：WWS
	日期：2022/10/06 15:14:58
	功能： CLR（公共语言运行库）

*****************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotFix
{
    class Test_CLRBinding
    {
        public static void Start()
        {
            for (int i = 0; i < 100000; i++)
            {
                Test_CLRBindingClass.Add(i,i);
            }
        }
    }
}



