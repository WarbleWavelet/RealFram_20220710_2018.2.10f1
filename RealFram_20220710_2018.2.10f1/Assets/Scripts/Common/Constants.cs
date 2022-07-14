public class Constants
{
    public const float offset = 10f;
    public const string MenuItem = "MyTools";
    public const string MenuAsset = "MyTools";
   
    //public const string AssetBundleConfig = "AssetBundleConfig"; // 系统自动转小写
    public const string AssetBundleConfig = "assetbundleconfig";




    #region Mgr
    /// <summary>类对象池默认最大 Pool_MaxCnt </summary>
    public const int ClassObjectPool_MAXCNT =500;
    public const int ClassObjectPool_AsyncLoadResPara_MAXCNT = 50;
    public const int ClassObjectPool_AsyncLoadResCallBack_MAXCNT = 100;

    /// <summary>卡着异步加载资源的最长时间</summary>
    public const int MAXASYNCLOADRESTIME = 200000;
    #endregion

}