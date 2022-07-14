/****************************************************
    文件：ObjectMgr.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/13 2:23:6
	功能：管理那种池子——ClassObjectPool<T>

       1：同步加载
       2：异步加载
       3：预加载
       4：取消异步加载
       5：基于离线数据的回收处理

*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMgr : Singleton<ObjectMgr> 
{


	Dictionary<Type,object> m_ClassPoolDic=new   Dictionary<Type,object>();


    /// <summary>
    /// 创建类对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="maxCnt"></param>
    /// <returns></returns>
    public ClassObjectPool<T> TryGetClassObjectPool<T>(int maxCnt) where T : class, new()
    {
        Type type = typeof(T);
        object obj;
        if ( m_ClassPoolDic.TryGetValue(type, out obj)==false || obj == null)
        {
            ClassObjectPool<T> pool = new ClassObjectPool<T>(maxCnt);
            m_ClassPoolDic.Add(type, pool);

            return pool;
        }
        else
        {
            return obj as ClassObjectPool<T>;
        }
    }
}
