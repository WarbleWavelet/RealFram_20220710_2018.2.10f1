/****************************************************
    文件：TestYield.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/24 16:37:11
	功能：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestYield : MonoBehaviour 
{

    void Start()
    {
        StartCoroutine(A());//在MonoBehaviour中使用

    }



    IEnumerator A()
    {
        AsyncOperation unLoadScene = SceneManager.LoadSceneAsync(Constants_Demo14.Scene_Menu);

        while (unLoadScene != null && unLoadScene.isDone == false)
        {
            Debug.Log("unLoadScene.progress" + unLoadScene.progress);
            yield return new WaitForEndOfFrame();
        }
    }
}
