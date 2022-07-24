using System.Collections.Generic;
using UnityEngine;

public class Common
{





    /// <summary>异步的Guid，为了可以取消该异步</summary> 
    static long  m_asyncGuid=0;
    /// <summary>异步的Guid，为了可以取消该异步</summary>
    public static long CreateGuid()
    {
        return m_asyncGuid++;
    }

    public static void Log( object obj)
    {
        UnityEngine.Debug.Log(obj.GetType().ToString() + "." + new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().ToString());//类名.方法名

    }


    public static string TrimName(string path, TrimNameType type)
    {


        switch (type)
        {
            case TrimNameType.Slash:
                {
                    return path.Substring(path.LastIndexOf('/') + 1);//sdcvghasvdj/gdhsag/a.prefab => a.prefab
                }
                break;
            case TrimNameType.SlashAndPoint:
                {
                    string name = path.Substring(path.LastIndexOf('/') + 1);// plane.unity3d
                    name = name.Substring(0, name.LastIndexOf('.'));// plane
                    return name;
                }
                break;
            default:
                {
                    return path;
                } break;
        }


    }

   public static void FixShader(GameObject go)
    {
        MeshRenderer[] mr = go.GetComponentsInChildren<MeshRenderer>();
        List<Material> lst = new List<Material>();
        for (int i = 0; i < mr.Length; i++)
        {
            lst.AddRange(mr[i].materials);
        }
        SkinnedMeshRenderer[] smr = go.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < smr.Length; i++)
        {
            lst.AddRange(smr[i].materials);
        }

        for (int i = 0; i < lst.Count; i++)
        {
            lst[i].shader = Shader.Find("Custom/benghuai");
        }
    }
}


public enum TrimNameType
{
    None,
    /// <summary>A/B/C.prefab => C.prefab</summary>
    Slash,
    /// <summary>A/B/C.prefab => C</summary>
    SlashAndPoint,


}