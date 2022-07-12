using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetDataBase_Read : MonoBehaviour
{



    void Start()
    {
        GameObject go = ReadAssetDatabase(DefinePath.path_ADB);
        go.transform.position = Vector3.right * Constants.offset;

    }

    GameObject ReadAssetDatabase(string path)
    {
        GameObject prefab =new GameObject(path);
            //= AssetDatabase.LoadAssetAtPath<GameObject>(path);
        GameObject go = Instantiate(prefab);//可以不加后缀

        return go;
    }
}
