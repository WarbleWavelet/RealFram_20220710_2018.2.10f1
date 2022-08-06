/****************************************************
    文件：BufData.cs
	作者：lenovo
    邮箱: 
    日期：2022/8/4 16:19:49
	功能：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


[System.Serializable]
public class BuffData : ExcelBase {

    #region 字属
    [XmlIgnore]
    public Dictionary<int, BuffBase> AllBuffDic = new Dictionary<int, BuffBase>();
    [XmlElement("AllBuffList")]  public List<BuffBase> AllBuffList { get; set; }
    [XmlElement("MonsterBuffList")]  public List<BuffBase> MonsterBuffList { get; set; }
    #endregion




    #region 方法
#if UNITY_EDITOR
    public override void Construction()
    {
        AllBuffList= SetAllBuffList(AllBuffList); //属性不能ref
        MonsterBuffList=SetMonsterBuffList(MonsterBuffList);

    }




#endif

    #region 辅助
    private List<BuffBase> SetMonsterBuffList(List<BuffBase> MonsterBuffList)
    {
        MonsterBuffList = new List<BuffBase>();
        for (int i = 0; i < 5; i++)
        {
            BuffBase buff = new BuffBase();
            buff.Id = i + 1;
            buff.Name = "全BUFF" + i;
            buff.OutLook = "Assets/GameData/..." + i;
            buff.Time = Random.Range(0.5F, 10);
            buff.BuffType = (BuffEnum)Random.Range(0, 4);
            //
            buff.AllString = new List<string>();
            buff.AllString.Add("ceshi" + i);
            //
            buff.AllBuffList = GetSubAllBuffList();
            //
            MonsterBuffList.Add(buff);
        }

        return MonsterBuffList;
    }

    List<BuffBase> SetAllBuffList(List<BuffBase> AllBuffList)
    {
        AllBuffList = new List<BuffBase>();
        for (int i = 0; i < 10; i++)
        {
            BuffBase buff = new BuffBase();

            buff.Id = i + 1;
            buff.Name = "全BUFF" + i;
            buff.OutLook = "Assets/GameData/..." + i;
            buff.BuffType = (BuffEnum)Random.Range(0, 4);
            //
            buff.AllString = new List<string>();
            buff.AllString.Add("ceshi" + i);
            buff.AllString.Add("ceshiq" + i);
            //
            buff.AllBuffList = GetSubAllBuffList();
            //
            AllBuffList.Add(buff);
        }

        return AllBuffList;
    }

    List<BuffTest> GetSubAllBuffList()
    {
        List<BuffTest> AllBuffList = new List<BuffTest>();
        int count = Random.Range(0, 4);
        for (int j = 0; j < count; j++)
        {
            BuffTest test = new BuffTest();
            test.Id = j + Random.Range(0, 5);
            test.Name = "name" + j;
            AllBuffList.Add(test);
        }

        return AllBuffList;
    }
    #endregion

    public override void Init()
    {
        AllBuffDic.Clear();
        for (int i = 0; i < AllBuffList.Count; i++)
        {
            if (AllBuffDic.ContainsKey(AllBuffList[i].Id) == true)
            {
                Debug.LogError("已经包含");
            }
            AllBuffDic.Add(AllBuffList[i].Id, AllBuffList[i]);
        }
    }

    /// <summary>
    /// 根据ID查找buff
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public BuffBase FinBuffById(int id)
    {
        return AllBuffDic[id];
    }
    #endregion
}


[System.Serializable]
public class BuffBase
{

    [XmlAttribute("Id")] public int Id { get; set; }                    //ID
    [XmlAttribute("Name")] public string Name { get; set; }             //Name
    [XmlAttribute("OutLook")] public string OutLook { get; set; }       //预知路径
    [XmlAttribute("Time")] public float Time { get; set; }              //怪物等级
    [XmlAttribute("BuffType")]  public BuffEnum BuffType { get; set; }
    [XmlElement("AllString")] public List<string> AllString { get; set; }
    [XmlElement("AllBuffList")] public List<BuffTest> AllBuffList { get; set; }
}


public enum BuffEnum
{
    None = 0,
    Ranshao = 1,
    Bingdong = 2,
    Du = 3,
}


[System.Serializable]
public class BuffTest
{
    [XmlAttribute("Id")]  public int Id { get; set; }
    [XmlAttribute("Name")]  public string Name { get; set; }
}
