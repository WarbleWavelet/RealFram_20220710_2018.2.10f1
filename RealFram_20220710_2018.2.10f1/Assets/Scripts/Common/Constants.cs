﻿public class Constants
{
    public const float Demo01_Offset = 10f;
    public const string MenuItem = "MyTools";
    public const string MenuAsset = "MyAssets";
   




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
public class Constants_Demo13
{

    public const string PanelPath = "Assets/GameData/Prefabs/UGUI/Panel/Demo13/";
    public const string MenuPanel = PanelPath + "MenuPanel.prefab";
}

public class Constants_Demo14
{
    public const string Scene_Menu = "Menu14";
    public const string Scene_Start = "Start14";
    public const string Scene_Empty = "Empty14";
    public const string PanelPath = "Assets/GameData/Prefabs/UGUI/Panel/Demo14/";
    public const string LoadPanel = PanelPath + "LoadPanel.prefab";
    public const string MenuPanel = PanelPath + "MenuPanel.prefab";


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