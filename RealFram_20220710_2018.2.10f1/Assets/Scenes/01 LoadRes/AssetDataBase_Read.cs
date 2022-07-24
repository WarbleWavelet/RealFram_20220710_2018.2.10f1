using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Demo01
{ 
    public class AssetDataBase_Read : MonoBehaviour
    {
        void Start()
        {
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(DefinePath.Demo01_Attack_Prefab);
            GameObject go = GameObject.Instantiate(obj) as GameObject;
            go.transform.position = Vector3.right * Constants.Demo01_Offset;

        }

    }

}
