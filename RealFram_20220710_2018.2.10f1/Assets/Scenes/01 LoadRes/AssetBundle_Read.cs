
using UnityEngine;


namespace Demo01
{
    public class AssetBundle_Read : MonoBehaviour 
    {

        void Start()
        {
            Object obj= LoadObjectFromAB(DefinePath.Demo01_Bytes_Attack);
            GameObject go = GameObject.Instantiate( obj ) as GameObject;
            go.transform.position = Vector3.left * Constants.Demo01_Offset;

            Common.FixShader( go );
        }

        Object LoadObjectFromAB(string path)
        {
            AssetBundle ab = AssetBundle.LoadFromFile(path);//有后缀就必须加后缀
            string name = Common.TrimName(path, TrimNameType.Slash);
            Object obj = ab.LoadAsset<Object>(name);//可以不加后缀
            return obj;
        }

    }

}










