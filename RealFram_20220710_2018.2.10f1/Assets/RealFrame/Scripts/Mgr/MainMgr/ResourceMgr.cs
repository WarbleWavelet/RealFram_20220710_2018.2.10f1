/****************************************************
    文件：ResourceMgr.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/13 2:22:47
	功能：资源管理器
       1：以双向链表为基础的资源池（基于使用率）
       2：基础资源同步加载
       3：基本资源卸载
       4：基础资源异步加载(加载谁的顺序，控制每帧的加载)
       5：清空缓存
       6：预加载
       6：为ObjectManager提供的同步异步资源加载
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object=UnityEngine.Object;//经常判null检测




/// <summary>Async一般资源</summary>
public delegate void OnAsyncObject(string path, Object obj, object para1 = null, object para2 = null, object para3=null);
/// <summary>Async可实例资源</summary>
public delegate void OnAsyncResObj(string path, ResObj resObj, object para1 = null, object para2 = null, object para3 = null);

public class ResourceMgr : Singleton<ResourceMgr>
{

    #region 字属
    /// <summary>在内存中的资源,没被引用（常用但目前没引用）</summary>
    protected MapLst<ResItem> m_NoRefResItemLst = new MapLst<ResItem>();

    /// <summary>在内存中的资源，被引用</summary>
    public Dictionary<uint, ResItem> m_RefResItemDic { get; set; } = new Dictionary<uint, ResItem>();

    /// <summary>从哪里加载：AB包 、 Editor</summary>
    bool m_loadFromAB = false;

    #region Async
    /// <summary>某个MonoBehaviour（它要开协程）</summary>
    MonoBehaviour m_startMono;


    /// <summary>正在异步加载的资源参数表</summary>
    Dictionary<uint, AsyncLoadResPara> m_asyncLoadResParaDic = new Dictionary<uint, AsyncLoadResPara>();
    /// <summary>异步加载Res的列表</summary>
    List<AsyncLoadResPara>[] m_asyncLoadResParaLst = new List<AsyncLoadResPara>[(int)AsyncLoadResPriority.Count];
    /// <summary>异步对象池</summary>
    public ClassObjectPool<AsyncLoadResPara> m_AsyncLoadResParaPool = new ClassObjectPool<AsyncLoadResPara>(Constants.ClassObjectPool_AsyncLoadResPara_MAXCNT);

    public ClassObjectPool<AsyncTotalCallBack> m_AsyncLoadResCallBackPool = new ClassObjectPool<AsyncTotalCallBack>(Constants.ClassObjectPool_AsyncLoadResCallBack_MAXCNT);


    #endregion


    #endregion


    #region 生命
    /// <summary>
    /// 开始协程的条件，传入自身this
    /// </summary>
    /// <param name="mono"></param>
    public void InitMgr(MonoBehaviour mono)
    {

        for (int i = 0; i < (int)AsyncLoadResPriority.Count; i++)
        {
            m_asyncLoadResParaLst[i] = new List<AsyncLoadResPara>();
        }
        m_startMono = mono;
        m_startMono.StartCoroutine(AsyncLoadObject());


    }
    #endregion



    #region ResObj


    /// <summary>
    /// ObjectMgr用到
    /// </summary>
    /// <param name="path"></param>
    /// <param name="resObj"></param>
    /// <param name="cb"></param>
    /// <param name="priority"></param>
    public void AsyncLoadResObj(string path,
        ResObj resObj,
        OnAsyncResObj cb,
        AsyncLoadResPriority priority)
    {
        ResItem resItem = GetResItem(resObj.m_Crc);

        if (resItem != null)
        {
            resObj.m_ResItem = resItem;
            if (cb != null)
            {
                cb(path, resObj);
            }

            return;
        }

        //
        AsyncLoadResPara para = null;
        uint crc = resObj.m_Crc;
        if (m_asyncLoadResParaDic.TryGetValue(crc, out para) == false || para == null)//新的资源加载参数类
        {
            para = SpawnAsyncLoadPara(path, priority);
        }

        //回调，接着会Mono 自动进行 AsyncLoadResource
        AsyncTotalCallBack _cb = m_AsyncLoadResCallBackPool.Spawn(true);
        _cb.m_ResObj = resObj;
        _cb.m_AsyncResObj = cb;
        para.m_CbLst.Add(_cb);
    }


    /// <summary>
    /// Mgr级取消异步
    /// </summary>
    /// <param name="resObj"></param>
    /// <returns></returns>
    public bool AsyncCheckCancelLoadResObj(ResObj resObj)
    {
        AsyncLoadResPara para = null;
        if (m_asyncLoadResParaDic.TryGetValue(resObj.m_Crc, out para) == true
            && m_asyncLoadResParaLst[(int)para.m_Priority].Contains(para) == true)
        {
            List<AsyncTotalCallBack> lst = para.m_CbLst;
            for (int i = lst.Count-1; i >= 0; i--)//方便移除
            {
                AsyncTotalCallBack cb = lst[i];
                if (cb != null && cb.m_ResObj == resObj)
                {
                    cb.Reset();
                    m_AsyncLoadResCallBackPool.Recycle(cb);
                    para.m_CbLst.Remove(cb);
                }

            }

            if (lst.Count <= 0)
            {
                m_asyncLoadResParaLst[(int)para.m_Priority].Remove(para);
                para.Reset();//Ocean和上面对调顺序，怀疑因para.m_Priority出错
                m_AsyncLoadResParaPool.Recycle(para);
                //
                m_RefResItemDic.Remove(resObj.m_Crc);

                return false;
            }
        }

        return false;
    }


    /// <summary>
    /// 同步加载资源，给ObjectMgr用的（RefCnt）
    /// </summary>
    /// <param name="path"></param>
    /// <param name="resObj"></param>
    /// <returns></returns>
    public ResObj LoadResObjSync(string path, ResObj resObj)
    {
        if (resObj == null)
        {
            return null;
        }

        uint crc= (resObj.m_Crc==0) ? CRC32.GetCRC32(path) : resObj.m_Crc;

        ResItem resItem = GetResItem(crc);


   
        if (resItem != null)     //Get得到就Get
        {
            resObj.m_ResItem = resItem;
            return resObj;
        }

      
        Object obj = null;  //Get不到就到下一层Get
#if UNITY_EDITOR//测试从Editor加载            


        if (m_loadFromAB == false)
        {
            resItem = AssetBundleMgr.Instance.GetResItem(crc);//迷惑m_loadFromAB == false为什么还使用AssetBundleMgr
            if (resItem != null && resItem.m_Obj != null)
            {
                obj = resItem.m_Obj as Object;
            }
            else
            {
                NewResItemAndObj<Object>(ref resItem, ref obj, path, crc);
            }
        }
#endif
       
        if (obj == null) //Get不到就Load
        {
            resItem = AssetBundleMgr.Instance.LoadResItem(crc);
            if (resItem != null && resItem.m_AB != null)
            {
                if (resItem.m_Obj != null)
                {
                    obj = resItem.m_Obj as Object;
                }
                else
                {
                    obj = resItem.m_AB.LoadAsset<Object>(resItem.m_AssetName);
                }
            }
            else
            {
                resItem = new ResItem(crc);

            }
        }
   

        CacheResItem(path, ref resItem, crc, obj);     //缓存
        resObj.m_ResItem = resItem;
        resItem.m_JmpClr = resObj.m_JmpClr;


        return resObj;
    }

    public bool UnloadResObj(ResObj resObj, bool destroyCache)
    {
        if (resObj == null) 
        {
            return false;
        }

   
        uint crc = resObj.m_Crc;     //  crc => resItem
        ResItem resItem = null;

        if (m_RefResItemDic.TryGetValue(crc, out resItem) == false || resItem == null)
        {
            Debug.LogErrorFormat("资源不存在，或被多次清空");
            return false;
        }

        GameObject.Destroy(resObj.m_Go);
        resItem.RefCnt--;
        UnloadResItem(resItem, destroyCache);



        return true;
    }



    #endregion


    #region ResItem

    /// <summary>
    /// <summary>
    /// 比如跳转场景时，清内存
    /// </summary>
    public void ClearAllResItem()
    {
        //if (m_NoRefLst.Count() > 0)
        //{
        //    ResItem resItem=m_NoRefLst.GetTail();
        //    DestroyResItem(resItem, resItem.m_Clear);
        //    m_NoRefLst.Pop();
        //}
        List<ResItem> lst = new List<ResItem>();
        foreach (ResItem resItem in m_RefResItemDic.Values)
        {
            if (true == resItem.m_JmpClr)
            {
                lst.Add(resItem);
            }
        }

        foreach (ResItem resItem in lst)
        {
            UnloadResItem(resItem, true);
        }
    }

    /// <summary>
    /// Get引用过的ResItem，获取 异步、同步、预加载
    /// </summary>
    /// <param name="crc"></param>
    /// <param name="addCnt"></param>
    /// <returns></returns>
    ResItem GetResItem(uint crc, int addCnt = 1)
    {
        ResItem resItem = null;
        if (m_RefResItemDic.TryGetValue(crc, out resItem)==true && resItem != null)
        {

                resItem.RefCnt += addCnt;
                resItem.m_LastUseTime = Time.realtimeSinceStartup;

                //安全判断，理论进不来
                if (resItem.RefCnt <= 1)
                {
                    m_NoRefResItemLst.Remove(resItem);
                    // noRefLst.AddToHead(resItem);
                }
            
        }

        return resItem;
    }

    /// <summary>
    /// 缓存（RefCnt+1）
    /// </summary>
    /// <param name="name"></param>
    /// <param name="resItem"></param>
    /// <param name="crc"></param>
    /// <param name="obj"></param>
    /// <param name="addRefCnt"></param>

    void CacheResItem(string name, ref ResItem resItem, uint crc, UnityEngine.Object obj, int addRefCnt = 1)
    {
         Washout();
        if (resItem == null)
        {

            UnityEngine.Debug.LogError(
                this.GetType().ToString() 
                + "." 
                + new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().ToString());//类名.方法名


            return;

        }

        if (obj == null)
        {
            UnityEngine.Debug.LogError(this.GetType().ToString() + "." + new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().ToString());//类名.方法名

     return ;
        }

        resItem.m_Obj = obj;
        resItem.m_LastUseTime = Time.realtimeSinceStartup;
        resItem.m_Guid = obj.GetInstanceID();
        resItem.RefCnt += addRefCnt;

        //
        ResItem oldResItem = null;
        if (m_RefResItemDic.TryGetValue(crc, out oldResItem))//Update
        {
            m_RefResItemDic[crc] = resItem;
        }
        else//add
        {
            m_RefResItemDic.Add(crc, resItem);
        }
    }

    /// <summary>
    /// 缓存太多，清理一些
    /// </summary>
    void Washout()
    {
        //游戏内存/设备总内存=60%                              
        //float destroyPercent = 0.8f;

        //if (m_NoRefResItemLst.Count() <= 0)
        //{
        //    return;
        //}

        //ResItem resItem = m_NoRefResItemLst.GetTail();
        //m_NoRefResItemLst.Pop();
        //UnloadResItem(resItem, true);
        WashOutByCount();

    }

    void WashOutByCount()
    {
        while (m_NoRefResItemLst.Count() >= Constants.MaxCacheCnt+1)
        {
            for (int i = 0; i < Constants.MaxCacheCnt/2; i++)
            {
                ResItem resItem = m_NoRefResItemLst.GetTail();
                UnloadResItem(resItem, true);
            }
        }
    }

    /// <summary>
    /// 回收资源(是否有其他Object在引用)
    /// </summary>
    /// <param name="resItem"></param>
    /// <param name="destroyCache">是否要删除缓存</param>
    protected void UnloadResItem(ResItem resItem, bool destroyCache = false)
    {

        if (resItem == null || resItem.RefCnt > 0)
        {
            return;
        }



        //对NoRef
        if (destroyCache == false)
        {
            m_NoRefResItemLst.AddToHead(resItem);
             return;
        }
        
        //对Ref
        if (m_RefResItemDic.Remove(resItem.m_Crc) == false)
        {
            return;
        }

        #region 最占内存的两个地方
        m_NoRefResItemLst.Remove(resItem);
        //上层Mgr
        AssetBundleMgr.Instance.UnloadAB(resItem);
        ObjectMgr.Instance.ClearAllObjectsInPool(resItem.m_Crc);
            //下层引用，对象置空
        if (resItem.m_Obj != null)
        {
        
/**   只能清引用计数
#if UNITY_EDITOR
            Resources.UnloadAsset(resItem.m_Obj);//UNITY_EDITOR下为长贮内存，上面不足以清掉对该资源的引用（静态）；这段会延时清存
#endif     
            resItem.m_Obj = null;
**/
         resItem.m_Obj = null;
#if UNITY_EDITOR
            Resources.UnloadUnusedAssets();//UNITY_EDITOR下为长贮内存，上面不足以清掉对该资源的引用（静态）；这段会延时清存
#endif     
     
        }
         #endregion
        
    }

    /// <summary>
    ///不需要实例化的资源的卸载<para />
    ///遍历方法<para />
    /// </summary>
    /// <param name="obj">Unity中的Object</param>
    /// <param name="destroyCache"></param>
    /// <returns></returns>
    public bool UnloadResItemByObject(UnityEngine.Object obj, bool destroyCache = false)
    {
        if (obj == null)
        {
            return false;
        }

        ResItem resItem = null;
        foreach (ResItem _resItem in m_RefResItemDic.Values)//耗性能
        {
            if (_resItem.m_Guid == obj.GetInstanceID())
            {
                resItem = _resItem;
            }
        }

        if (resItem == null)
        {
            Debug.LogErrorFormat("该资源不存在缓存中");
            return false;
        }

        resItem.RefCnt--;//
        UnloadResItem(resItem, destroyCache);

        return true;

    }

    /// <summary>
    /// 不需要实例化的资源的卸载<para />
    /// TryGetValue<para />
    /// </summary>
    /// <param name="path"></param>
    /// <param name="destroyCache"></param>
    /// <returns></returns>
    public bool UnloadResItemByPath(string path, bool destroyCache = false)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        // path => crc => resItem
        uint crc = CRC32.GetCRC32(path);
        ResItem resItem = null;

        if (m_RefResItemDic.TryGetValue(crc, out resItem) == false || resItem == null)
        {
            Debug.LogErrorFormat("资源不存在，或被多次清空");
        }


        resItem.RefCnt--;
        UnloadResItem(resItem, destroyCache);

        return true;

    }

    #region ResItemRefCnt
    public int AddResItemRefCnt(uint crc, int cnt = 1)
    {
        ResItem resItem = null;
        if (m_RefResItemDic.TryGetValue(crc, out resItem) == false || resItem == null)
        {
            return 0;
        }

        resItem.RefCnt += cnt;
        resItem.m_LastUseTime = DateTime.Now.Ticks;

        return resItem.RefCnt;
    }

    public int AddResItemRefCnt(ResObj resObj, int cnt = 1)
    {
        if (resObj == null)
        {
            return 0;
        }
        return AddResItemRefCnt(resObj.m_Crc, cnt);
    }

    public int SubResItemRefCnt(uint crc, int cnt = 1)
    {
        ResItem resItem = null;
        if (m_RefResItemDic.TryGetValue(crc, out resItem) == false || resItem == null)
        {
            return 0;
        }

        resItem.RefCnt -= cnt;

        return resItem.RefCnt;
    }

    public int SubResItemRefCnt(ResObj resObj, int cnt = 1)
    {
        if (resObj == null)
        {
            return 0;
        }
        return SubResItemRefCnt(resObj.m_Crc, cnt);
    }
    #endregion


    #endregion

    #region Object
    /// <summary>
    /// 给ResourceMgr资源加载完成回调
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cb"></param>
    /// <param name="priority"></param>
    /// <param name="para1"></param>
    /// <param name="para2"></param>
    /// <param name="para3"></param>
    /// <param name="crc"></param>
    public void AsyncLoadObject(string path,
        OnAsyncObject cb,
        AsyncLoadResPriority priority,
        bool isSprite=false,
        object para1 = null,
        object para2 = null,
        object para3 = null,
        uint crc = 0)
    {
        if (crc == 0)
        {
            crc = CRC32.GetCRC32(path);

        }

        ResItem resItem = GetResItem(crc);

        if (resItem != null)
        {
            if (cb != null)
            {
                cb(path, resItem.m_Obj, para1, para2, para3);
            }
            return;
        }
        AsyncLoadResPara para = null;
        if (m_asyncLoadResParaDic.TryGetValue(crc, out para) == false || para == null)
        {
            para = SpawnAsyncLoadPara(path, priority,isSprite);

        }

        //回调
        AsyncTotalCallBack _cb = m_AsyncLoadResCallBackPool.Spawn(true);
        _cb.m_AsyncObject = cb;
        _cb.m_Para1 = para1;
        _cb.m_Para2 = para2;
        _cb.m_Para3 = para3;
        para.m_CbLst.Add(_cb);

    }



    /// <summary>
    /// 异步加载资源（会一直运行）
    /// </summary>
    /// <returns></returns>
    IEnumerator AsyncLoadObject()
    {
        List<AsyncTotalCallBack> cbLst = new List<AsyncTotalCallBack>();
        long lastYieldTime = System.DateTime.Now.Ticks;
        while (true)
        {
            bool haveYield = false;//内层yield后外层不用yield
           
            for (int i = 0; i < (int)AsyncLoadResPriority.Count; i++) //不同优先级的回调列表，High Middle Low
            {
                if (m_asyncLoadResParaLst[(int)AsyncLoadResPriority.High].Count > 0) //优先high
                {
                    i = (int)AsyncLoadResPriority.High;
                }
                else if (m_asyncLoadResParaLst[(int)AsyncLoadResPriority.Middle].Count > 0) //次优先Middle
                {
                    i = (int)AsyncLoadResPriority.Middle;
                }

                List<AsyncLoadResPara> paraLst = m_asyncLoadResParaLst[i];
                if (paraLst.Count <= 0)
                {
                    continue;//继续空跑
                }


                AsyncLoadResPara para = paraLst[0];//Get cbLst 要加载的Item
                paraLst.RemoveAt(0);
                cbLst = para.m_CbLst;

            
                Object obj = null;    //obj => resItem
                ResItem resItem = null;
#if UNITY_EDITOR
                if (m_loadFromAB == false)
                {

                    if (para.m_Sprite == true)// 特殊：Sprite 会不能直接等于 asset
                    {
                        obj = LoadAssetByEditor<Sprite>(para.m_Path);
                    }
                    else
                    {
                        obj = LoadAssetByEditor<Object>(para.m_Path);
                    }

                    yield return new WaitForSeconds(0.5f);//模拟异步

                    resItem = AssetBundleMgr.Instance.GetResItem(para.m_Crc);

                    NewResItem(ref resItem, para.m_Crc);

                }

#endif             
                if (null == obj)   //m_loadFromAB==true
                {
                    resItem = AssetBundleMgr.Instance.LoadResItem(para.m_Crc);

                    if (null != resItem && null != resItem.m_AB)
                    {
                        AssetBundleRequest abReq = null;
                        if (para.m_Sprite == true)// 特殊：Sprite 会不能直接等于 asset
                        {
                           // abReq = resItem.m_AB.LoadAssetAsync<Sprite>(resItem.m_ABName);
                            abReq = resItem.m_AB.LoadAssetAsync<Sprite>(  resItem.m_AssetName);
                        }
                        else
                        {
                            abReq = resItem.m_AB.LoadAssetAsync(resItem.m_AssetName);
                        }
                        yield return abReq;

                        if (abReq.isDone)
                        {
                            obj = abReq.asset;

                        }
                        lastYieldTime = System.DateTime.Now.Ticks;

                    }
                }
                //缓存资源
                CacheResItem(para.m_Path, ref resItem, para.m_Crc, obj, cbLst.Count);

                // AsyncLoadResCallBack
                for (int j = 0; j < cbLst.Count; j++)
                {
                    AsyncTotalCallBack cb = cbLst[j];
                    if (cb != null && cb.m_AsyncResObj != null && cb.m_ResObj != null)//LoadObject（可实例资源）的回调
                    {
                        ResObj resObj = cb.m_ResObj;
                        resObj.m_ResItem = resItem;
                        cb.m_AsyncResObj(para.m_Path, resObj, resObj.m_Para1, resObj.m_Para2, resObj.m_Para3);
                        cb.m_AsyncResObj = null;
                        resObj = null;
                    }

                    if (cb != null && cb.m_AsyncObject != null)//LoadObject（一般资源）的回调
                    {
                        cb.m_AsyncObject(para.m_Path, obj, cb.m_Para1, cb.m_Para2, cb.m_Para3);
                        cb.m_AsyncObject = null;
                    }



                    //回收
                    cb.Reset();
                    m_AsyncLoadResCallBackPool.Recycle(cb);
                }

                //移除回收
                obj = null;
                cbLst.Clear();
                m_asyncLoadResParaDic.Remove(para.m_Crc);

                para.Reset();
                m_AsyncLoadResParaPool.Recycle(para);

                //防止资源太大，卡点时间
                if (System.DateTime.Now.Ticks - lastYieldTime > Constants.MAXASYNCLOADRESTIME)
                {
                    yield return null;
                    lastYieldTime = System.DateTime.Now.Ticks;
                    haveYield = true;

                }
            }

            //卡着加载
            if (haveYield == false && System.DateTime.Now.Ticks - lastYieldTime > Constants.MAXASYNCLOADRESTIME)
            {
                lastYieldTime = System.DateTime.Now.Ticks; ;
                yield return null;
            }
        }
    }


    /// <summary>
    /// 基础资源同步加载(不用实例化，Texture等)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns>resItem.mObj</returns>
    public T LoadResource<T>(string path) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }
        //同步加载
        uint crc = CRC32.GetCRC32(path);
        ResItem resItem = GetResItem(crc);

        //
        if (resItem != null)
        {
            return resItem.m_Obj as T;

        }

        T obj = null;
#if UNITY_EDITOR//测试从Editor加载            
        if (m_loadFromAB == false)
        {

            resItem = AssetBundleMgr.Instance.GetResItem(crc);  //StreamAsset ,实际打包位置要外迁移到AssetBundle
            if (resItem != null && resItem.m_AB != null)
            {
                if (resItem.m_Obj != null)
                {
                    obj = resItem.m_Obj as T;
                }
                else
                {
                    obj = resItem.m_AB.LoadAsset<T>(resItem.m_AssetName);

                }
            }
            else
            {
                NewResItemAndObj<T>(ref resItem, ref obj, path, crc);


            }

        }
#endif
        if (obj == null)
        {
            resItem = AssetBundleMgr.Instance.LoadResItem(crc);
            if (resItem != null && resItem.m_AB != null)
            {
                if (resItem.m_Obj != null)
                {
                    obj = resItem.m_Obj as T;
                }
                else
                {
                    obj = resItem.m_AB.LoadAsset<T>(resItem.m_AssetName);
                }
            }
        }
        //缓存
        CacheResItem(path, ref resItem, crc, obj);


        return obj;
    }













    /// <summary>
    /// 比如跳转场景时
    /// </summary>
    public void PreLoadObject(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        //同步加载
        uint crc = CRC32.GetCRC32(path);
        ResItem resItem = GetResItem(crc,0);//预加载不需要引用计数
        if (resItem != null)
        {
            return;
        }


       Object obj = null;
#if UNITY_EDITOR//测试从Editor加载            

        if (m_loadFromAB == false)
        {

            resItem = AssetBundleMgr.Instance.GetResItem(crc);
            if (resItem!=null && resItem.m_Obj != null)
            {
                obj = resItem.m_Obj;
            }
            else
            {
                NewResItemAndObj<Object>(ref resItem,ref obj,path,crc);
            }
        }
#endif
        if (obj == null)
        {
            resItem = AssetBundleMgr.Instance.LoadResItem(crc);
            if (resItem != null && resItem.m_AB != null)
            {
                if (resItem.m_Obj != null)
                {
                    obj = resItem.m_Obj ;
                }
                else
                {
                    obj = resItem.m_AB.LoadAsset<Object>(resItem.m_ABName);
                }
            }
        }
        //缓存
        CacheResItem(path, ref resItem, crc, obj);
        // 预加载后面要用，不清存
        resItem.m_JmpClr = false;
        //ReleaseResItem(obj, false);//每次写在持有对象累
        UnloadResItemByPath(path, false);
    }
    #endregion


    #region Para
    /// <summary>
    /// 新增资源加载参数项
    /// </summary>
    /// <param name="path"></param>
    /// <param name="resObj"></param>
    /// <param name="priority"></param>
    AsyncLoadResPara SpawnAsyncLoadPara(string path,  AsyncLoadResPriority priority, bool isSprite=false)
    {
        AsyncLoadResPara para = null;
        uint crc = CRC32.GetCRC32(path);
        para = m_AsyncLoadResParaPool.Spawn(true);
        para.m_Crc = crc;
        para.m_Path = path;
        para.m_Priority = priority;
        para.m_Sprite = isSprite;
        //

        m_asyncLoadResParaDic.Add(crc, para);
        m_asyncLoadResParaLst[(int)priority].Add(para);

        return para;
    }
    #endregion



    #region LoadFromAB
    /// <summary>
    /// 设置AB包
    /// </summary>
    /// <param name="state"></param>
    public void SetLoadFromAB(bool state = true)
    {
        m_loadFromAB = state;
    }
    /// <summary>
    /// 加载Ab包
    /// </summary>
    /// <returns></returns>
    public bool GetLoadFromAB()
    {
        return m_loadFromAB;
    }
    #endregion

    #region 打包位移外迁，还要在编辑器下使用资源


#if UNITY_EDITOR
    private void NewResItemAndObj<T>(ref ResItem resItem,
        ref T _t,
        string path, uint crc) where T : UnityEngine.Object
    {
        if (resItem == null)
        {
            resItem = new ResItem(crc);
        }

        _t = LoadAssetByEditor<T>(path);

    }

    private void NewResItem(ref ResItem resItem, uint crc)
    {
        if (resItem == null)
        {
            resItem = new ResItem(crc);
        }
    }


    /// <summary>
    /// 从UnityEditor.AssetDatabase加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    T LoadAssetByEditor<T>(string path) where T : UnityEngine.Object
    {
        return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
    }
#endif


    #endregion
}


#region 链表层
/// <summary>
/// 双向链表节点
/// </summary>
/// <typeparam name="T"></typeparam>
public class DoubleLinkedListNode<T> where T : class, new()
{
    public DoubleLinkedListNode<T> Pre = null;
    public DoubleLinkedListNode<T> Nxt = null;
    /// <summary>当前节点</summary>	
    public T SelfNode;

}
/// <summary>
/// 双链表
/// </summary>
/// <typeparam name="T"></typeparam>
public class DoubleLinkedList<T> where T : class, new()
{

    /// <summary>头节点</summary>	
    public DoubleLinkedListNode<T> Head = null;

    /// <summary>尾结点</summary>
    public DoubleLinkedListNode<T> Tail = null;
    /// <summary>双链表Pool</summary> 

    public ClassObjectPool<DoubleLinkedListNode<T>> DLLNPool = ObjectMgr.Instance.GetOrNewClassObjectPool<DoubleLinkedListNode<T>>(Constants.ClassObjectPool_MAXCNT);

    /// <summary>当前数量</summary>	
    private int cnt;
    public int Cnt
    {
        get
        {
            return cnt;
        }

    }

    #region AddTo
    public DoubleLinkedListNode<T> AddToHead(T node)
    {
        DoubleLinkedListNode<T> _node = DLLNPool.Spawn(true);
        _node.Pre = null;
        _node.Nxt = null;
        _node.SelfNode = node;
        return AddToHead(_node);
    }

    public DoubleLinkedListNode<T> AddToHead(DoubleLinkedListNode<T> node)
    {
        if (node == null)
        {
            return null;
        }
        //
        if (Head == null)
        {
            Head = Tail = node;
        }
        else
        {

            node.Nxt = Head;
            Head.Pre = node;
            Head = node;
        }
        cnt++;

        return null;
    }


    public DoubleLinkedListNode<T> AddToTail(T node)
    {
        DoubleLinkedListNode<T> _node = DLLNPool.Spawn(true);
        _node.Pre = null;
        _node.Nxt = null;
        _node.SelfNode = node;
        return AddToTail(_node);
    }

    public DoubleLinkedListNode<T> AddToTail(DoubleLinkedListNode<T> node)
    {
        if (node == null)
        {
            return null;
        }
        //
        if (Head == null)
        {
            Head = Tail = node;
        }
        else
        {
            node.Pre = Tail;
            Tail.Nxt = node;
            Tail = node;
        }
        cnt++;

        return null;
    }
    #endregion

    public void RemoveNode(DoubleLinkedListNode<T> node)
    {
        if (node == null)
        {
            return;
        }
        if (Head == node)
        {
            Head = node.Nxt;
        }
        if (Tail == node)
        {
            Tail = node.Pre;
        }

        if (node.Pre != null)
        {
            node.Pre.Nxt = node.Nxt;
        }

        node.Pre = node.Nxt = null;
        node.SelfNode = null;
        DLLNPool.Recycle(node);
        cnt--;
    }

    public void MoveToHead(DoubleLinkedListNode<T> node)
    {
        if (node == null)
        {
            return;
        }
        if (Head == node)
        {
            return;
        }
        if (node.Pre == null && node.Nxt == null)
        {
            return;
        }
        if (Tail == node)
        {
            Tail = node.Pre;//一个就为null

        }

        if (node.Pre != null)
        {
            node.Pre.Nxt = node.Nxt;
        }

        if (node.Nxt != null)
        {
            node.Nxt.Pre = node.Pre;
        }

        node.Pre = null;
        node.Nxt = Head;
        Head.Pre = node;
        Head = node;

        if (Tail == null)
        {
            Tail = Head;

        }
    }
}
#endregion


#region MapLst
/// <summary>
/// 管理双链表和节点
/// </summary>
/// <typeparam name="T"></typeparam>
public class MapLst<T> where T : class, new()
{
    DoubleLinkedList<T> lst = new DoubleLinkedList<T>();
    Dictionary<T, DoubleLinkedListNode<T>> DLLNDic = new Dictionary<T, DoubleLinkedListNode<T>>();


    #region 增
    /// <summary>
    /// 增
    /// </summary>
    /// <param name="node"></param>
    public void AddToHead(T node)
    {
        DoubleLinkedListNode<T> _node = null;
        if (DLLNDic.TryGetValue(node, out _node) && _node != null) //不包含直接到到前面
        {
            lst.AddToHead(node);
            return;
        }
        else 
        {
            lst.AddToHead(node);
            DLLNDic.Add(node, lst.Head);
        }
    }
    #endregion

    #region 改
    public bool MoveToHead(T node)
    {
        DoubleLinkedListNode<T> _node = null;
        if (DLLNDic.TryGetValue(node, out _node) && _node != null)
        {
            return false;
        }
        else
        {
            lst.MoveToHead(_node);
            return true;
        }
    }
    #endregion  

    #region 查
    /// <summary>
    /// 获取尾部节点
    /// </summary>
    /// <returns></returns>
    public T GetTail()
    {
        return lst.Tail == null ? null : lst.Tail.SelfNode;
    }

  /// <summary>
    /// count
    /// </summary>
    /// <returns></returns>
    public int Count()
    {
        return lst.Cnt;
    }

    /// <summary>
    /// 包含
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public bool Contains(T node)
    {
        DoubleLinkedListNode<T> _node = null;
        if (DLLNDic.TryGetValue(node, out _node) == false || _node == null)
        {
            return false;
        }
        return true;


    }
    #endregion

    #region 删

    /// <summary>
    /// 删除最后一个，弹出
    /// </summary>
    public void Pop()
    {

        if (lst.Tail.SelfNode != null)
        {
            Remove(lst.Tail.SelfNode);
        }
    }



    /// <summary>
    /// 删除节点
    /// </summary>
    /// <param name="node"></param>
    public void Remove(T node)
    {
        DoubleLinkedListNode<T> _node = null;
        if (DLLNDic.TryGetValue(node, out _node) == false || _node == null)
        {
            return;
        }
        lst.RemoveNode(_node);
        DLLNDic.Remove(node);
    }

    /// <summary>
    /// 析构函数，自动清除，Mono初始最大20M,清完不够double40M,不及时清，容易大
    /// </summary>
    ~MapLst()
    { 
        Clear();
    }

    /// <summary>
    /// 清空
    /// </summary>
    public void Clear()
    {
        if (lst.Tail != null)
        {
            Remove(lst.Tail.SelfNode);//自动填充Tail，所以循环删除Tail就好
        }
    }
    #endregion

}
#endregion


#region Async
/// <summary>
/// 加载资源优先级（异步）
/// </summary>
public enum AsyncLoadResPriority
{
    High,
    Middle,
    Low, 
    Count

}
/// <summary>
///资源加载参数类
/// </summary>
public class AsyncLoadResPara
{
   public List<AsyncTotalCallBack> m_CbLst=new List<AsyncTotalCallBack>();
    public uint m_Crc;
    public string m_Path;
    public AsyncLoadResPriority m_Priority = AsyncLoadResPriority.Low;
    /// <summary>是不是Sprite，Sprite不能强Object来赋值 AssetBundleRequest.asset.所以标记来特殊处理</summary>
    public bool m_Sprite=false;

    public void Reset()
    {
        m_Crc = 0;
        m_Path = "";
        m_Priority = AsyncLoadResPriority.Low;
        m_Sprite = false;
        m_CbLst.Clear();
    }

}

/// <summary>
/// ResourcesMgr异步加载回调类
/// </summary>
public class AsyncTotalCallBack
{
    public OnAsyncObject m_AsyncObject =null;
    public OnAsyncResObj m_AsyncResObj =null;
    public ResObj m_ResObj = null;
    public object m_Para1 = null;
    public object m_Para2 = null;
    public object m_Para3 = null;

    public void Reset()
    {
         m_AsyncObject = null;
        m_AsyncResObj = null;
        m_ResObj = null;
        m_Para1 = null;
        m_Para2 = null;
        m_Para3 = null;

    }
}
#endregion


#region ResObj

/// <summary>
///ResItem=>ResObj（过度类） =>GameObject <para />
/// public uint m_Crc = 0 <para />
/// public bool m_JmpClr = true <para />
/// public GameObject m_Go = null//m_ResItem.m_Obj实例出来的 <para />
/// public ResItem m_ResItem = null <para />
///  public bool m_Released=false; <para />
/// public int m_GUID = 0
/// </summary>
public class ResObj
{
    public uint m_Crc = 0;
    /// <summary>进入回收池后，跳场景时清除</summary>
    public bool m_JmpClr = true;

    //
    /// <summary>实例出的Go</summary>
    public GameObject m_Go = null;//m_ResItem.m_Obj实例出来的
    public ResItem m_ResItem=null;
    public int m_GUID=0;//后面Ocean几乎没用到，用m_Go.GetInstanceID()
    public bool m_Released=false;//防止多次Release
    /// <summary>设置到场景中的某个节点下</summary>
    public bool m_SetSceneParent=false;
    /// <summary>实例完的回调</summary>
    public OnAsyncObject m_CBInstaniate = null;
    //回传的三个参数，个数自己定义
    public object m_Para1 =null;
    public object m_Para2 =null;
    public object m_Para3 =null;
    /// <summary>离线数据，加载时用到</summary>
    public OfflineData m_OfflineData = null;

    public void Reset()
    {
        m_Crc = 0;
        m_Go = null;
        m_JmpClr = true;
        m_GUID = 0;
        m_ResItem = null;
        m_Released = false;
        m_SetSceneParent = false;
        m_CBInstaniate = null;
        m_Para1 = null;
        m_Para2 = null;
        m_Para3 = null;

    }
}
#endregion



