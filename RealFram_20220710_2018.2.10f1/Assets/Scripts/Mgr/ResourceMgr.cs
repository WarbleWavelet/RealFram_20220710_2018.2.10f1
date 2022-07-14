/****************************************************
    文件：ResourceMgr.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/13 2:22:47
	功能：资源管理器
       1：以双向链表为基础的资源池（基于使用率）
       2：基础资源同步加载
       3：基本资源卸载
       4：基础资源异步加载
       5：清空缓存
       6：预加载
       6：为ObjectManager提供的同步异步资源加载

path=>Res
path=>crc
Cache
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object=UnityEngine.Object;//经常判null检测

public class ResourceMgr : Singleton<ResourceMgr>
{
    /// <summary>在内存中的资源,没被引用（常用但目前没引用）</summary>
    protected MapLst<ResItem> noRefLst=new MapLst<ResItem>();

    /// <summary>在内存中的资源，被引用</summary>
    public Dictionary<uint, ResItem> RefDic { get; set; } = new Dictionary<uint, ResItem>();


     bool loadFromAB = false;

    /// <summary>
    /// 基础资源同步加载(不用实例化，Texture等)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    #region T
    public T LoadResource<T>(string path) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(path))
        { 
            return null;
        }

        uint crc = CRC32.GetCRC32(path);
        ResItem resItem = GetResItem(crc);
        if (resItem != null)
        {
            return resItem as T;
        }


        T obj = null;
#if UNITY_EDITOR//测试从Editor加载            

        if (loadFromAB == false)
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
            if (resItem != null && resItem.m_AssetBundle != null)
            {
                if (resItem.m_Obj != null)
                {
                    obj = resItem.m_Obj as T;
                }
                else
                { 
                     obj = resItem.m_AssetBundle.LoadAsset<T>( resItem.m_AssetBundleName);
                }
            }
        }

        CacheResItem(path,ref resItem, crc, obj);

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
    ResItem GetResItem(uint crc,int addCnt=1)
    { 
        ResItem resItem=null;
        if (RefDic.TryGetValue(crc, out resItem))
        {
            if (resItem != null)
            {
                resItem.RefCnt += addCnt;
                resItem.m_LastUseTime = Time.realtimeSinceStartup;

                //安全判断，理论进不来
                if (resItem.RefCnt <= 1)
                { 
                    noRefLst.Remove(resItem);
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

    void CacheResItem(string name, ref ResItem resItem, uint crc, UnityEngine.Object obj, int addRefCnt=1)
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
            m_GUID=obj.GetInstanceID()
        };
        resItem.RefCnt += addRefCnt;

        //
        ResItem oldResItem = null;
        if (RefDic.TryGetValue(crc, out oldResItem))//Update
        {
            RefDic[crc] = resItem;
        }
        else//add
        {
            RefDic.Add(crc, resItem);
        }
    }

    /// <summary>
    /// 缓存太多，清理一些
    /// </summary>
    void Washout()
    {
        float destroyPercent = 0.8f;

        if (noRefLst.Count() <= 0)
        {
            return;
        }

        ResItem resItem = noRefLst.GetTail();
        DestroyResItem(resItem,true);
        noRefLst.Pop();

    }

    /// <summary>
    /// 回收资源(是否有其他Object在引用)
    /// </summary>
    /// <param name="resItem"></param>
    /// <param name="destroyCache">是否要删除缓存</param>
    protected void DestroyResItem(ResItem resItem, bool destroyCache = false)
    {
        
        if (resItem == null || resItem.RefCnt > 0)
        { 
            return ;
        }

        //对Ref
        if (RefDic.Remove(resItem.m_Crc) == false)
        {
            return;
        }

        //对NoRef
        if (destroyCache == false)
        {
            noRefLst.AddToHead(resItem);
           // return;
        }
        else
        {
            #region 最占内存的两个地方
            //上层Mgr
            AssetBundleMgr.Instance.ReleaseResItem(resItem);

            //下层引用，对象置空
            if (resItem.m_Obj != null)
            {
                resItem.m_Obj = null;
            }
            #endregion
        }




    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="destroyCache"></param>
    /// <returns></returns>
  public  bool ReleaseResItem(UnityEngine.Object obj, bool destroyCache=false)
    {
        if (obj != null)
        {
            return false;
        }

        ResItem resItem=null;
        foreach (ResItem  _resItem in RefDic.Values)
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

        resItem.RefCnt--;
        DestroyResItem( resItem, destroyCache);

        return true;

    }
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

    public ClassObjectPool<DoubleLinkedListNode<T>> DLLNPool = ObjectMgr.Instance.TryGetClassObjectPool<DoubleLinkedListNode<T>>(Constants.ClassobjectPool_MAXCNT);

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