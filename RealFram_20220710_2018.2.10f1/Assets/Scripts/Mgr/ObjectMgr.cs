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

    #region 字段 属性
    /// <summary> 类对象池  </summary>
    Dictionary<Type, object> m_classPoolDic = new Dictionary<Type, object>();




    /// <summary>一种可能有多个</summary> 
    Dictionary<uint, List<ResObj>> m_resObjLstDic = new Dictionary<uint, List<ResObj>>();

    /// <summary>m_ResObjPoolDic拿不到就Spawn</summary>
    ClassObjectPool<ResObj> m_resObjPool = new ClassObjectPool<ResObj>(Constants.ClassObjectPool_RESOBJ_MAXCNT);

    /// <summary> InstaniateID, ResObject 所有实例的Object</summary> 
    Dictionary<int, ResObj> m_resObjDic = new Dictionary<int, ResObj>();

    /// <summary>正在异步加载中的数据</summary> 
    Dictionary<long, ResObj> m_asyncResObjDic = new Dictionary<long, ResObj>();
    #region Trans
    /// <summary>父节点</summary> 
    public Transform m_RecyclePoolTrans;
    /// <summary>场景节点，DontDestroyOnLoad(gameObject);</summary>
    public Transform m_SceneTrans;
    #endregion
    #endregion


    #region 生命
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
    #endregion



    #region GameObject
    /// <summary>
    /// ObjectMgr中创建的，有户籍？
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public bool IsCreateByObjectMgr(GameObject go)
    {
        return m_resObjDic[go.GetInstanceID()] != null;
    }

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
        ResObj resObj = GetResObjSync(crc);
        //Pool没有
        if (resObj == null)
        {
            resObj = SpawnResObj(true);
            resObj.m_Crc = crc;
            resObj.m_JmpClr = jmpClr;
            //ResouceManager提供加载方法
            resObj = ResourceMgr.Instance.LoadResObjSync(path, resObj);

            //if (resObj.m_ResItem !=null && resObj.m_ResItem.m_Obj != null)
            if ( resObj.m_ResItem.m_Obj != null)
            {
                GameObject go = resObj.m_ResItem.m_Obj as GameObject;
                resObj.m_Go = GameObject.Instantiate(go) as GameObject;
                resObj.m_OfflineData = go.GetComponent<OfflineData>();
            }

        }

        //父节点
        if (setScene)
        {
            resObj.m_Go.transform.SetParent(m_SceneTrans, false);
        }

        //保存
        int key = resObj.m_Go.GetInstanceID();
        if (m_resObjDic.ContainsKey(key) == false)
        {
            m_resObjDic.Add(key, resObj);
        }

        return resObj.m_Go;
    }

    /// <summary>
    /// Async实例对象
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cbInstaniate">完成时回调</param>
    /// <param name="priority">加载优先级</param>
    /// <param name="setSence">射到场景</param>
    /// <param name="jmpClr">转场景清楚</param>
    /// <param name="para1"></param>
    /// <param name="para2"></param>
    /// <param name="para3"></param>
    /// <param name="crc"></param>
    /// 
    public long AsyncInstaniateGameObject(string path,
        OnAsyncObject cbInstaniate,
        AsyncLoadResPriority priority,
        bool setSence = false,
        bool jmpClr = true,
        object para1 = null,
        object para2 = null,
        object para3 = null)
    {
        if (string.IsNullOrEmpty(path))
        {
            return 0;
        }

        uint crc = CRC32.GetCRC32(path);
        ResObj resObj = GetResObjSync(crc);//ResCnt有Add函数

        if (resObj != null)//get不到就spawn
        {
            if (setSence)
            {
                resObj.m_Go.transform.SetParent(m_SceneTrans);
            }

            if (cbInstaniate != null)
            {
                cbInstaniate(path, resObj.m_Go, para1, para2, para3);//返回给Mono的GameObject
            }
            return resObj.m_GUID;
        }
        long asyncGUID = Common.CreateGuid();
        resObj = SpawnResObj(true);
        resObj.m_Crc = crc;
        resObj.m_SetSceneParent = setSence;
        resObj.m_JmpClr = jmpClr;
        resObj.m_CBInstaniate = cbInstaniate;

        resObj.m_Para1 = para1;
        resObj.m_Para2 = para2;
        resObj.m_Para3 = para3;

        ResourceMgr.Instance.AsyncLoadResObj(path, resObj, OnLoadResObjFinished, priority);

        return asyncGUID;
    }
    //




    /// <summary>
    /// 释放内存
    /// </summary>
    /// <param name="go"></param>
    /// <param name="maxCacheCnt">如果要缓存，个数</param>
    /// <param name="destroyCache"> =>resItem  =>AB包资源及其依赖</param>
    /// <param name="recycleParent">比如UI，读取慢，就false，不放回去</param>
    public void UnloadGameObject(GameObject go, int maxCacheCnt = -1, bool destroyCache = false, bool recycleParent = true)
    {
        if (go == null)
        {
            return;
        }

        //go =>resobj
        int key = go.GetInstanceID();
        ResObj resObj = null;

        //分开写，定位Err
        if (m_resObjDic.TryGetValue(key, out resObj) == false)
        {
            Debug.LogErrorFormat("Err");
            return;
        }

        if (resObj == null)
        {
            Debug.LogErrorFormat("Err");
            return;
        }

        if (resObj.m_Released == true)
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
        List<ResObj> lst = null;
        if (maxCacheCnt == 0)//不缓存
        {
            UnloadResObj(key, destroyCache, resObj);
        }
        else//缓存
        {
            if (m_resObjLstDic.TryGetValue(resObj.m_Crc, out lst) == false || lst == null)//缓存过
            {
                lst = new List<ResObj>();

                m_resObjLstDic.Add(resObj.m_Crc, lst);

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
                UnloadResObj(key, destroyCache, resObj);
            }
        }
    }



    #endregion

    #region ResObj
    /// <summary>对应的ResObj是不是在异步加载中</summary>
    public bool IsAsyncResObj(long guid)
    {
        return m_asyncResObjDic[guid] != null;
    }

    /// <summary>
    /// 完全清空
    /// </summary>
    /// <param name="key"></param>
    /// <param name="destroyCache"></param>
    /// <param name="resObj"></param>
    void UnloadResObj(int key, bool destroyCache, ResObj resObj)
    {
        m_resObjDic.Remove(key);
        ResourceMgr.Instance.UnloadResObj(resObj, destroyCache);
        resObj.Reset();
        m_resObjPool.Recycle(resObj);

    }

    /// <summary>
    /// 同步加载
    /// </summary>
    /// <param name="crc"></param>
    /// <returns></returns>
    ResObj GetResObjSync(uint crc)
    {
        //Get lst
        List<ResObj> lst = null;
        if (m_resObjLstDic.TryGetValue(crc, out lst) == false || lst == null || lst.Count <= 0)
        {
            return null;
        }


        ResourceMgr.Instance.AddResItemRefCnt(crc);
        //Get ResObgj From lst
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
                go.name.Replace(Constants.FixSur_ResObject_m_Go, "");//到Editor
#endif


            }
            //
            if (System.Object.ReferenceEquals(resObj.m_OfflineData, null) == false)
            { 
                resObj.m_OfflineData.Reset();
            }
        }


        return resObj;

    }

    /// <summary>
    /// Get不要就Spawn
    /// </summary>
    /// <param name="createEmptyPool"></param>
    /// <returns></returns>
    ResObj SpawnResObj(bool createEmptyPool = true)
    {
        return m_resObjPool.Spawn(createEmptyPool);
    }

    /// <summary>
    /// Mgr级取消异步
    /// </summary>
    /// <param name="resObj_m_GUID"></param>
    void AsyncCheckCancelLoadResObj(int resObj_m_GUID)
    {
        ResObj resObj = null;
        if (m_asyncResObjDic.TryGetValue(resObj_m_GUID, out resObj) == true
            && ResourceMgr.Instance.AsyncCheckCancelLoadResObj(resObj) == true)
        {
            m_asyncResObjDic.Remove(resObj.m_GUID);
            resObj.Reset();
            m_resObjPool.Recycle(resObj);
        }
    }

    void OnLoadResObjFinished(string path, ResObj resObj, object para1 = null, object para2 = null, object para3 = null)
    {
        if (resObj == null)
        {
            return;
        }

        if (resObj.m_ResItem.m_Obj == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Err");
#endif
        }
        else
        {
            resObj.m_Go = GameObject.Instantiate(resObj.m_ResItem.m_Obj) as GameObject;
            resObj.m_GUID = resObj.m_Go.GetInstanceID();
        }

        AsyncCheckCancelLoadResObj(resObj.m_GUID);


        //父节点
        if (resObj.m_Go != null && resObj.m_SetSceneParent == true && m_SceneTrans != null)
        {
            GameObject go = resObj.m_Go as GameObject;
            go.transform.SetParent(m_SceneTrans);
            resObj.m_OfflineData = go.GetComponent<OfflineData>();
        }

        //存入m_ResObjDic
        if (resObj.m_CBInstaniate != null)
        {
            int key = resObj.m_GUID;
            if (m_resObjDic.ContainsKey(key) == false)
            {
                m_resObjDic.Add(key, resObj);
            }

            resObj.m_CBInstaniate(path, resObj.m_Go, resObj.m_Para1, resObj.m_Para2, resObj.m_Para3);
        }
    }
    #endregion


    #region ClassObjectPool


    /// <summary>
    ///  删m_resObjLstDic
    /// </summary>
    public void ClearCache()
    {
        List<uint> crcLst=new List<uint>();

       
        foreach (var item in m_resObjLstDic) //删ResObj
        {
            uint crc = item.Key;
            List<ResObj> lst = item.Value;
            for (int i = lst.Count - 1; i >= 0; i--)
            { 
                ResObj resObj = lst[i];
                if (System.Object.ReferenceEquals(resObj.m_Go, null) == false && resObj.m_JmpClr == true) //删Go和resObj
                {
                   
                    m_resObjDic.Remove(resObj.m_Go.GetInstanceID());
                    GameObject.Destroy(resObj.m_Go);

                    resObj.Reset(); 
                    m_resObjPool.Recycle(resObj);
                    lst.Remove(resObj);
                }
             }

            if (lst.Count <= 0)
            {
                crcLst.Add(crc);
            }
        }

       
        for (int i = 0; i < crcLst.Count; i++)  //删Pool
        {
            uint crc=crcLst[i];
            if (m_resObjLstDic.ContainsKey(crc) == true)
            { 
                m_resObjLstDic.Remove(crc);
            }
        }
        crcLst.Clear();
    }


    /// <summary>
    /// 创建类对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="maxCnt"></param>
    /// <returns></returns>
    public ClassObjectPool<T> GetOrNewClassObjectPool<T>(int maxCnt) where T : class, new()
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
    public void ClearAllObjectsInPool(uint crc)
    {
        List<ResObj> lst = null;
        if ( m_resObjLstDic.TryGetValue(crc, out lst)==false || lst == null)
        { 
              return;      
        }


        for (int i = lst.Count - 1; i >= 0; i--)
        {
            ResObj resObj = lst[i];
            if (resObj.m_JmpClr)
            {
                lst.Remove(resObj);
                m_resObjDic.Remove(resObj.m_Go.GetInstanceID());
                GameObject.Destroy(resObj.m_Go);              
                //
                resObj.Reset();
                m_resObjPool.Recycle(resObj);
            }
        }

        if (lst.Count <= 0)
        {
            m_resObjLstDic.Remove(crc);
        }
    }
    #endregion


    #region Offlinedata
    public OfflineData GetOfflineData(GameObject go)
    {
        OfflineData data = null;
        ResObj resObj = null;
        if (m_resObjDic.TryGetValue(go.GetInstanceID(), out resObj) == true && resObj != null )
        {
            data = resObj.m_OfflineData;
        }
        return data;
    }

    public OfflineData GetOfflineData(ResObj resObj)
    {
        return GetOfflineData( resObj.m_Go);
    }
    #endregion

}
