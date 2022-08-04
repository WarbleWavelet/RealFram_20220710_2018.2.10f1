/****************************************************
    文件：MonsterData.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/28 22:41:49
	功能： 不和父类写一起。转xml时会污染
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[System.Serializable]
public class MonsterData : ExcelBase
{

    #region 字属
    /// <summary>怪物百科</summary>
    [XmlIgnore] //不序列化
    public Dictionary<int, MonsterBase> m_MonsterDic = new Dictionary<int, MonsterBase>();

    /// <summary>所有的怪物</summary>                 
    [XmlElement("AllMonster")] //与xml相关，不改
    public List<MonsterBase> m_MonsterLst { get; set; }  //反序列时填充，new会叠加
    //public List<MonsterBase> m_MonsterLst { get; set; }   //反序列时填充，new会叠加
    #endregion




    #region 方法
#if UNITY_EDITOR
    /// <summary>
    /// 编辑器下初始类转xml
    /// </summary>
    public override void Construction()
    {
        m_MonsterLst = new List<MonsterBase>();
        for (int i = 0; i < 5; i++)
        {
            MonsterBase monster = new MonsterBase
            {
                Id = i + 1,
                Name = "Monster_" + i ,
                OutLook = "Assets/GameData/Prefabs/Attack.prefab",
                Rare = 2,
                Height = 2 + i,
            };
            m_MonsterLst.Add(monster);
        }
    }
#endif

    /// <summary>
    /// 数据初始化
    /// </summary>
    public override void Init()
    {
        m_MonsterDic.Clear();
        foreach (MonsterBase monster in m_MonsterLst) //记录百科
        {
            if (m_MonsterDic.ContainsKey(monster.Id))
            {
                Debug.LogError(monster.Name + " 有重复ID");
            }
            else
            {
                m_MonsterDic.Add(monster.Id, monster);
            }
        }
    }

    /// <summary>
    /// 根据ID查找Monster数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public MonsterBase GetMonsterById(int id)
    {
        return m_MonsterDic[id];
    }
    #endregion
}



[System.Serializable]
public class MonsterBase
{
    
    [XmlAttribute("Id")] public int Id { get; set; }                    //ID
    [XmlAttribute("Name")] public string Name { get; set; }             //Name
    [XmlAttribute("OutLook")] public string OutLook { get; set; }       //预知路径
    [XmlAttribute("Level")] public int Level { get; set; }              //怪物等级
    [XmlAttribute("Rare")] public int Rare { get; set; }                //怪物稀有度
    [XmlAttribute("Height")]  public float Height { get; set; }         //怪物高度
}
