/****************************************************
    文件：Singleton.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/13 2:13:9
	功能：单例类
*****************************************************/

public class Singleton<T> where T : new()
{

    #region 单例
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }
    #endregion


}