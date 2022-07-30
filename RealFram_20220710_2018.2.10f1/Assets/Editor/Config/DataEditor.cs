/****************************************************
    文件：DataEditor.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/28 22:57:57
	功能：
*****************************************************/

using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class DataEditor
{
    const string m_Xml_InnerPath = "Assets/GameData/Data/Xml/";
    static string m_Xml_OutetrPath = DefinePath.ProjectRoot + "/Data/Reg/";
    static string m_Excel_OutetrPath = DefinePath.ProjectRoot + "/Data/Excel/";
    const string m_path_Bin = "Assets/GameData/Data/Bin/";
    const string m_path_Scripts = "Assets/Scripts/Data/";


    #region MenuItem

    [MenuItem("Assets/My Assets/Class2Xml", false, 0)]//按钮在菜单栏的位置
    static void Class2Xml()
    {

        UnityEngine.Object[] objArr = Selection.objects;

        for (int i = 0; i < objArr.Length; i++)
        {
            UnityEngine.Object obj = objArr[i];
            string title = "正在转成Xml";
            string info = "";
            info += "正在转化" + obj.name + "....";
            float prg = (1.0f * i) / objArr.Length; ;
            EditorUtility.DisplayCancelableProgressBar(title, info, prg);
            Name2Class2Xml(obj.name);
        }


        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();

    }


    [MenuItem("Assets/My Assets/Xml2Bin", false, 1)]//按钮在菜单栏的位置
    static void Xml2Bin()
    {

        UnityEngine.Object[] objArr = Selection.objects;

        for (int i = 0; i < objArr.Length; i++)
        {
            UnityEngine.Object obj = objArr[i];
            string title = "正在转成Bin";
            string info = "";
            info += "正在转化" + obj.name + "....";
            float prg = (1.0f * i) / objArr.Length; ;
            EditorUtility.DisplayCancelableProgressBar(title, info, prg);
            Name2Xml2Bin(obj.name);
        }


        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();

    }

    [MenuItem("Assets/My Assets/AllXml2Bin", false, 2)]//按钮在菜单栏的位置
    static void AllXml2Bin()
    {



        string xmlPath = DefinePath.ProjectRoot + m_Xml_InnerPath;
        string[] pathArr = Directory.GetFiles(xmlPath, "*.*", SearchOption.AllDirectories);


        for (int i = 0; i < pathArr.Length; i++)
        {
            string path = pathArr[i];
            string title = "正在添加离线数据";
            string info = "";
            info += "正在修改" + path + "....";
            float prg = (1.0f * i) / pathArr.Length; ;
            EditorUtility.DisplayCancelableProgressBar(title, info, prg);
            AllPath2Xml2Bin(path);
        }

        EditorUtility.ClearProgressBar();



    }



    [MenuItem("测试/Xml/Test_ReadXml", false, 3)]//按钮在菜单栏的位置
    static void Test_ReadXml() //读取工程下xmlPath的xml
    {

        #region MonsterData.xml
        /**
        <? xml version = "1.0" encoding = "utf-8" ?>
        < data name = "MonsterData" from = "G怪物.xlsx" to = "MonsterData.xml" >
            < variable  name = "AllMonster" type = "list" >
                < list name = "MonsterBase" sheetname = "怪物配置" mainKey = "Id" >
                    < variable  name = "Id" col = "ID" type = "int" />
                    < variable  name = "Name" col = "名字" type = "string" />
                    < variable  name = "OutLook" col = "预知路径" type = "string" />
                    < variable  name = "Level" col = "怪物等级" type = "int" />
                    < variable  name = "Rare" col = "怪物稀有度" type = "int" defaultValue = "0" />
                    < variable  name = "Height" col = "怪物高度" type = "float" />
                </ list >
            </ variable >
        </ data >
       **/
        #endregion
        string xmlPath = m_Xml_OutetrPath + "MonsterData.xml";
        XmlReader reader = null;
        try
        {
            XmlDocument xml = new XmlDocument();
            reader = XmlReader.Create(xmlPath);
            xml.Load(reader);
            XmlNode n1 = xml.SelectSingleNode("data");
            XmlElement e1 = (XmlElement)n1; //01 data
            string className = e1.GetAttribute("name");
            string xmlName = e1.GetAttribute("to");
            string excelName = e1.GetAttribute("from");
            reader.Close();
            Debug.LogFormat("className:{0}\txmlName:{1}\texcelName:{2}", className, xmlName, excelName);
            //
            foreach (XmlNode n2 in e1.ChildNodes)
            {
                XmlElement e2 = (XmlElement)n2;//02 variable 
                string name2 = e2.GetAttribute("name");
                string type2 = e2.GetAttribute("type");
                Debug.LogFormat("name2:{0}\type2:{1}", name2, type2);
                //
                XmlNode n3 = e2.FirstChild;
                XmlElement e3 = (XmlElement)n3;//03 list
                string listName = e3.GetAttribute("name");
                string sheetName = e3.GetAttribute("sheetname");
                string mainKey = e3.GetAttribute("mainKey");
                Debug.LogFormat("listName:{0}\tsheetName:{1}\tmainKey:{2}", listName, sheetName, mainKey);
                foreach (XmlNode n4 in e3.ChildNodes)
                {
                    XmlElement e4 = (XmlElement)n4; //04 variable
                    string name4 = e4.GetAttribute("name");
                    string col = e4.GetAttribute("col");
                    string type4 = e4.GetAttribute("type");
                    Debug.LogFormat("name4:{0}\tcol:{1}\ttype4:{2}", name4, col, type4);
                }
            }
        }
        catch (Exception e)
        {
            if (reader != null)
            {
                reader.Close();
            }
            Debug.LogError(e);
        }


    }



    [MenuItem("测试/Excel/Test_WriteExcel", false, 4)]//按钮在菜单栏的位置
    static void Test_WriteExcel() //读取工程下xmlPath的xml
    {
        string xlsxPath = m_Excel_OutetrPath + "G怪物.xlsx";

        FileInfo fi = new FileInfo(xlsxPath);
        if (fi.Exists == true)
        {
            fi.Delete();
            fi = new FileInfo(xlsxPath);
        }

        using (ExcelPackage pack = new ExcelPackage(fi))
        {
            ExcelWorksheet sheet = pack.Workbook.Worksheets.Add("怪物配置");
            WriteCell(sheet.Cells[1, 1], "测试");
            WriteCellAndAdapt(sheet.Cells[2, 1], " Siki学院Ocean老师!!!!!!");
            WriteCellAndAdapt(sheet.Cells[3, 1], " Siki学院Ocean老师!!!!!! \n dhwsjkfhjklsdcgjadcghkasn");


            pack.Save();//必加
            //RW 的增删查改
            // ExcelWorksheet sheet = pack.Workbook.Worksheets.Add("怪物配置");  
            //sheet.InsertColumn();//插入行，从某一行开始插入多少行
            //sheet.InsertRow();//插入列，从某一列开始插入多少列
            //sheet.DeleteColumn();//删除行，从某一行开始删除多少行
            //sheet.DeleteRow();//删除列，从某一列开始删除多少列
            //sheet.Column(1).Width = 10;//设定第几行宽度
            //sheet.Row(1).Height = 30;//设定第几列高度
            //sheet.Column(1).Hidden = true;//设定第几行隐藏
            //sheet.Row(1).Hidden = true;//设定第几列隐藏
            //sheet.Column(1).Style.Locked = true;//设定第几行锁定
            //sheet.Row(1).Style.Locked = true;//设定第几列锁定
            //
            //sheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;//设定所有单元格对齐方式
            // sheet.Cells.AutoFitColumns();
            //
            //range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.None; //填充颜色
            //range.Style.Fill.BackgroundColor.SetColor();//设置单元格内背景颜色
            //range.Style.Font.Color.SetColor();//设置单元格内字体颜色
            //range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;//对齐方式 


        }
    }



    [MenuItem("测试/Reflection/Test_Reflection", false, 4)]//按钮在菜单栏的位置
    static void Test_Reflection() //读取工程下xmlPath的xml
    {
        TestReflection testReflection = new TestReflection
        {
            m_ID = 0,
            m_Name = "千山鸟费劲",
            m_Female = false

        };

       int ID = (int)GetClassMember(testReflection, "m_ID", GetBindingFlags() );
       string name = (string)GetClassMember(testReflection, "m_Name", GetBindingFlags());
       bool female = (bool)GetClassMember(testReflection, "m_Female", GetBindingFlags());

        Debug.LogFormat("测试反射：\tID：{0}\tname：{1}\tfemale：{2}", ID, name, female);


    }




    #region 辅助

    /// <summary>
    /// 类反射得到字段属性值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <param name="propertyName"></param>
    /// <param name="bindingFlags"></param>
    /// <returns></returns>
    static object GetClassMember<T>(T t, string propertyName, BindingFlags bindingFlags)
    {

        Type type = t.GetType();
        MemberInfo[] miArr = type.GetMember(propertyName, BindingFlags.Public | BindingFlags.Instance);

        while (miArr == null || miArr.Length <= 0) //保护性措施
        {
            type = type.BaseType;
            if (type == null)
            {
                return null;
            }
            miArr = type.GetMember(propertyName, bindingFlags);
        }

        object res = null;
        switch (miArr[0].MemberType)
        {
            case MemberTypes.Field:
                {
                    res = type.GetField(propertyName, bindingFlags).GetValue(t);
                }
                break;
            case MemberTypes.Property:
                {
                    res = type.GetProperty(propertyName, bindingFlags).GetValue(t);
                }
                break;
            default: break;
        }

        return res;
    }



    /// <summary>
    /// 写入
    /// </summary>
    /// <param name="range"></param>
    /// <param name="value"></param>
    static void WriteCell(ExcelRange range, string value)
    {
        range.Value = value;

    }

    /// <summary>
    /// 写入and自适应
    /// </summary>
    /// <param name="range"></param>
    /// <param name="value"></param>
    private static void WriteCellAndAdapt(ExcelRange range, string value)
    {
        range.Value = value;
        AutoAdapt(range);
    }

    /// <summary>
    ///表格的一种自适应
    /// </summary>
    /// <param name="pack"></param>
    static void AutoAdapt(ExcelRange range)
    {
        range.AutoFitColumns();
        range.Style.WrapText = true;
    }

    /// <summary>
    /// 设置表格的宽高
    /// </summary>
    /// <param name="pack"></param>
    static void Default_RC_WH(ExcelPackage pack)
    {
        ExcelWorksheet sheet = pack.Workbook.Worksheets.Add("怪物配置");
        sheet.DefaultColWidth = 10;//sheet页面默认行宽度
        sheet.DefaultRowHeight = 30;//sheet页面默认列高度
        sheet.Cells.Style.WrapText = true;//设置所有单元格的自动换行
        pack.Save();
    }


    private static void AllPath2Xml2Bin(string path)
    {
        if (path.EndsWith(".xml") == true)
        {
            string _path = Common.TrimName(path, TrimNameType.SlashAfter);
            string name = _path.Replace(".xml", "");


            Name2Xml2Bin(name);


        }
    }


    private static void Name2Xml2Bin(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        try
        {
            Type type = GetClassByClassName(name);

            if (type != null)
            {
                string xmlPath = m_Xml_InnerPath + name + ".xml";
                string binPath = m_path_Bin + name + ".bytes";
                object obj = FormatTool.Xml2Class(xmlPath, type);
                FormatTool.Class2Bin(binPath, obj);
                Debug.Log(name + "xml转二进制成功，二进制路径为:" + binPath);
            }
        }
        catch
        {
            Debug.LogError(name + "xml转二进制失败！");
        }
    }

    /// <summary>
    /// 数量1，防止点错了，污染旧数据
    /// </summary>
    /// <param name="name"></param>
    static void Name2Class2Xml(string name)
    {
        Type type = GetClassByClassName(name);

        if (type != null)
        {
            var temp = Activator.CreateInstance(type);
            if (temp is ExcelBase)
            {
                (temp as ExcelBase).Construction();
            }
            string xmlPath = m_Xml_InnerPath + name + ".xml";
            FormatTool.Xml2Class(xmlPath, temp);
            Debug.Log(name + "类转xml成功，xml路径为:" + xmlPath);
        }
    }

    /// <summary>
    /// 根据类名达到类
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    static Type GetClassByClassName(string name)
    {
        Type type = null;
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = asm.GetType(name);
            if (type != null)
            {
                break;
            }
        }

        return type;
    }

    /// <summary>
    /// 进行反射的Flags
    /// </summary>
    /// <returns></returns>
    static BindingFlags GetBindingFlags()
    {
        return BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
    }
    #endregion

  
    #endregion

}


public class TestReflection
{
    //public int m_ID;
    //public string m_Name;
    //public bool m_Female;       
    //或
    public int m_ID { get; set; }
    public string m_Name { get; set; }
    public bool m_Female { get; set; }
}