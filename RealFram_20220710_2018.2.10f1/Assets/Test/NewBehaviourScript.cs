/****************************************************
    文件：NewBehaviourScript.cs
	作者：lenovo
    邮箱: 
    日期：2022/10/5 12:50:17
	功能：
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
namespace MyNamespace
{
  public class NewBehaviourScript : MonoBehaviour
{
    void Start()
    {
        Common.Log_NamespaceClassFunction();
        Common.Log_ClassFunction();
    }
}
}


