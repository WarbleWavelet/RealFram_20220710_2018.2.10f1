public class Common
{


    public static string TrimName(string path)
    {
        string _name = path.Substring(path.LastIndexOf('/') + 1);// plane.unity3d
        _name = _name.Substring(0, _name.LastIndexOf('.'));// plane

        return _name;
    }
}