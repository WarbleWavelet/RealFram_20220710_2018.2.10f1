using UnityEngine;

public class Constants
{
    public const float Demo01_Offset = 10f;
    public const string MenuItem = "MyTools";
    public const string MenuItem_Offline = "MyTools/离线数据/";
    public const string MenuItem_FormatTool = "数据转换/";
    public const int MenuItem_FormatTool_StartIdx = 0;
    public const string Assets = "My Assets/";
    public const string Shader_BengHuai = "Custom/benghuai";





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
  

    //ResourceMgr
    public const int MaxCacheCnt = 500;

    #endregion

    public const string AppName = "RealFrame";

}
public class Constants_Demo13
{

    private const string PanelPath = "Assets/GameData/Prefabs/UGUI/Panel/Demo13/";
    public const string MenuPanel = PanelPath + "Demo13MenuPanel.prefab";
}

public class Constants_Demo14
{
    public const string Scene_Menu = "Menu14";
    public const string Scene_Start = "Start14";
    public const string Scene_Empty = "Empty14";
    //
    private const string PanelPath = "Assets/GameData/Prefabs/UGUI/Panel/Demo14/";
    public const string Prefab_LoadPanel = PanelPath + "Demo14LoadPanel.prefab";
    public const string Prefab_MenuPanel = PanelPath + "Demo14MenuPanel.prefab";
    public const string Prefab_Attack = "Assets/GameData/Prefabs/Attack.prefab";
    public const string MP3_SenLin = "Assets/GameData/Sounds/senlin.mp3";
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