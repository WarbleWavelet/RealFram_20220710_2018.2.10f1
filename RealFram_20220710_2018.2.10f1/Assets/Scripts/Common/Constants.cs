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
    public const int ClassObjectPool_RESOBJ_MAXCNT = 1000;

    /// <summary>卡着异步加载资源的最长时间</summary>
    public const int MAXASYNCLOADRESTIME = 200000;

    public const string FixPre="";
    public const string FixSur_ResObject_m_Go="(Recycle)";
    public const string FixSur_InstaniateGameObject="(Clone)";
    #endregion

}




public   class  Constans_OfflineData
{ 
        
       public const string m_Type = "t:prefab";
       public const string m_Path = "Assets/GameData/OfflineData/UIOfflineData";


}public   class  Constans_UIOfflineData
{ 
        
       public const string m_Type = "t:prefab";
       public const string m_Path = "Assets/GameData/Prefabs/UGUI/Panel";
}


public   class  Constans_ParticleOfflineData
{ 
        
       public const string m_Type = "t:prefab";
       public const string m_Path = "Assets/GameData/OfflineData/ParticleOfflineData";
}