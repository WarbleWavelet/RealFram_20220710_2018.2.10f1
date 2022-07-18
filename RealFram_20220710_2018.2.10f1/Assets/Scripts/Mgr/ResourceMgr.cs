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




/// <summary>回调</summary>
public delegate void OnAsyncLoadResFinished(string path, Object obj, object para1, object para2, object para3);

public class ResourceMgr : Singleton<ResourceMgr>
{

    #region 字段 属性
    /// <summary>在内存中的资源,没被引用（常用但目前没引用）</summary>
    protected MapLst<ResItem> m_NoRefResItemLst = new MapLst<ResItem>();

    /// <summary>在内存中的资源，被引用</summary>
    public Dictionary<uint, ResItem> m_RefResItemDic { get; set; } = new Dictionary<uint, ResItem>();


    bool m_loadFromAB = false;

    #region Async
    /// <summary>某个MonoBehaviour（它要开协程）</summary>
    MonoBehaviour m_startMono;
    /// <summary>异步加载Res的列表</summary>
    List<AsyncLoadResPara>[] m_asyncLoadingResParaLst = new List<AsyncLoadResPara>[(int)AsyncLoadResPriority.Count];
    /// <summary>正在异步加载的资源</summary>
    Dictionary<uint, AsyncLoadResPara> m_asyncLoadingResParaDic = new Dictionary<uint, AsyncLoadResPara>();

    /// <summary>异步对象池</summary>
    public ClassObjectPool<AsyncLoadResPara> m_AsyncLoadResParaPool = new ClassObjectPool<AsyncLoadResPara>(Constants.ClassObjectPool_AsyncLoadResPara_MAXCNT);

    public ClassObjectPool<AsyncLoadResCallBack> m_AsyncLoadResCallBackPool = new ClassObjectPool<AsyncLoadResCallBack>(Constants.ClassObjectPool_AsyncLoadResCallBack_MAXCNT);
    #endregion


    #endregion


    #region 异步
    /// <summary>
    /// 开始协程的条件，传入自身this
    /// </summary>
    /// <param name="mono"></param>
    public void InitCoroutine(MonoBehaviour mono)
    {

        for (int i = 0; i < (int)AsyncLoadResPriority.Count; i++)
        {
            m_asyncLoadingResParaLst[i] = new List<AsyncLoadResPara>();
        }
        m_startMono = mono;
        m_startMono.StartCoroutine(AsyncLoadResource());
    }


    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <returns></returns>
    IEnumerator AsyncLoadResource()
    {
        List<AsyncLoadResCallBack> cbLst = new List<AsyncLoadResCallBack>();
        long lastYieldTime = System.DateTime.Now.Ticks;
        while (true)
        {
            bool haveYield = false;//内层yield后外层不用yield
            //不同优先级的回调列表，High Middle Low
            for (int i = 0; i < (int)AsyncLoadResPriority.Count; i++)
            {
                List<AsyncLoadResPara> paraLst = m_asyncLoadingResParaLst[i];
                if (paraLst.Count <= 0)
                {
                    continue;
                }

                AsyncLoadResPara para = paraLst[0];//要加载的Item
                paraLst.RemoveAt(0);
                cbLst = para.m_CbLst;

                //obj => resItem
                Object obj = null;
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
                    if (resItem == null)
                    {
                        resItem = new ResItem();
                        resItem.m_Crc = para.m_Crc;
                    }
                }

#endif
                if (null == obj)
                {
                    resItem = AssetBundleMgr.Instance.LoadResItem(resItem.m_Crc);

                    if (null != resItem && null != resItem.m_AB)
                    {
                        AssetBundleRequest abReq = resItem.m_AB.LoadAssetAsync(resItem.m_AssetName);

                        yield return abReq;

                        if (abReq.isDone)
                        {
                            if (para.m_Sprite == true)// 特殊：Sprite 会不能直接等于 asset
                            {
                                obj = resItem.m_AB.LoadAsset<Sprite>(resItem.m_ABName);
                            }
                            else
                            {
                                obj = abReq.asset;
                            }

                        }
                        lastYieldTime = System.DateTime.Now.Ticks;

                    }
                }
                //缓存资源
                CacheResItem(para.m_Path, ref resItem, resItem.m_Crc, obj, cbLst.Count);

                // AsyncLoadResCallBack
                for (int j = 0; j < cbLst.Count; j++)
                {
                    AsyncLoadResCallBack cb = cbLst[j];

                    if (cb != null & cb.m_FinishedCb != null)
                    {
                        cb.m_FinishedCb(para.m_Path, obj, cb.m_Para1, cb.m_Para2, cb.m_Para3);
                        cb.m_FinishedCb = null;
                    }
                    cb.Reset();
                    m_AsyncLoadResCallBackPool.Recycle(cb);
                }

                //AsyncLoadResCallPara
                obj = null;
                cbLst.Clear();
                m_asyncLoadingResParaDic.Remove(para.m_Crc);

                //
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




    public void AsyncLoadResource(string path,
        OnAsyncLoadResFinished finishedCb,
        AsyncLoadResPriority priority,
        object para1 = null,
        object para2 = null,
        object para3 = null,
        uint crc = 0)
    {
        if (crc == 0)
        {
            crc = CRC32.GetCRC32(path);

        }

        ResItem resItem = GetCacheResItem(crc);

        if (resItem != null)
        {
            if (finishedCb != null)
            {
                finishedCb(path, resItem.m_Obj, para1, para2, para3);
            }
            return;
        }

        AsyncLoadResPara para = null;
        if (m_asyncLoadingResParaDic.TryGetValue(crc, out para) == false || para == null)
        {
            para = m_AsyncLoadResParaPool.Spawn(true);
            para.m_Crc = crc;
            para.m_Path = path;
            para.m_Priority = priority;
            //
            m_asyncLoadingResParaLst[(int)priority].Add(para);
            m_asyncLoadingResParaDic.Add(crc, para);

        }

        //回调
        AsyncLoadResCallBack cb = m_AsyncLoadResCallBackPool.Spawn(true);
        cb.m_FinishedCb = finishedCb;
        cb.m_Para1 = para1;
        cb.m_Para2 = para2;
        cb.m_Para3 = para3;
        para.m_CbLst.Add(cb);

    }
    #endregion


    #region T

    /// <summary>
    /// 基础资源同步加载(不用实例化，Texture等)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public T LoadResource<T>(string path) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }
        //同步加载
        uint crc = CRC32.GetCRC32(path);
        ResItem resItem = GetCacheResItem(crc);

        //
        if (resItem != null)
        {
            return resItem as T;
        }


        T obj = null;
#if UNITY_EDITOR//测试从Editor加载            

        if (m_loadFromAB == false)
        {

            resItem = AssetBundleMgr.Instance.GetResItem(crc);
            if (resItem.m_Obj != null)
            {
                obj = resItem.m_Obj as T;
            }
            else
            {
                obj = LoadAssetByEditor<T>(path);
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


#if UNITY_EDITOR
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


    #region ResItem
    /// <summary>
    /// 异步、同步、预加载ResItem，获取
    /// </summary>
    /// <param name="crc"></param>
    /// <param name="addCnt"></param>
    /// <returns></returns>
    ResItem GetCacheResItem(uint crc, int addCnt = 1)
    {
        ResItem resItem = null;
        if (m_RefResItemDic.TryGetValue(crc, out resItem))
        {
            if (resItem != null)
            {
                resItem.m_RefCnt += addCnt;
                resItem.m_LastUseTime = Time.realtimeSinceStartup;

                //安全判断，理论进不来
                if (resItem.m_RefCnt <= 1)
                {
                    m_NoRefResItemLst.Remove(resItem);
                    // noRefLst.AddToHead(resItem);
                }
            }
        }

        return resItem;
    }

    /// <summary>
    /// 缓存
    /// </summary>
    /// <param name="name"></param>
    /// <param name="resItem"></param>
    /// <param name="crc"></param>
    /// <param name="obj"></param>
    /// <param name="addRefCnt"></param>

    void CacheResItem(string name, ref ResItem resItem, uint crc, UnityEngine.Object obj, int addRefCnt = 1)
    {
        /// Washout();
        if (resItem == null)
        {
            Debug.LogErrorFormat("Err");

        }

        if (obj == null)
        {
            Debug.LogErrorFormat("Err");
        }
        resItem = new ResItem
        {
            m_Obj = obj,//占内存
            m_AssetName = name,
            m_Crc = crc,
            m_LastUseTime = Time.realtimeSinceStartup,
            m_GUID = obj.GetInstanceID()
        };
        resItem.m_RefCnt += addRefCnt;

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
        float destroyPercent = 0.8f;

        if (m_NoRefResItemLst.Count() <= 0)
        {
            return;
        }

        ResItem resItem = m_NoRefResItemLst.GetTail();
        DestroyResItem(resItem, true);
        m_NoRefResItemLst.Pop();

    }

    /// <summary>
    /// 回收资源(是否有其他Object在引用)
    /// </summary>
    /// <param name="resItem"></param>
    /// <param name="destroyCache">是否要删除缓存</param>
    protected void DestroyResItem(ResItem resItem, bool destroyCache = false)
    {

        if (resItem == null || resItem.m_RefCnt > 0)
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
            AssetBundleMgr.Instance.ReleaseResItem(resItem);
        ObjectMgr.Instance.ClearPoolObject(resItem.m_Crc);
            //下层引用，对象置空
            if (resItem.m_Obj != null)
            {
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
    public bool ReleaseResItem(UnityEngine.Object obj, bool destroyCache = false)
    {
        if (obj != null)
        {
            return false;
        }

        ResItem resItem = null;
        foreach (ResItem _resItem in m_RefResItemDic.Values)//耗性能
        {
            if (_resItem.m_GUID == obj.GetInstanceID())
            {
                resItem = _resItem;
            }
        }

        if (resItem == null)
        {
            Debug.LogErrorFormat("该资源不存在缓存中");
            return false;
        }

        resItem.m_RefCnt--;
        DestroyResItem(resItem, destroyCache);

        return true;

    }

    /// <summary>
    /// 不需要实例化的资源的卸载<para />
    /// TryGetValue<para />
    /// </summary>
    /// <param name="path"></param>
    /// <param name="destroyCache"></param>
    /// <returns></returns>
    public bool ReleaseResItem(string path, bool destroyCache = false)
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


        resItem.m_RefCnt--;
        DestroyResItem(resItem, destroyCache);

        return true;

    }



    #endregion


    #region ResObj
    /// <summary>
    /// 同步加载资源，给ObjectMgr用的
    /// </summary>
    /// <param name="path"></param>
    /// <param name="resObj"></param>
    /// <returns></returns>
    public ResObj LoadResObj(string path, ResObj resObj)
    {
        if (resObj == null)
        {
            return null;
        }

        uint crc= (resObj.m_Crc==0) ? CRC32.GetCRC32(path) : resObj.m_Crc;

        ResItem resItem = GetCacheResItem(crc);


        //Get得到就Get
        if (resItem != null)
        {
            resObj.m_ResItem = resItem;
            return resObj;
        }

        //Get不到就到下一层Get
        Object obj = null;
#if UNITY_EDITOR//测试从Editor加载            

        if (m_loadFromAB == false)
        {

            resItem = AssetBundleMgr.Instance.GetResItem(crc);
            obj = LoadAssetByEditor<Object>(path);
            
        }
#endif
        //Get不到就Load
        if (obj == null)
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
        }
        //缓存
        CacheResItem(path, ref resItem, crc, obj);
        resObj.m_ResItem = resItem;
        resItem.m_JmpClr = resObj.m_JmpClr;


        return resObj;
    }

    public bool ReleaseResObject(ResObj resObj, bool destroyCache)
    {
        if (resObj == null) 
        {
            return false;
        }

        //  crc => resItem
        uint crc = resObj.m_Crc;
        ResItem resItem = null;

        if (m_RefResItemDic.TryGetValue(crc, out resItem) == false || resItem == null)
        {
            Debug.LogErrorFormat("资源不存在，或被多次清空");
        }

        GameObject.Destroy(resObj.m_Go); ;
        resItem.m_RefCnt--;
        DestroyResItem(resItem, destroyCache);

        return true;
    }


    #region RefCnt
  public int AddResItemRefCnt( uint crc, int cnt=1  )
    { 
        ResItem resItem= null;
        if ( m_RefResItemDic.TryGetValue( crc, out resItem) ==false || resItem==null)
        {
            return 0;
        }

        resItem.m_RefCnt += cnt;
        resItem.m_LastUseTime = DateTime.Now.Ticks;

        return resItem.m_RefCnt;
    }

    public int AddResItemRefCnt(ResObj resObj, int cnt = 1)
    {
        if (resObj == null)
        {
            return 0;
        }
        return AddResItemRefCnt( resObj.m_Crc, cnt);
    }

    public int SubResItemRefCnt(uint crc, int cnt = 1)
    {
        ResItem resItem = null;
        if (m_RefResItemDic.TryGetValue(crc, out resItem) == false || resItem == null)
        {
            return 0;
        }

        resItem.m_RefCnt -= cnt;

        return resItem.m_RefCnt;
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








    /// <summary>
    /// 比如跳转场景时，清内存
    /// </summary>
    public void ClearCache()
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
            if (  true==resItem.m_JmpClr)
            { 
                 lst.Add(resItem);
            }
        }

        foreach (ResItem resItem in lst)
        {
            DestroyResItem(resItem, true);
        }
    }

    /// <summary>
    /// 比如跳转场景时
    /// </summary>
    public void PreLoadRes(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        //同步加载
        uint crc = CRC32.GetCRC32(path);
        ResItem resItem = GetCacheResItem(crc,0);//预加载不需要引用计数
        if (resItem != null)
        {
            return;
        }


       Object obj = null;
#if UNITY_EDITOR//测试从Editor加载            

        if (m_loadFromAB == false)
        {

            resItem = AssetBundleMgr.Instance.GetResItem(crc);
            if (resItem.m_Obj != null)
            {
                obj = resItem.m_Obj;
            }
            else
            {
                obj = LoadAssetByEditor<Object>(path);
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
        ReleaseResItem(path, false);
    }
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

    public ClassObjectPool<DoubleLinkedListNode<T>> DLLNPool = ObjectMgr.Instance.TryGetClassObjectPool<DoubleLinkedListNode<T>>(Constants.ClassObjectPool_MAXCNT);

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
        if (DLLNDic.TryGetValue(node, out _node) && _node != null)
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
    /// 析构函数，自动清除
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
/// 加载资源参数
/// </summary>
public class AsyncLoadResPara
{
   public List<AsyncLoadResCallBack> m_CbLst=new List<AsyncLoadResCallBack>();
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

public class AsyncLoadResCallBack
{
    public OnAsyncLoadResFinished m_FinishedCb =null;
    public object m_Para1 = null;
    public object m_Para2 = null;
    public object m_Para3 = null;

    public void Reset()
    {
         m_FinishedCb = null;
        m_Para1 = null;
        m_Para2 = null;
        m_Para3 = null;

    }
}
#endregion




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
    /// <summary>跳场景清除</summary>
    public bool m_JmpClr = true;

    //
    /// <summary>实例出的Go</summary>
    public GameObject m_Go = null;//m_ResItem.m_Obj实例出来的
    public ResItem m_ResItem=null;
    public int m_GUID=0;
    public bool m_Released=false;//防止多次Release


    public void Reset()
    {
        m_Crc = 0;
        m_Go = null;
        m_JmpClr = true;
        m_GUID = 0;
        m_ResItem = null;
        m_Released = false;
    }
}