
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










