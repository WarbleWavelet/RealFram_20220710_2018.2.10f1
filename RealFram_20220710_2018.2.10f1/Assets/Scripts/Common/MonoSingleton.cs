/****************************************************
    文件：MonoSingleton.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/25 20:8:50
	功能：一种常用的类
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{


    #region 单例
    private static T _instance;

    public static T Instance
    {

        get
        {
            return _instance;
        }
    }
    #endregion





    #region 生命
    public virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = (T)this;
        }
        else
        {
            Debug.LogError("绝对不可能有两个单例："+this.GetType());
        }
    }


    #endregion

}
