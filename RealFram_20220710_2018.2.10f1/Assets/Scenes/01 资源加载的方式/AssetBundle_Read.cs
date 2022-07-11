
using UnityEngine;

public class AssetBundle_Read : MonoBehaviour 
{

    void Start()
    {
        GameObject go= ReadAssetBundle(DefinePath.path_Stream);
        go.transform.position = Vector3.left * Constants.offset;
    }

    GameObject ReadAssetBundle(string path)
    {
        AssetBundle ab = AssetBundle.LoadFromFile(path);//必须加后缀
        string _name =Common.TrimName(path );
        GameObject prefab = ab.LoadAsset<GameObject>(_name);//可以不加后缀
        GameObject go = Instantiate(  prefab);

        return go;
    }
   
}

public class DefinePath
{
    public static string path_Stream = Application.streamingAssetsPath + "/attack.unity3d";
    public static string path_ADB =  "Assets/GameData/Prefabs/Attack.prefab";
    public static string path_Xml = Application.dataPath + "/Scenes/02/test.xml";
}

public class Common
{


    public static string TrimName(string path)
    {
        string _name = path.Substring(path.LastIndexOf('/') + 1);// plane.unity3d
        _name = _name.Substring(0, _name.LastIndexOf('.'));// plane

        return _name;
    }
}


public class Constants
{
    public const float offset = 10f;
}




