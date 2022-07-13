/****************************************************
    文件：ClassObjectPool.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/13 2:27:26
	功能：一种Pool
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassObjectPool<T> where T : class, new() //一种类，可以实例
{

    public Stack<T> m_Pool = new Stack<T>();
    public int m_MaxCnt = 0;
    public int m_NoRecycleCnt = 0;

    public ClassObjectPool(int maxCnt)
    {
        m_MaxCnt = maxCnt;
        for (int i = 0; i < maxCnt; i++)
        {
            m_Pool.Push(new T());
        }
    }

    /// <summary>
    /// 没池造池
    /// </summary>
    /// <param name="CreatePoolEmpty"></param>
    /// <returns></returns>
    public T Spawn(bool CreatePoolEmpty)
    {
        if (m_Pool.Count > 0)
        {
            T rtn = m_Pool.Pop();
            if (rtn == null)

            {
                if (CreatePoolEmpty)
                {
                    rtn = new T();
                }
            }
            m_NoRecycleCnt++;
            return rtn;
        }

        else
        {

            if (CreatePoolEmpty)
            {
                T rtn = new T();
                m_NoRecycleCnt++;
                return rtn;
            }



        }

        return null;
    }



    /// <summary>
    /// 回收对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool Recycle(T obj)
    {
        if (obj == null)
        {
            m_NoRecycleCnt++;
            return false;
        }
        //
        m_NoRecycleCnt--;
        if (m_MaxCnt > 0 && m_Pool.Count >= m_MaxCnt)
        {
            obj = null;
            return false;
        }

        m_Pool.Push((T)obj);

        return true;
    }
}
