/****************************************************
    文件：ObjectMgr.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/13 2:23:6
	功能：对象管理器
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMgr : Singleton<ObjectMgr> {

	Dictionary<Type,object> m_ClassPoolDic=new   Dictionary<Type,object>();


    /// <summary>
    /// 创建类对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="maxCnt"></param>
    /// <returns></returns>
    public ClassObjectPool<T> TryGetClassobjectPool<T>(int maxCnt) where T : class, new()
    {
        Type type = typeof(T);
        object obj;
        if (m_ClassPoolDic.TryGetValue(type, out obj) || obj == null)
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
