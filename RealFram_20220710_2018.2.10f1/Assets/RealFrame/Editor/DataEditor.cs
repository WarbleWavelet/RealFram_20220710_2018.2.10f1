/****************************************************
    文件：DataEditor.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/28 22:57:57
	功能：Bin Xml Class Excel Protobuf各种转来转去
*****************************************************/

using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class DataEditor
{
    const string m_Xml_InnerPath = "Assets/RealFrame/GameData/Data/Xml/";//xml数据
    static string m_Xml_OutetrPath = DefinePath.ProjectPath + "Data/Reg/"; //xml结构 reg
    static string m_Excel_OutetrPath = DefinePath.ProjectPath + "Data/Excel/";//excel数据
    //
    const string m_Bin_InnerPath = "Assets/RealFrame/GameData/Data/Bin/";
    const string m_Protobuf_InnerPath = "Assets/RealFrame/GameData/Data/Protobuf/";
    const int m_startIdx = DefinePath.MenuItem_FormatTool_StartIdx;//调顺序，但因为二级难调

    #region MenuItem



    #region Class
    [MenuItem(DefinePath.MenuItem_FormatTool + "Class/Class2Xml", false, 1 + m_startIdx)]//按钮在菜单栏的位置
    static void MenuItem_Class2Xml()
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
            Class2Xml(obj.name);
        }


        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();

    }
    #endregion




    #region Xml


    [MenuItem(DefinePath.MenuItem_FormatTool + "Xml/Xml2Bin", false, 1 + m_startIdx)]//按钮在菜单栏的位置
    static void MenuItem_Xml2Bin()
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

  
    
    [MenuItem(DefinePath.MenuItem_FormatTool + "Xml/Xml2BinAll", false, 1 + m_startIdx)]//按钮在菜单栏的位置
   public static void MenuItem_Xml2BinAll() ///xml转Bin
    {



        string xmlPath = DefinePath.ProjectPath + m_Xml_InnerPath;
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


     [MenuItem(DefinePath.MenuItem_FormatTool + "Xml/Xml2Excel", false, 1 + m_startIdx)]//按钮在菜单栏的位置
    static void MenuItem_Xml2Excel() //读取工程下xmlPath的xml
    {

        UnityEngine.Object[] objArr = Selection.objects;

        for (int i = 0; i < objArr.Length; i++)
        {
            UnityEngine.Object obj = objArr[i];
            string title = "正在转成Excel";
            string info = "";
            info += "正在转化" + obj.name + "....";
            float prg = (1.0f * i) / objArr.Length; ;
            EditorUtility.DisplayCancelableProgressBar(title, info, prg);
            Xml2Excel(obj.name);
        }


        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }





    [MenuItem(DefinePath.MenuItem_FormatTool + "Xml/Test_ReadXml", false, 1+ m_startIdx)]//按钮在菜单栏的位置
    static void MenuItem_Test_ReadXml() //读取工程下xmlPath的xml
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
    #endregion





    #region Excel


    [MenuItem(DefinePath.MenuItem_FormatTool + "Excel/Excel2Xml", false, 5 + m_startIdx)]//按钮在菜单栏的位置
    static void MenuItem_Excel2Xml() //读取工程下xmlPath的xml
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
            Excel2Xml(obj.name);
        }

        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem(DefinePath.MenuItem_FormatTool + "Excel/Excel2XmlAll", false, 5 + m_startIdx)]//按钮在菜单栏的位置
    static void MenuItem_Excel2XmlAll() //读取工程下xmlPath的xml
    {
        bool findOnce = false;
        string name = "";
        string[] filePathArr = Directory.GetFiles(m_Xml_OutetrPath, "*", SearchOption.TopDirectoryOnly);//我需要保存一些以前用的在目录，所以top
        for (int i = 0; i < filePathArr.Length; i++)
        {


            if (filePathArr[i].EndsWith(".xml") == false)
            {
                if (i >= filePathArr.Length - 1 && findOnce==false)
                {
                    Debug.LogErrorFormat("没有找到Reg，路径：{0}", m_Xml_OutetrPath);
                }
                continue;
            }
            EditorUtility.DisplayProgressBar("查找文件夹下的Excel", "正在扫描路径" + filePathArr[i] + "... ...", 1.0f / filePathArr.Length * i);
            name=  Common.TrimName(filePathArr[i], TrimNameType.SlashAndPoint);
            Excel2Xml( name );
            findOnce = true;

        }

        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem(DefinePath.MenuItem_FormatTool + "Excel/Excel2BinAll", false, 5 + m_startIdx)]//按钮在菜单栏的位置
    static void MenuItem_Excel2BinAll() //读取工程下xmlPath的xml
    {
        bool findOnce = false;
        string name = "";
        string[] filePathArr = Directory.GetFiles(m_Xml_OutetrPath, "*", SearchOption.TopDirectoryOnly);//我需要保存一些以前用的在目录，所以top
        for (int i = 0; i < filePathArr.Length; i++)
        {


            if (filePathArr[i].EndsWith(".xml") == false)
            {
                if (i >= filePathArr.Length - 1 && findOnce == false)
                {
                    Debug.LogErrorFormat("没有找到Reg，路径：{0}", m_Xml_OutetrPath);
                }
                continue;
            }
            EditorUtility.DisplayProgressBar("查找文件夹下的Excel", "正在扫描路径" + filePathArr[i] + "... ...", 1.0f / filePathArr.Length * i);
            name = Common.TrimName(filePathArr[i], TrimNameType.SlashAndPoint);
            bool toBin = true;
            Excel2Xml(name,toBin);
            findOnce = true;

        }

        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem(DefinePath.MenuItem_FormatTool + "Excel/Test_WriteExcel", false, 5 + m_startIdx)]//按钮在菜单栏的位置
    static void MenuItem_Test_WriteExcel() //读取工程下xmlPath的xml
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


    #endregion




    #region Reflection


    [MenuItem(DefinePath.MenuItem_FormatTool + "Reflection/Test_根据反射读取类的属性值", false, 99 + m_startIdx)]//按钮在菜单栏的位置
    static void MenuItem_Test_Reflection() //读取工程下xmlPath的xml
    {
        TestReflection testRef = new TestReflection
        {
            m_ID = 0,
            m_Name = "千山鸟费劲",
            m_Female = false,
            m_Lst = new List<string>(),
            m_SubLst = new List<TestReflectionSub>()
        };
        testRef.m_Lst.Add("刘备");
        testRef.m_Lst.Add("关羽");
        testRef.m_Lst.Add("张飞");
        testRef.m_SubLst.Add(new TestReflectionSub() { m_Name = "刘禅" });
        testRef.m_SubLst.Add(new TestReflectionSub() { m_Name = "关平" });
        testRef.m_SubLst.Add(new TestReflectionSub() { m_Name = "张苞" });
        //                                                                                //
        int ID = (int)Ref_Class_Member_Get(testRef, "m_ID", GetBindingFlags());
        string name = (string)Ref_Class_Member_Get(testRef, "m_Name", GetBindingFlags());
        bool female = (bool)Ref_Class_Member_Get(testRef, "m_Female", GetBindingFlags());
        Debug.LogFormat("测试反射：\tID：{0}\tname：{1}\tfemale：{2}", ID, name, female);

        List<object> lst = Ref_Class_List_Get(testRef, "m_Lst", -1);
        List<object> subLst = Ref_Class_List_Get(testRef, "m_SubLst", -1);
        //

        foreach (var item in lst) //列表存字符串
        {
            Debug.Log(item);
        }
        foreach (var item in subLst) //列表存类
        {
            string subName = Ref_Class_Member_Get(item, "m_Name", GetBindingFlags()) as string;
            Debug.Log(subName);
        }
    }


    [MenuItem(DefinePath.MenuItem_FormatTool + "Reflection/Test_数据反射成类", false, 99 + m_startIdx)]//按钮在菜单栏的位置
    static void MenuItem_Test_ReflectionByData() //读取工程下xmlPath的xml
    {
        object obj = Ref_Class_New("TestReflection01");
        Ref_Class_Member_SetValue(obj, "m_Name", "刘备");
        Ref_Class_Member_SetValue(obj, "m_Female", false);
        //SetClassMemberValue(testRef, "m_Female", System.Convert.ToBoolean( "false"));
        Ref_Class_Member_SetValue(obj, "m_ID", 0);
        // SetClassMemberValue(testRef, "m_ID", System.Convert.ToInt32( "0" ));

        TestReflection01 testRef01 = obj as TestReflection01;

        Debug.LogFormat("测试反射：\tID：{0}\tname：{1}\tfemale：{2}", testRef01.m_ID, testRef01.m_Name, testRef01.m_Female);

    }


    [MenuItem(DefinePath.MenuItem_FormatTool + "Reflection/Test_数据反射成类(浮点，枚举)", false, 99 + m_startIdx)]//按钮在菜单栏的位置
    static void MenuItem_Test_ReflectionByData_Float_Enum() //读取工程下xmlPath的xml
    {
        object obj = Ref_Class_New("TestReflection02");
        Ref_Class_Member_SetValue(obj, "m_Name", "刘备", "string");
        Ref_Class_Member_SetValue(obj, "m_Female", "false", "bool");
        Ref_Class_Member_SetValue(obj, "m_ID", "0", "int");
        Ref_Class_Member_SetValue(obj, "m_Height", "180.1", "float"); //浮点
        Ref_Class_Member_SetValue(obj, "m_Rank", "One", "enum"); //枚举


        TestReflection02 testRef02 = obj as TestReflection02;

        Debug.LogFormat("测试反射：\tID：{0}\tname：{1}\tfemale：{2}\t height:{3}\t rank:{4}", testRef02.m_ID, testRef02.m_Name, testRef02.m_Female, testRef02.m_Height, testRef02.m_Rank);

    }

    [MenuItem(DefinePath.MenuItem_FormatTool + "Reflection/Test_数据反射成类(列表)", false, 99 + m_startIdx)]//按钮在菜单栏的位置
    static void MenuItem_Test_ReflectionByData_Lst() //读取工程下xmlPath的xml
    {
        object _classObj = Ref_Class_New("TestReflection02");
        Ref_Class_Member_SetValue(_classObj, "m_Name", "刘备", "string");
        Ref_Class_Member_SetValue(_classObj, "m_Female", "false", "bool");
        Ref_Class_Member_SetValue(_classObj, "m_ID", "0", "int");
        Ref_Class_Member_SetValue(_classObj, "m_Height", "180.1", "float"); //浮点
        Ref_Class_Member_SetValue(_classObj, "m_Rank", "One", "enum"); //枚举


        TestReflection02 _class = _classObj as TestReflection02;

        Debug.LogFormat("测试反射：\tID：{0}\tname：{1}\tfemale：{2}\t height:{3}\t rank:{4}", _class.m_ID, _class.m_Name, _class.m_Female, _class.m_Height, _class.m_Rank);
        //

        object strLst = Ref_List_New(typeof(string)); //处理列表
        object classLst = Ref_List_New(typeof(TestReflectionSub));
        for (int i = 0; i < 3; i++)
        {
            object strItem = i + "_刘备";
            Ref_List_Add(strLst, strItem);
            //
            object classItem = Ref_Class_New("TestReflectionSub");
            Ref_Class_Member_SetValue(classItem, "m_Name", i + "_刘禅", "string");
            Ref_List_Add(classLst, classItem);
        }
        _class.GetType().GetProperty("m_Lst").SetValue(_class, strLst);
        _class.GetType().GetProperty("m_SubLst").SetValue(_class, classLst);



        foreach (var item in _class.m_Lst) //打印列表
        {
            Debug.Log(item);
        }
        foreach (var item in _class.m_SubLst) //打印列表
        {
            Debug.Log(item.m_Name);
        }

    }
    #endregion


    #region Protobuf


    [MenuItem(DefinePath.MenuItem_FormatTool + "Protobuf/Xml2Protobuf", false, 6 + m_startIdx)]//按钮在菜单栏的位置
    static void MenuItem_Xml2Protobuf()
    {

        UnityEngine.Object[] objArr = Selection.objects;

        for (int i = 0; i < objArr.Length; i++)
        {
            UnityEngine.Object obj = objArr[i];
            string title = "正在转成Protobuf";
            string info = "";
            info += "正在转化" + obj.name + "....";
            float prg = (1.0f * i) / objArr.Length; ;
            EditorUtility.DisplayCancelableProgressBar(title, info, prg);
            Xml2Protobuf(obj.name);
        }


        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }



    [MenuItem(DefinePath.MenuItem_FormatTool + "Protobuf/Protobuf2Xml", false, 6 + m_startIdx)]//按钮在菜单栏的位置
    static void MenuItem_Protobuf2Xml()
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
            Protobuf2Xml(obj.name);
        }


        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    [MenuItem(DefinePath.MenuItem_FormatTool + "Protobuf/Test_Protobuf2Class（MonsterData）", false, 6 + m_startIdx)]//按钮在菜单栏的位置
    static void MenuItem_Test_Protobuf2Class()
    {
        string protobufPath = m_Protobuf_InnerPath +"MonsterData.bytes";
        MonsterData md = FormatTool.Protobuf2Class<MonsterData>(protobufPath);
        foreach (MonsterBase  mb in md.m_MonsterLst)
        {
            Debug.LogFormat("MonsterBase\tName:{0}\tOutLook:{1}", mb.Name,mb.OutLook  );
        }

        foreach (KingBase mb in md.m_KingLst)
        {
            Debug.LogFormat("KingBase\tName:{0}\tOutLook:{1}", mb.Name, mb.OutLook);
        }
    }


        #endregion


        #endregion









        #region 辅助
        static void Excel2Xml(string regName, bool toBin=false)
    {
        string className = "";
        string xmlName = "";
        string excelName = "";
        Dictionary<string, Lst> lstDic = ReadReg(regName, ref excelName, ref xmlName, ref className);      //第一步，读取Reg文件，确定类的结构


        string excelPath = m_Excel_OutetrPath + excelName;//第二步，读取excel里面的数据
        if (File.Exists(excelPath) == false)
        {
            Debug.LogErrorFormat("没有表{0},路径：{1}",excelName, m_Excel_OutetrPath);
            return;
        }


        Dictionary<string, Sheet> sheetDic = new Dictionary<string, Sheet>();
        try  //读取Excel
        {
            using (FileStream stream = new FileStream(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) //支持打开
            {
                using (ExcelPackage pack = new ExcelPackage(stream))
                {
                    ExcelWorksheets excelWorksheetArr = pack.Workbook.Worksheets;
                    for (int sheetIdx = 0; sheetIdx < excelWorksheetArr.Count; sheetIdx++)//工作表
                    {
                        Sheet sheet = new Sheet();
                        ExcelWorksheet worksheet = excelWorksheetArr[sheetIdx + 1]; //Excel从1开始
                        Lst lst = lstDic[worksheet.Name];
                        int colCnt = worksheet.Dimension.End.Column;//列  //Dimension尺寸
                        int rowCnt = worksheet.Dimension.End.Row;     //行

                        for (int colIdx = 0; colIdx < lst.m_VarList.Count; colIdx++) //每个表的列信息
                        {
                            sheet.m_NameLst.Add(lst.m_VarList[colIdx].m_Name);  //列的变量名（列名，列的变量名，列的类型）
                            sheet.m_TypeLst.Add(lst.m_VarList[colIdx].m_Type);  //列类型
                        }

                        for (int rowIdx = 1; rowIdx < rowCnt; rowIdx++)
                        {
                            Row row = new Row();
                            int colIdx = 0;


                            if ( IsSubSheet(lst)==true) //列表要拆分
                            {

                               string IDVal=  worksheet.Cells[rowIdx + 1, 1].Value.ToString().Trim();   
                                row.m_ParentVal = IDVal;  //外键
                                colIdx = 1;
                            }
                            for (; colIdx < colCnt; colIdx++)
                            {
                       
                                ExcelRange range = worksheet.Cells[rowIdx + 1, colIdx + 1];
                                string rangeVal = ""; 
                                if (range.Value != null) //表格不填数据，用默认值
                                {
                                    rangeVal = range.Value.ToString().Trim();  //单元格数据
                                }

                                
                                string colName =   worksheet.Cells[1, colIdx + 1].Value.ToString().Trim(); //第一行的都是列名//正则会去除/r/n
                                string varName = GetVarNameFromColName(lst.m_VarList, colName);//变量名

                                row.m_RowDataDic.Add( varName , rangeVal);  //Id,1之类
                            }

                            sheet.m_RowLst.Add(row); //行数据
                        }
                        if (String.IsNullOrEmpty(worksheet.Name))
                        {

                            Debug.LogErrorFormat("变量名为空：{0}", 11);
                        }
                        sheetDic.Add(worksheet.Name, sheet); //表
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("Excel2Xml{0}",e);
            return;
        }

        


        object _object =  NewSetObjectByClassName(  className, lstDic,sheetDic);
        //
        FormatTool.Class2Xml(m_Xml_InnerPath + xmlName, _object);
        if (toBin == false)
        {
            Debug.LogFormat("{0}转{1}成功", excelName, xmlName);
        }
        else
        {
            FormatTool.Class2Bin(m_Bin_InnerPath+className+".bytes" ,_object );
            Debug.LogFormat("{0}转{1}成功", excelName, className + ".bytes");
        }


    
        AssetDatabase.Refresh();
    }


    /// <summary>
    /// 在Excel中是被拆分的子表
    /// </summary>
    /// <param name="lst"></param>
    /// <returns></returns>
        static bool IsSubSheet(Lst lst)
        {
            return string.IsNullOrEmpty( lst.m_SplitStr)==true
            && lst.m_ParentVar != null
            && string.IsNullOrEmpty(lst.m_ParentVar.m_Foreign) == false;
        }


    /// <summary>
    ///   根据类的结构，创建类，并且给每个变量赋值（从excel里读出来的值）
    /// </summary>
    /// <param name="className"></param>
    /// <param name="xmlPath"></param>
    /// <param name="xmlName"></param>
    /// <param name="lstDic"></param>
    /// <param name="sheetDic"></param>
    /// <param name=""></param>
    static object NewSetObjectByClassName(string className,
        Dictionary<string, Lst> lstDic,
        Dictionary<string, Sheet> sheetDic
         )
    {
        object _object = Ref_Class_New(className);

        List<string> outterKeyLst = new List<string>();
        foreach (string str in lstDic.Keys)//最外层var的Name
        {
            Lst lst = lstDic[str];
            if (lst.m_Depth == 1)
            {
                outterKeyLst.Add(str);
            }
        }

        for (int i = 0; i < outterKeyLst.Count; i++)
        {
            string outterKey = outterKeyLst[i];
            ReadDataToClass( _object, lstDic[outterKey], sheetDic[outterKey], lstDic, sheetDic, null);//递归
        }


        return _object;

    }


    /// <summary>
    /// Xml与Excel的中间存储类（需要Reg(Xml结构文件)）
    /// 以MonsterData为例写注释
    /// </summary>
    /// <param name="_object"></param>
    /// <param name="lst"></param>
    /// <param name="sheet"></param>
    /// <param name="lstDic"></param>
    /// <param name="sheetDic"></param>
    /// <param name="foreign">外键，子表的外键</param>
    private static void ReadDataToClass(object _object, Lst lst, Sheet sheet, 
        Dictionary<string, Lst> lstDic, 
        Dictionary<string, Sheet> sheetDic,
        object foreign)
    {
        object item = Ref_Class_New( lst.m_Name ); //为了得到变量类型
        object m_lst = Ref_List_New( item.GetType() );
       
        for (int rowIdx = 0; rowIdx < sheet.m_RowLst.Count; rowIdx++) //创建 m_MonsterLst
        {
            Row row = sheet.m_RowLst[rowIdx];
            if (foreign != null 
                && String.IsNullOrEmpty(row.m_ParentVal)==false) 
            {
                if (foreign.ToString() != row.m_ParentVal) //没有外键，不用拆分，继续赋值
                {
                    continue;
                }
            }

            object addItem = Ref_Class_New(lst.m_Name); //MonsterData

            for (int colIdx = 0; colIdx < lst.m_VarList.Count; colIdx++) //m_MonsterLst之类
            {
                Var _var = lst.m_VarList[colIdx];//var节点

                //Debug.LogFormat("列名：{0}",_var.m_Col);
                if (_var == null || _var.m_Type == null || _var.m_Type == "")//读不到数据时
                {
                    Debug.LogErrorFormat("_var:{0},_var.m_Type:{1}", _var, _var.m_Type);
                }
                if (_var.m_Type == "list" 
                    && String.IsNullOrEmpty(_var.m_SplitStr) == true)// 不分割类列表
                {
                    ReadDataToClass(addItem, 
                        lstDic[_var.m_SheetName_SelfList], 
                        sheetDic[_var.m_SheetName_SelfList],
                        lstDic, sheetDic,
                        Ref_Class_Member_Get(addItem, lst.m_MainKey));
                }
                else if (_var.m_Type == "list")//分割类列表
                {
                    string rangeVal = GetRange( sheet, rowIdx, colIdx);
                    SetClassLst( addItem,
                        lstDic[_var.m_SheetName_SelfList], 
                        rangeVal);
                }
                else if (IsBaseList(_var.m_Type) == true)//base列表
                {
                    string rangeVal = GetRange(sheet, rowIdx, colIdx);
                    SetBaseLst(addItem, _var, rangeVal);
                }
                else//值写入
                {
                    string rangeVal = GetRange( sheet, rowIdx, colIdx);
                    if (String.IsNullOrEmpty(rangeVal) == true && _var.m_DeafultValue != null)  //对单元格空白的处理
                    {
                        rangeVal = _var.m_DeafultValue;
                    }
                    if (String.IsNullOrEmpty(rangeVal) == true)
                    {
                        Debug.LogErrorFormat("表格单元格数据为空，Reg未配置默认数据:{0}", rangeVal);
                        continue;
                    }
                    Ref_Class_Member_SetValue(addItem, sheet.m_NameLst[colIdx], rangeVal, sheet.m_TypeLst[colIdx]);
                }
            }
             Ref_List_Add(m_lst, addItem);
        }
       Ref_Class_Member_SetValue(_object,lst.m_ParentVar.m_Name, m_lst);
    }




    static string GetRange(Sheet sheet, int rowIdx, int colIdx)
    {
        Row row = sheet.m_RowLst[rowIdx]; //行
        Dictionary<string, string> rowData = row.m_RowDataDic;//行的数据
        string colName = sheet.m_NameLst[colIdx]; //列名
        string rangeVal = rowData[colName];//根据列名取数据 

        return rangeVal;
    }
    /// <summary>
    /// 列名得到变量名
    /// </summary>
    /// <param name="colName"></param>
    /// <returns></returns>
    static string GetVarNameFromColName(List<Var> varLst, string colName)
    {
        foreach (var item in varLst)
        {
            if (item.m_Col == colName)
            {
                return item.m_Name;
            }
        }

        return null;
    }

    static void Xml2Excel(string name) //读取工程下xmlPath的xml
    {

        string className = "";
        string xmlName = "";
        string excelName = "";
       string regName = name;//一般同名,不一样就是为了测试别的
        //
        Dictionary<string, global::Lst> lstDic = ReadReg(regName, ref excelName, ref xmlName, ref className); //读取结构
        object _object = ClassName2Object(className);



        List<global::Lst> lstLst = new List<global::Lst>();
        foreach (global::Lst lst in lstDic.Values)
        {
            if (lst.m_Depth == 1) //第一层lst
            {
                lstLst.Add(lst);
            }
        }

        Dictionary<string, global::Sheet> sheetDic = new Dictionary<string, global::Sheet>();
        for (int i = 0; i < lstLst.Count; i++)    //Lst节点
        {
            ReadData(_object, lstLst[i], lstDic, sheetDic, "", 60);
        }

        string xlsxPath = m_Excel_OutetrPath + excelName; //被占用，自带了后缀
        if (FileIsOpened(xlsxPath))
        {
            Debug.LogError("文件被占用，无法修改");
            return;
        }

        try
        {
            FileInfo fi = new FileInfo(xlsxPath); //存则删，新建
            if (fi.Exists == true)
            {
                fi.Delete();
                fi = new FileInfo(xlsxPath);

            }

            //
            using (ExcelPackage pack = new ExcelPackage(fi))
            {
                foreach (string str in sheetDic.Keys)
                {
                    ExcelWorksheet worksheet = pack.Workbook.Worksheets.Add(str);
                    global::Sheet sheet = sheetDic[str];
                    for (int i = 0; i < sheet.m_NameLst.Count; i++)//字段数据，标头
                    {
                        ExcelRange range = worksheet.Cells[1, i + 1];//1开始
                        range.Value = sheet.m_NameLst[i];
                        range.AutoFitColumns();
                    }

                    for (int i = 0; i < sheet.m_RowLst.Count; i++) //每一行数据
                    {
                        global::Row row = sheet.m_RowLst[i];
                        for (int j = 0; j < sheet.m_RowLst[i].m_RowDataDic.Count; j++)
                        {
                            ExcelRange range = worksheet.Cells[i + 2, j + 1];
                            string val = row.m_RowDataDic[sheet.m_NameLst[j]];
                            range.Value = val;
                            range.AutoFitColumns();
                            if (val.Contains("\n") || val.Contains("\r\n"))
                            {
                                range.Style.WrapText = true;
                            }
                        }
                    }

                    worksheet.Cells.AutoFitColumns();
                }
                pack.Save();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }
        Debug.Log("生成" + xlsxPath + "成功！！！");
    }

    /// <summary>
    /// 判断文件是否被占用 打开
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static bool FileIsOpened(string path)
    {
        bool result = false;

        if (!File.Exists(path))
        {
            result = false;
        }
        else
        {
            FileStream fileStream = null;
            try
            {
                fileStream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

                result = false;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                result = true;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        return result;
    }



    /// <summary>
    /// 递归读取类里面的数据
    /// </summaryxml_VarClass
    /// <param name="_object"></param>
    /// <param name="lst"></param>
    /// <param name="lstDic">查找储存值</param>
    /// <param name="sheetDic">查找储存值</param>
    /// <param name="mainKey">子表需要主键值</param>
    /// <param name="idx">调试用的</param>
    private static void ReadData(object _object,
        global::Lst lst,
        Dictionary<string, global::Lst> lstDic,
        Dictionary<string, global::Sheet> sheetDic,
        string mainKey,
        int idx)
    {

        //Debug.Log("ReadData" + idx);


        List<Var> varLst = lst.m_VarList;
        Var _var = lst.m_ParentVar;//这个是AllMonster，只能统一
        //object var_NameLst = Ref_Class_List_Get(  _object, "m_MonsterLst",1  );//第一层var,类的属性名 MonsterData.m_MonsterLst
        object var_NameLst = Ref_Class_Member_Get(_object, Class2XmlChangeString( _var.m_Name) );// 第一层var
        int varCnt = Ref_List_Cnt(var_NameLst, 1);
        global::Sheet sheet = new global::Sheet();


        if (string.IsNullOrEmpty(_var.m_Foreign) == false)//外键不为空
        {
            sheet.m_NameLst.Add(_var.m_Foreign);
            sheet.m_TypeLst.Add(_var.m_Type);
        }

        for (int i = 0; i < varLst.Count; i++)
        {
            string colName = varLst[i].m_Col;
            if (string.IsNullOrEmpty(colName) == false)  //列名
            {
                sheet.m_NameLst.Add(colName);//列名
                sheet.m_TypeLst.Add(varLst[i].m_Type); //变量类型
            }
            else
            {
                //Debug.LogFormat("未设置列名或在拆分子表：{0}", varLst[i].m_Name );
            }
        }

        string tempKey = mainKey; //缓存第一层mainKey，子表外键
        for (int i = 0; i < varCnt; i++) //遍历第一层var，有n个monster
        {
            object item = Ref_List_Get(var_NameLst, i);//这一行
            Row row = new Row();

            if (string.IsNullOrEmpty(_var.m_Foreign) == false
                && string.IsNullOrEmpty(tempKey) == false)
            {
                row.m_RowDataDic.Add(_var.m_Foreign, tempKey);
            }

            if (string.IsNullOrEmpty(lst.m_MainKey) == false)
            {
                mainKey = Ref_Class_Member_Get(item, lst.m_MainKey).ToString();
            }

            for (int j = 0; j < varLst.Count; j++) //第2层var，monster的n个属性
            {

                if (varLst[j].m_Type == "list"
                    && string.IsNullOrEmpty(varLst[j].m_SplitStr) == true) //列表理不是类
                {
                    global::Lst getLst = lstDic[varLst[j].m_SheetName_SelfList];
                    ReadData(item, getLst, lstDic, sheetDic, mainKey, 228);
                }
                else if (varLst[j].m_Type == "list") //列表里面是类，建立子表
                {
                    global::Lst getLst = lstDic[varLst[j].m_SheetName_SelfList];
                    string value = GetClassLst(item, varLst[j], getLst);
                    row.m_RowDataDic.Add(varLst[j].m_Col, value);
                }
                else if ( IsBaseList(varLst[j].m_Type)==true )              //单元格里是分隔符;
                {
                    string value = GetBaseLst(item, varLst[j]);
                    row.m_RowDataDic.Add(varLst[j].m_Col, value);
                }
                else
                {
                    object value = Ref_Class_Member_Get(item, varLst[j].m_Name);
                    if (varLst != null)
                    {
                        row.m_RowDataDic.Add(varLst[j].m_Col, value.ToString());
                    }
                    else
                    {
                        Debug.LogErrorFormat("反射出来为空！", varLst[j].m_Name);   //有可能xml配错了
                    }
                }
            }

            string key = _var.m_SheetName_SelfList;
            if (key != null)
            {
                if (sheetDic.ContainsKey(key) == true)
                {
                    sheetDic[key].m_RowLst.Add(row);
                }
                else
                {
                    sheet.m_RowLst.Add(row);
                    sheetDic.Add(key, sheet);
                }
            }
            else
            {
                Debug.LogError("key为空");
            }

        }
    }


    /// <summary>
    /// 不是类列表
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    static bool IsBaseList(string type)
    {
        return type == "listStr"
            || type == "listFloat"
            || type == "listInt"
            || type == "listBool";
    }



    #region 列表xe
    /// <summary>
    /// 01 获取本身是一个类的列表，但是数据比较少
    /// 02 没办法确定父级结构的
    /// 00用分隔符方式转类列表。类之间用 \n，属性之间用;
    /// </summary>
    /// <returns></returns>
    private static string GetClassLst(object _object, Var _var, global::Lst lst)
    {

        #region 示例
        /*
		<variable  name="AllBuffList" type="list" foregin ="ID" >
			<list name = "BuffTest" sheetname="所有buff的测试list" >
				<variable  name="Id" col = "TestID" type="int"/>
				<variable  name="Name" col = "名字" type="string"/>
			</list>
		</variable> 

        <MonsterBuffList Id="5" Name="全BUFF4" OutLook="Assets/GameData/...4" Time="8.298173" BuffType="Ranshao">
            <AllString>ceshi4</AllString>
            <AllBuffList Id="4" Name="name0" />
            <AllBuffList Id="5" Name="name1" />
        </MonsterBuffList>
         */
        #endregion

        string split = _var.m_SplitStr;
        string classSplit = lst.m_SplitStr;
        string str = "";
        if (string.IsNullOrEmpty(split) || string.IsNullOrEmpty(classSplit))
        {
            Debug.LogError("类的列类分隔符或变量分隔符为空！！！");
            return str;
        }
        object dataList = Ref_Class_Member_Get(_object, _var.m_Name, GetBindingFlags());
        int lstCnt = Ref_List_Cnt(dataList);
        for (int i = 0; i < lstCnt; i++) //list外层的var ，一般只有一个
        {
            object item = Ref_List_Get(dataList, i);
            for (int j = 0; j < lst.m_VarList.Count; j++)// list里面2 个 var
            {
                object value = Ref_Class_Member_Get(item, lst.m_VarList[j].m_Name);
                str += value.ToString();              //
                if (j != lst.m_VarList.Count - 1)
                {
                    str += TrimReg( classSplit);
                }
            }

            if (i != lstCnt - 1)
            {
                str +=  TrimReg(split);
            }
        }
        return str;

    }

    /// <summary>
    /// 获取基础List里面的所有值
    /// 示例处理AllString
    /// </summary>
    /// <param name="_object">数据类</param>
    /// <param name="_var">结构类</param>
    /// <returns></returns>
    private static string GetBaseLst(object _object, Var _var)
    {

        #region 示例
        /** 切割AllString里面的数据，分号连接
         * 数据类
        <AllBuffList Id="3" Name="全BUFF2" OutLook="Assets/GameData/...2" Time="0.95974493" BuffType="Ranshao">
            <AllString>ceshi2</AllString>
            <AllString>ceshiq2</AllString>
            <AllBuffList Id="3" Name="name0" />
        </AllBuffList>

        结构类
        <variable  name="AllString" col = "测试list列" type="listStr" split = ";"/>
        <variable  name="AllBuffList" type="list" foregin ="ID" >
            <list name = "BuffTest" sheetname="所有buff的测试list" >
                <variable  name="Id" col = "TestID" type="int"/>
                <variable  name="Name" col = "名字" type="string"/>
            </list>
        </variable>
         **/
        #endregion

        string str = "";
        if (string.IsNullOrEmpty(_var.m_SplitStr)) //；等
        {
            Debug.LogError("基础List的分隔符为空！");
            return str;
        }

        object dataLst = Ref_Class_Member_Get(_object, _var.m_Name); //AllBuffList找AllString
        int lstCnt = Ref_List_Cnt(dataLst); //2个

        for (int i = 0; i < lstCnt; i++) //列表里面的字符串
        {
            object item = Ref_List_Get(dataLst, i);
            str += item.ToString();
            if (i != lstCnt - 1) //遍历中
            {
                str += TrimReg(_var.m_SplitStr);//分隔符，分号
            }
        }
        return str;
    }

    static Type GetType(Var _var)
    {
        Type type = null;


        switch (_var.m_Type)
        {
            case "listStr":
                {
                    type = typeof(string);
                }
                break;
            case "listFloat":
                {
                    type = typeof(float);
                }
                break;
            case "listInt":
                {
                    type = typeof(int);
                }
                break;
            case "listBool":
                {
                    type = typeof(bool);
                }
                break;
            default: break;
        }

        return  type;
    }

    /// <summary>
    /// 基础（分类）List赋值
    /// </summary>
    /// <param name="_object"></param>
    /// <param name="_var"></param>
    /// <param name="value"></param>
    private static void SetBaseLst(object _object, Var _var, string value)
    {
        object list = Ref_List_New( _var);
        if (list == null)
        {
            Debug.LogErrorFormat("list为空：{0}",list );
            return;
        }
        string[] rowArr = SplitString(value,  _var.m_SplitStr); //分割
        for (int i = 0; i < rowArr.Length; i++)
        {
            object addItem = rowArr[i].Trim();
            try
            {
                Ref_List_Add(list, addItem);
            }
            catch
            {
                Debug.LogFormat("{0}里 {1}列表添加失败！具体数值是：{2}" ,_var.m_SheetName_SelfList , list,addItem  );
            }
        }
        Ref_Class_Member_SetValue(_object, _var.m_Name,list);
     
    }



    /// <summary>
    /// 类列表赋值  （拆分或不拆分）
    /// </summary>
    /// <param name="_object"></param>
    /// <param name="lst"></param>
    /// <param name="rangeVal"></param>
    private static void SetClassLst(object _object, Lst lst, string rangeVal)
    {
        object item = Ref_Class_New(lst.m_Name);
        object list = Ref_List_New(item.GetType());

         
        if (string.IsNullOrEmpty(rangeVal)==true)//打印空白单元格信息
        {
            object Id = Ref_Class_Member_Get(_object, "Id");  ////在_object里面，不确定Class取不了
            if (Id != null)
            {
                Debug.LogFormat("表{0}里面{1}={2}自定义list的列{3}里有空值！","",  "Id",(int)Id , lst.m_ParentVar.m_Col); //找不到正确的SheetName
            }
            else
            { 
                 Debug.LogFormat("表{0}里面自定义list的列{1}里有空值！", "", lst.m_ParentVar.m_Col);
            }
            return;
        }
        else //将单元格中的类列表写入lst
        {
            string splitStr = TrimReg( lst.m_ParentVar.m_SplitStr ); //Reg中读出的要替换
            string[] rangeArr = SplitString(rangeVal,  splitStr);  //行分割 "\n"
            for (int i = 0; i < rangeArr.Length; i++) //遍历每行表格
            {
                object addItem = Ref_Class_New(lst.m_Name);
                string[] rangeLst =   SplitString( rangeArr[i].Trim(), lst.m_SplitStr); // 行的单元格分割
                for (int j = 0; j < rangeLst.Length; j++)
                {
                    Ref_Class_Member_SetValue(addItem, lst.m_VarList[j].m_Name, rangeLst[j].Trim(), lst.m_VarList[j].m_Type);//用类存储起来
                }
                Ref_List_Add(list, addItem );
            }
        }
        Ref_Class_Member_SetValue(_object,lst.m_ParentVar.m_Name, list);
    }


    /// <summary>
    /// 分隔符从Reg读取时要替换
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    static string TrimReg(string str)
    { 
         return str.Replace("\\n", "\n").Replace("\\r", "\r");
    }
    #endregion




    /// <summary>
    /// 读取xml结构,第一层data
    /// </summary>
    /// <param name="regName"></param>
    /// <param name="excelName"></param>
    /// <param name="xmlName"></param>
    /// <param name="className"></param>
    /// <returns></returns>
    private static Dictionary<string, global::Lst> ReadReg(string regName, ref string excelName, ref string xmlName, ref string className)
    {
        string path = m_Xml_OutetrPath + regName + ".xml";
        // string name = Common.TrimName(path, TrimNameType.SlashAndPoint);
        if (File.Exists(path) == false)//判
        {
            Debug.LogFormat("不存在：{0}", path);
        }


        XmlDocument xml = new XmlDocument(); //读Xml
        XmlReader reader = XmlReader.Create(path);
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;//忽略xml里面的注释
        xml.Load(reader);
        XmlNode xn = xml.SelectSingleNode("data");
        XmlElement xe = (XmlElement)xn;
        className = xe.GetAttribute("name");
        xmlName = xe.GetAttribute("to");
        excelName = xe.GetAttribute("from");

        Dictionary<string, global::Lst> lstDic = new Dictionary<string, global::Lst>(); //储存所有变量的表         //递归读取
        ReadXmlNode(xe, lstDic, 0);

        reader.Close();


        return lstDic;
    }

    /// <summary>
    /// 递归读取配置
    /// </summary>
    /// <param name="dataE"></param>
    /// <param name="lstDic">引用类型，不用写ref</param>
    /// <param name="depth">深度</param>
    private static void ReadXmlNode(XmlElement dataE, Dictionary<string, global::Lst> lstDic, int depth)
    {
        #region xml
        /**
        <data name ="MonsterData" from="G怪物.xlsx" to = "MonsterData.xml">
            <variable  name="AllMonster" type="list">
                <list name = "MonsterBase" sheetname="怪物配置" mainKey = "Id">
                    <variable  name="Id" col = "ID" type="int"/>
                    <variable  name="Name" col = "名字" type="string"/>
                    <variable  name="OutLook" col = "预知路径" type="string"/>
                    <variable  name="Level" col = "怪物等级" type="int"/>
                    <variable  name="Rare" col = "怪物稀有度" type="int" defaultValue = "0"/>
                    <variable  name="Height" col = "怪物高度" type="float"/>
                </list>
            </variable>
        </data>
         */
        #endregion

        depth++;
        foreach (XmlNode node in dataE.ChildNodes)
        {
            XmlElement xe = (XmlElement)node;      //02 var1
            if (xe.GetAttribute("type") == "list") //是list
            {
                XmlElement listE = (XmlElement)node.FirstChild;//列表的只有一个节点  //03 _list

                Var _var = SetVar(xe);
                global::Lst _lst = SetLst(listE,  _var, depth);

                if (string.IsNullOrEmpty(_lst.m_SheetName) == false)
                {
                    if (lstDic.ContainsKey(_lst.m_SheetName) == false)
                    {

                        foreach (XmlNode insideNode in listE.ChildNodes)            //获取该类下面所有变量
                        {
                            XmlElement insideE = (XmlElement)insideNode;
                            Var insideVar = SetVar(insideE);

                            _lst.m_VarList.Add(insideVar);
                        }
                        lstDic.Add(_lst.m_SheetName, _lst);
                    }
                }

                ReadXmlNode(listE, lstDic, depth);
            }
        }
    }


   /// <summary>
   /// reg中的xml之一
   /// </summary>
   /// <param name="_xe"></param>
   /// <returns></returns>
    static Var SetVar(XmlElement _xe)
    {
        global::Var _class = new global::Var()                      //list里面的varClass
        {
            m_Name = _xe.GetAttribute("name"),
            m_Type = _xe.GetAttribute("type"),
            m_Col = _xe.GetAttribute("col"),
            m_DeafultValue = _xe.GetAttribute("defaultValue"),
            m_Foreign = _xe.GetAttribute("foreign"),
            m_SplitStr = _xe.GetAttribute("split"),
        };
        if (_class.m_Type == "list")                        //是list,继续读
        {
            _class.m_ClassName_SelfList = ((XmlElement)_xe.FirstChild).GetAttribute("name");
            _class.m_SheetName_SelfList = ((XmlElement)_xe.FirstChild).GetAttribute("sheetname");
        }

        return _class;
    }


    /// <summary>
    /// reg中的xml之一
    /// </summary>
    /// <param name="_xe"></param>
    /// <returns></returns>
    static global::Lst SetLst(XmlElement _xe,  global::Var parentVar, int depth)
    {
        global::Lst _class = new global::Lst()     //lstClass               
        {
            m_Name = _xe.GetAttribute("name"),
            m_SheetName = _xe.GetAttribute("sheetname"),
            m_SplitStr = _xe.GetAttribute("split"),
            m_MainKey = _xe.GetAttribute("mainKey"),
            m_ParentVar = parentVar,
            m_Depth = depth,
        };

        return _class;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="lst"></param>
    /// <param name="idx">调试确定位置</param>
    /// <returns></returns>
    static int Ref_List_Cnt(object lst, int idx = -1)
    {
        if (lst == null)
        {
            Debug.LogErrorFormat("Ref_List_Cnt{0}为空,idx={1}", lst, idx);
            return -1;
        }

        return System.Convert.ToInt32(
            lst.GetType().InvokeMember("get_Count",
            BindingFlags.Default | BindingFlags.InvokeMethod,
            null, lst, new object[] { })
        );
    }
    static object Ref_List_Add(object lst, object item)
    {
        return lst.GetType().InvokeMember("Add", BindingFlags.Default | BindingFlags.InvokeMethod, null, lst, new object[] { item });
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="lst"></param>
    /// <param name="idx">索引</param>
    /// <returns></returns>
    static object Ref_List_Get(object lst, int idx)
    {
        return lst.GetType().InvokeMember("get_Item", BindingFlags.Default | BindingFlags.InvokeMethod, null, lst, new object[] { idx });
    }


    static object Ref_List_New(Type type)
    {
        Type lstType = typeof(List<>);
        Type finalType = lstType.MakeGenericType(new Type[] { type });
        object lst = Activator.CreateInstance(finalType, new object[] { });

        return lst;
    }

    static object Ref_List_New(Var _var)
    {
        Type type = GetType(_var);
        if (type == null)
        { 
            Debug.LogErrorFormat("type为空：{0}",type );
            return null;
        }

        Type lstType = typeof(List<>);
        Type finalType = lstType.MakeGenericType(new Type[] { type });
        object lst = Activator.CreateInstance(finalType, new object[] { });

        return lst;
    }


    /// <summary>
    /// 设置类中属性的值 v2
    /// </summary>
    /// <param name="_object"></param>
    /// <param name="memberName"></param>
    /// <param name="memberVal"></param>
    static void Ref_Class_Member_SetValue(object _object, string memberName, object memberVal, string memberType)
    {
        PropertyInfo pi = _object.GetType().GetProperty(memberName);


        switch (memberType)
        {
            case "int":
                {
                    memberVal = Convert.ToInt32(memberVal);
                }
                break;
            case "float":
                {
                    memberVal = Convert.ToSingle(memberVal);
                }
                break;
            case "double":
                {

                }
                break;
            case "enum":
                {
                    memberVal = TypeDescriptor.GetConverter(pi.PropertyType).ConvertFromInvariantString(memberVal.ToString()); //枚举
                }
                break;
            case "string": break;
            case "bool":
                {
                    memberVal = Convert.ToBoolean(memberVal);
                }
                break;
            default: break;
        }

        pi.SetValue(_object, memberVal);
    }


    /// <summary>
    /// 设置类中属性的值  v1
    /// </summary>
    /// <param name="_object"></param>
    /// <param name="memberName"></param>
    /// <param name="memberVal"></param>
    static void Ref_Class_Member_SetValue(object _object, string memberName, object memberVal)
    {
        PropertyInfo pi = _object.GetType().GetProperty( Class2XmlChangeString( memberName) );
        pi.SetValue(_object, memberVal );
    }

    public static string[] SplitString(string _string, string splitStr)
    {
        return _string.Split(new string[] { splitStr }, StringSplitOptions.None);
    }


    /// <summary>
    /// 反射创建类的实例
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static object Ref_Class_New(string name)
    {
        object obj = null;
        Type type = null;
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type tempType = asm.GetType(name);
            if (tempType != null)
            {
                type = tempType;
                break;
            }
        }
        if (type != null)
        {
            obj = Activator.CreateInstance(type);
        }
        return obj;
    }


    /// <summary>                                                
    /// 得到该类该列表属性的所有对象，返回表
    /// </summary>
    /// <param name="_object"></param>
    /// <param name="memberName"></param>
    /// <param name="idx">调试找位置用的</param>
    /// <returns></returns>
    static List<object> Ref_Class_List_Get(object _object, string memberName, int idx = -1)
    {

        Debug.Log("Ref_Class_List_Get" + idx);
        object lst = Ref_Class_Member_Get(_object, memberName);
        int lstCnt = Ref_List_Cnt(lst, 2);
        //
        List<object> resLst = new List<object>();
        for (int i = 0; i < lstCnt; i++)
        {
            object item = lst.GetType().InvokeMember("get_Item",
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null, lst, new object[] { i });
            resLst.Add(item);
        }

        return resLst;
    }



    /// <summary>
    /// 类反射得到字段属性值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_object"></param>
    /// <param name="memberName"></param>
    /// <param name="bindingFlags"></param>
    /// <returns></returns>
    static object Ref_Class_Member_Get(object _object,
        string memberName,
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance,
        int i = -1)
    {

        #region 测试
        /**
           Test test= new Test();
           test.m_Lst=new List<int> { 1, 2, 3 };
           Type type = test.GetType();
           MemberInfo[] miArr = type.GetMember("m_Lst", bindingFlags);
           _object = test;
        /**/
        #endregion

        Type type = _object.GetType();
        MemberInfo[] miArr = type.GetMember(memberName, bindingFlags);

        while (miArr == null || miArr.Length <= 0) //保护性措施
        {
            type = type.BaseType;
            if (type == null)
            {
                return null;
            }
            miArr = type.GetMember(memberName, BindingFlags.Default | BindingFlags.Public);
        }

        object res = null;
        switch (miArr[0].MemberType)
        {
            case MemberTypes.Field:
                {
                    res = type.GetField(memberName, bindingFlags).GetValue(_object);
                }
                break;
            case MemberTypes.Property:
                {
                    res = type.GetProperty(memberName, bindingFlags).GetValue(_object);
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

    /// <summary>
    /// 反序列化xml到类
    /// </summary>
    /// <param name="className"></param>
    /// <returns></returns>
    private static object ClassName2Object(string className)
    {
        Type type = null;
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type tempType = asm.GetType(className);
            if (tempType != null)
            {
                type = tempType;
                break;
            }
        }
        if (type != null)
        {
            string xmlPath = m_Xml_InnerPath + className + ".xml";
            object obj = FormatTool.Xml2Class(xmlPath, type);
            return obj;

            // return FormatTool.Xml2Class(xmlPath, type);
        }

        return null;
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

    /// <summary>
    /// 当xml命名风格和C#类命名风格冲突，转接表
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    static string Class2XmlChangeString(string name)
    {
        if (name == "m_MonsterLst") return "AllMonster";
        if (name == "AllMonster") return "m_MonsterLst";
        return name;
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
                string binPath = m_Bin_InnerPath + name + ".bytes";
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
    /// <param name="name">既是类名，也是所选文件名</param>
    static void Class2Xml(string name)
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
            FormatTool.Class2Xml(xmlPath, temp);
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


    private static void Xml2Protobuf(string name)
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
                string protobufPath = m_Protobuf_InnerPath + name + ".bytes";
                object obj = FormatTool.Xml2Class(xmlPath, type);
                FormatTool.Class2Protobuf(protobufPath, obj);
                Debug.LogFormat("{0}：Xml2Protobuf成功，Protobuf路径为:{1}", name, protobufPath);
            }
        }
        catch
        {
            Debug.LogErrorFormat( "{0}：Xml2Protobuf失败！", name);
        }
    }



    static void Protobuf2Xml(string name)
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
           // FormatTool.Protobuf2Xml(xmlPath, temp);
            Debug.Log(name + "Protobuf转xml成功，xml路径为:" + xmlPath);
        }
    }

    #endregion



}



#region XML 结构 Reg

/// <summary>
/// 变量类
/// </summary>
public class Var
{

    public string m_Name { get; set; }  //原类里面变量的名称
    public string m_Type { get; set; } //变量类型
    public string m_Col { get; set; }   //变量对应的Excel里的列
    public string m_DeafultValue { get; set; } //变量的默认值
    public string m_Foreign { get; set; }     //变量是list的话，外联部分列
    public string m_SplitStr { get; set; }    //分隔符
    public string m_ClassName_SelfList { get; set; }   //如果自己是List，对应的list类名
    public string m_SheetName_SelfList { get; set; } //如果自己是list,对应的sheet名
}

/// <summary>
/// xml list的节点
/// </summary>
public class Lst
{

    public Var m_ParentVar { get; set; } //所属父级Var变量
    public int m_Depth { get; set; }    //深度
    public string m_Name { get; set; }    //类名
    public string m_SheetName { get; set; } //类对应的sheet名
    public string m_MainKey { get; set; } //主键
    public string m_SplitStr { get; set; }    //分隔符

    public List<Var> m_VarList = new List<Var>();     //所包含的变量
}

#endregion


#region Excel   数据
/// <summary>
/// Excel的数据 ,工作表
/// </summary>
public class Sheet 
{
    public List<string> m_NameLst = new List<string>();//列名
    public List<string> m_TypeLst = new List<string>();//列的类型
    public List<Row> m_RowLst = new List<Row>();   //列的数据
}


/// <summary>
/// 行
/// </summary>
public class Row
{
    public string m_ParentVal = "";  //外键
    public Dictionary<string, string> m_RowDataDic = new Dictionary<string, string>();
}

#endregion


#region 反射 测试类
public class TestReflection
{
    //public int m_ID;
    //public string m_Name;
    //public bool m_Female;       
    //或
    public int m_ID { get; set; }
    public string m_Name { get; set; }
    public bool m_Female { get; set; }
    public List<string> m_Lst { get; set; }
    public List<TestReflectionSub> m_SubLst { get; set; }
}

public class TestReflectionSub
{
    public string m_Name { get; set; }
}

public class TestReflection01
{
    //public int m_ID;
    //public string m_Name;
    //public bool m_Female;       
    //或
    public int m_ID { get; set; }
    public string m_Name { get; set; }
    public bool m_Female { get; set; }

}
public class TestReflection02
{
    //public int m_ID;
    //public string m_Name;
    //public bool m_Female;       
    //或
    public int m_ID { get; set; }
    public string m_Name { get; set; }
    public bool m_Female { get; set; }

    public float m_Height { get; set; }

    public Rank02 m_Rank { get; set; }
    public List<string> m_Lst { get; set; }

    public List<TestReflectionSub> m_SubLst { get; set; }
}

public enum Rank02
{
    None = 0,
    One = 1,
    Two = 2,
    Three = 3,
}


public class Test
{
    public List<int> m_Lst { get; set; }
}

#endregion