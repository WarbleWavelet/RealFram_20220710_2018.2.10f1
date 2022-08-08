/****************************************************
    文件：TestScriptableObject.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/11 14:55:37
	功能：读取ScriptableScripts  SO
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Demo04
{
    public class ReadSO : MonoBehaviour
    {
        public Button btn1;
        // Use this for initialization
        void Start()
        {
           btn1.onClick.AddListener(Read)  ;
        }

        void Read()
        {

#if UNITY_EDITOR
              AssetsSerilize assets = AssetDatabase.LoadAssetAtPath<AssetsSerilize>(DefinePath.Demo04_Asset);
            Debug.Log(assets.Id);
            Debug.Log(assets.Name);
            foreach (string str in assets.TestList)
            {
                Debug.Log(str);
            }
#endif


        }
    }

}
