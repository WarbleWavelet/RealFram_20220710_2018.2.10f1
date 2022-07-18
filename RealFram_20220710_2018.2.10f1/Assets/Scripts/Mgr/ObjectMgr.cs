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
    理论：Object不能挂载到面板，GameObject等的父类
          SetParent、SetActive与实例、销毁的性能消耗
          先实例再修改，不然污染预制体

*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMgr : Singleton<ObjectMgr> 
{
    /// <summary>
    /// 类对象池
    /// </summary>
	Dictionary<Type,object> m_classPoolDic=new   Dictionary<Type,object>();

    /// <summary>父节点</summary> 
    public Transform m_RecyclePoolTrans;
    /// <summary>场景节点，DontDestroyOnLoad(gameObject);</summary>
    public Transform m_SceneTrans;
    

    /// <summary>一种可能有多个</summary> 
    Dictionary<uint, List<ResObj>> m_ResObjPoolDic=new Dictionary<uint, List<ResObj>>();

    /// <summary>m_ResObjPoolDic拿不到就Spawn</summary>
    ClassObjectPool<ResObj> m_ResObjPool=new ClassObjectPool<ResObj>(Constants.ClassObjectPool_RESOBJ_MAXCNT);

    /// <summary> InstaniateID, ResObject 所有实例的Object</summary> 
    Dictionary<int, ResObj> m_ResObjDic=new Dictionary<int, ResObj>();








    #region ClassObjectPool
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
        if ( m_classPoolDic.TryGetValue(type, out obj)==false || obj == null)//没Pool
        {
            ClassObjectPool<T> pool = new ClassObjectPool<T>(maxCnt);
            m_classPoolDic.Add(type, pool);

            return pool;
        }
        else//有Pool
        {
            return obj as ClassObjectPool<T>;
        }

        
    }


    /// <summary>
    /// 清除某个资源在对象池中所有的对象
    /// </summary>
    /// <param name="crc"></param>
    public void ClearPoolObject(uint crc)
    {
        List<ResObj> lst = null;
        if ( m_ResObjPoolDic.TryGetValue(crc, out lst)==false || lst == null)
        { 
              return;      
        }


        for (int i = lst.Count - 1; i >= 0; i--)
        {
            ResObj resObj = lst[i];
            if (resObj.m_JmpClr)
            {
                lst.Remove(resObj);
                int _key = resObj.m_Go.GetInstanceID();
                GameObject.Destroy(resObj.m_Go);
                resObj.Reset();
               m_ResObjDic.Remove(_key);
                m_ResObjPool.Recycle(resObj);
            }
        }

        if (lst.Count <= 0)
        {
            m_ResObjPoolDic.Remove(crc);
        }
    }
    #endregion
    /// <summary>
    /// 
    /// </summary>
    /// <param name="recyclePoolTrans"></param>
    /// <param name="sceneTrans"></param>
    public void InitMgr(Transform recyclePoolTrans, Transform sceneTrans)
    {
        m_RecyclePoolTrans = recyclePoolTrans;
        m_SceneTrans = sceneTrans;
    }




    #region ResObj Object

    /// <summary>
    /// 实例
    /// </summary>
    /// <param name="path"></param>
    /// <param name="setScene">Object设置到Scene中</param>
    /// <param name="jmpClr">跳转场景就清空></param>
    public GameObject InstantiateObject(string path, bool setScene = false, bool jmpClr = true)
    {
        uint crc = CRC32.GetCRC32(path);  
        //Pool有
        ResObj resObj =GetResObj(crc);
        ResItem resItem = AssetBundleMgr.Instance.GetResItem(crc);
        //Pool没有
        if (resObj == null)
        {
            resObj = m_ResObjPool.Spawn(true);
            resObj.m_Crc = crc;
            resObj.m_JmpClr = jmpClr;
            //ResouceManager提供加载方法
            resObj = ResourceMgr.Instance.LoadResObj(path, resObj);
            if (resObj.m_ResItem.m_Obj != null)
            { 
                    resObj.m_Go = GameObject.Instantiate(resObj.m_ResItem.m_Obj) as GameObject;    
            }
        }

        //父节点
        if (setScene)
        {
            resObj.m_Go.transform.SetParent(m_SceneTrans,false);
        }

        //保存
        int key = resObj.m_Go.GetInstanceID();
        if (m_ResObjDic.ContainsKey( key ) == false)
        {
            m_ResObjDic.Add(key, resObj);
        }

        return  resObj.m_Go;
    }


    /// <summary>
    /// 同步加载
    /// </summary>
    /// <param name="crc"></param>
    /// <returns></returns>
    ResObj GetResObj(uint crc)
    {
       //Get lst
        List<ResObj> lst=null;
        if (m_ResObjPoolDic.TryGetValue(crc, out lst)==false || lst==null || lst.Count<=0)
        {
            return null;
        }


        ResourceMgr.Instance.AddResItemRefCnt( crc );
         ResObj resObj = lst[0];
        lst.RemoveAt(0);
        GameObject go = resObj.m_Go;
  

        //处理命名
        if (System.Object.ReferenceEquals(go, null) == false)
        {
            resObj.m_Released = false; 
            if (go.name.EndsWith(Constants.FixSur_ResObject_m_Go) == true)//刚拿出来
            {

#if UNITY_EDITOR
                go.name.Replace( Constants.FixSur_ResObject_m_Go, "");//到Editor
#endif


            }
        }


        return resObj;

    }


    /// <summary>
    /// 释放内存
    /// </summary>
    /// <param name="go"></param>
    /// <param name="maxCacheCnt">如果要缓存，个数</param>
    /// <param name="destroyCache"> =>resItem  =>AB包资源及其依赖</param>
    /// <param name="recycleParent">比如UI，读取慢，就false，不放回去</param>
    public void ReleaseObject(GameObject go, int maxCacheCnt = -1, bool destroyCache = false, bool recycleParent = true)
    {
        if (go == null)
        { 
            return;
        }

        //go =>resobj
        int key = go.GetInstanceID();
        ResObj resObj = null;
        if (m_ResObjDic.TryGetValue( key, out resObj) == false 
            || resObj == null 
            || resObj.m_Released == true)
        {
            Debug.LogErrorFormat("Err");
            return;
        }


#if UNITY_EDITOR
        if (go.name.EndsWith(Constants.FixSur_InstaniateGameObject))

        {
            go.name += Constants.FixSur_ResObject_m_Go;//回收过一次，加一个(Recycle)
        }

#endif

        //
        List<ResObj> lst=null;
        if (maxCacheCnt == 0)//不缓存
        {
            ReleaseAndRecycleResObj(key, destroyCache, resObj);
        }
        else//缓存
        {
            if (m_ResObjPoolDic.TryGetValue(resObj.m_Crc, out lst) == false || lst == null)//缓存过
            {
                lst=new List<ResObj>();
            
                m_ResObjPoolDic.Add( resObj.m_Crc, lst);
                    
            }

            if (resObj.m_Go != null)//没缓存过
            {
                if (recycleParent == true)
                {
                    resObj.m_Go.transform.SetParent(m_RecyclePoolTrans);
                }
                else
                {
                    resObj.m_Go.SetActive(false);
                }
            }

            if (maxCacheCnt < 0 || lst.Count < maxCacheCnt)//没满
            {
                lst.Add(resObj);
                resObj.m_Released = true;

                ResourceMgr.Instance.SubResItemRefCnt(resObj);
            }
            else//满了,不会进Pool
            {

                //回收
                ReleaseAndRecycleResObj( key,  destroyCache,  resObj);
            }
        }
    }


    /// <summary>
    /// 完全清空
    /// </summary>
    /// <param name="key"></param>
    /// <param name="destroyCache"></param>
    /// <param name="resObj"></param>
    void ReleaseAndRecycleResObj(int key,bool destroyCache,ResObj resObj)
    {
        m_ResObjDic.Remove(key);
        ResourceMgr.Instance.ReleaseResObject(resObj, destroyCache);
        resObj.Reset();
        m_ResObjPool.Recycle(resObj);
    }
    #endregion


}
