public class Common
{


    public static string TrimName(string path)
    {
        string _name = path.Substring(path.LastIndexOf('/') + 1);// plane.unity3d
        _name = _name.Substring(0, _name.LastIndexOf('.'));// plane

        return _name;
    }


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

}