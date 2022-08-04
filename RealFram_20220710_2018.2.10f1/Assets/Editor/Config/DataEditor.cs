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
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class DataEditor
{
    const string m_Xml_InnerPath = "Assets/GameData/Data/Xml/";
    static string m_Xml_OutetrPath = DefinePath.ProjectRoot + "Data/Reg/";
    static string m_Excel_OutetrPath = DefinePath.ProjectRoot + "Data/Excel/";
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



    [MenuItem("测试/Reflection/根据反射读取类的属性值", false, 4)]//按钮在菜单栏的位置
    static void Test_Reflection() //读取工程下xmlPath的xml
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

        List<object> lst = Ref_Class_List_Get(testRef, "m_Lst",-1);
        List<object> subLst = Ref_Class_List_Get(testRef, "m_SubLst",-1);
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


    [MenuItem("测试/Reflection/数据反射成类", false, 4)]//按钮在菜单栏的位置
    static void Test_ReflectionByData() //读取工程下xmlPath的xml
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


    [MenuItem("测试/Reflection/数据反射成类(浮点，枚举)", false, 4)]//按钮在菜单栏的位置
    static void Test_ReflectionByData_Float_Enum() //读取工程下xmlPath的xml
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

    [MenuItem("测试/Reflection/数据反射成类(列表)", false, 4)]//按钮在菜单栏的位置
    static void Test_ReflectionByData_Lst() //读取工程下xmlPath的xml
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


    #region 说明



    [MenuItem("测试/Reflection/Xml2Excel", false, 4)]//按钮在菜单栏的位置
    static void Xml2Excel() //读取工程下xmlPath的xml
    {

        string className = "";
        string xmlName = "";
        string excelName = "";
        UnityEngine.Object[] obj = Selection.objects;
        string name = obj[0].name; //"MonsterData";
        //
        Dictionary<string, Lst> lstDic = ReadReg(name, ref excelName, ref xmlName, ref className); //读取结构
        object _object = ClassName2Object(className);



        List<Lst> lstLst = new List<Lst>();
        foreach (Lst lst in lstDic.Values)
        {
            if ( lst.m_Depth == 1)
            {
                lstLst.Add(lst);
            }
        }

        Dictionary<string, global::Sheet> sheetDic = new Dictionary<string, global::Sheet>();
        for (int i = 0; i < lstLst.Count; i++)    //Lst节点
        {
            ReadData( _object, lstLst[i], lstDic, sheetDic, "",60 );
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
            if (fi.Exists==true)
            {
               // fi.Delete();
                
            }
            fi = new FileInfo(xlsxPath);
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
    private static void ReadData(object _object, 
        Lst lst, 
        Dictionary<string, Lst> lstDic, 
        Dictionary<string, global::Sheet> sheetDic, 
        string mainKey,
        int idx)
    {

        Debug.Log("hkdgkcdckd"+idx);


        List<Var> varLst = lst.m_VarList;
        Var _var = lst.m_ParentVar;
        //object var_NameLst = Ref_Class_List_Get(  _object, "m_MonsterLst",1  );//第一层var
        object var_NameLst = Ref_Class_Member_Get(_object, "m_MonsterLst");// 第一层var
        int varCnt = Ref_List_Cnt(  var_NameLst ,1);
        Sheet sheet = new Sheet();


        if (  string.IsNullOrEmpty(_var.m_Foregin) ==false )
        {
            sheet.m_NameLst.Add(_var.m_Foregin);
            sheet.m_TypeLst.Add(_var.m_Type);
        }

        for (int i = 0; i < varLst.Count; i++)
        {
            if ( string.IsNullOrEmpty(varLst[i].m_Col) ==false )
            {
                sheet.m_NameLst.Add(varLst[i].m_Col);
                sheet.m_TypeLst.Add(varLst[i].m_Type);
            }
        }

        string tempKey = mainKey; //第一层mainKey
        for (int i = 0; i < varCnt; i++) //遍历第一层var，有n个monster
        {
            object item = Ref_List_Get(var_NameLst, i);//这一行
            Row row = new Row();

            if (  string.IsNullOrEmpty(_var.m_Foregin)==false && string.IsNullOrEmpty(tempKey)==false  )
            {
                row.m_RowDataDic.Add(_var.m_Foregin, tempKey);
            }

            if (  string.IsNullOrEmpty(lst.m_MainKey)==false  )
            {
                mainKey = Ref_Class_Member_Get(item, lst.m_MainKey).ToString();
            }

            for (int j = 0; j < varLst.Count; j++) //第2层var，monster的n个属性
            {

                if (varLst[j].m_Type == "list" && string.IsNullOrEmpty(varLst[j].m_SplitStr))
                {
                    Lst getLst = lstDic[varLst[j].m_SheetName_SelfList];
                    ReadData(item, getLst, lstDic, sheetDic, mainKey,228);
                }
                else if (varLst[j].m_Type == "list")
                {
                    Lst getLst = lstDic[varLst[j].m_SheetName_SelfList];
                    string value = GetSplitStrList(item, varLst[j], getLst);
                    row.m_RowDataDic.Add(varLst[j].m_Col, value);
                }
                else if (varLst[j].m_Type == "listStr" 
                    || varLst[j].m_Type == "listFloat" 
                    || varLst[j].m_Type == "listInt" 
                    || varLst[j].m_Type == "listBool")     
                {
                    string value = GetSpliteBaseList(item, varLst[j]);
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
                        Debug.LogErrorFormat(  "反射出来为空！",varLst[j].m_Name);   //有可能xml配错了
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
    /// 获取本身是一个类的列表，但是数据比较少；（没办法确定父级结构的）
    /// </summary>
    /// <returns></returns>
    private static string GetSplitStrList(object data, Var varClass, Lst sheetClass)
    {
        string split = varClass.m_SplitStr;
        string classSplit = sheetClass.m_SplitStr;
        string str = "";
        if (string.IsNullOrEmpty(split) || string.IsNullOrEmpty(classSplit))
        {
            Debug.LogError("类的列类分隔符或变量分隔符为空！！！");
            return str;
        }
        object dataList = Ref_Class_Member_Get(data, varClass.m_Name, GetBindingFlags());
            int listCount = System.Convert.ToInt32(dataList.GetType().InvokeMember("get_Count", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { }));
            for (int i = 0; i < listCount; i++)
            {
                object item = dataList.GetType().InvokeMember("get_Item", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { i });
                for (int j = 0; j < sheetClass.m_VarList.Count; j++)
                {
                    object value = Ref_Class_Member_Get(item, sheetClass.m_VarList[j].m_Name, GetBindingFlags());
                    str += value.ToString();
                    if (j != sheetClass.m_VarList.Count - 1)
                    {
                        str += classSplit.Replace("\\n", "\n").Replace("\\r", "\r");
                    }
                }

                if (i != listCount - 1)
                {
                    str += split.Replace("\\n", "\n").Replace("\\r", "\r");
                }
            }
            return str;
        
    }

        /// <summary>
        /// 获取基础List里面的所有值
        /// </summary>
        /// <returns></returns>
        private static string GetSpliteBaseList(object data, Var varClass)
        {
            string str = "";
            if (string.IsNullOrEmpty(varClass.m_SplitStr))
            {
                Debug.LogError("基础List的分隔符为空！");
                return str;
            }
            object dataList = Ref_Class_List_Get(data, varClass.m_Name,4);
            int listCount = System.Convert.ToInt32(dataList.GetType().InvokeMember("get_Count", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { }));

            for (int i = 0; i < listCount; i++)
            {
                object item = dataList.GetType().InvokeMember("get_Item", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { i });
                str += item.ToString();
                if (i != listCount - 1)
                {
                    str += varClass.m_SplitStr.Replace("\\n", "\n").Replace("\\r", "\r");
                }
            }
            return str;
        }



   /// <summary>
   /// 读取xml结构,第一层data
   /// </summary>
   /// <param name="name"></param>
   /// <param name="excelName"></param>
   /// <param name="xmlName"></param>
   /// <param name="className"></param>
   /// <returns></returns>
    private static Dictionary<string, Lst> ReadReg(string name, ref string excelName, ref string xmlName, ref string className)
    {
        string path = m_Xml_OutetrPath + name + ".xml";
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
       
        Dictionary<string, Lst> lstDic = new Dictionary<string, Lst>(); //储存所有变量的表         //递归读取
        ReadXmlNode(xe, lstDic,0);

        reader.Close();


        return lstDic;
    }

    /// <summary>
    /// 递归读取配置
    /// </summary>
    /// <param name="dataE"></param>
    /// <param name="lstDic">引用类型，不用写ref</param>
    /// <param name="depth">深度</param>
    private static void ReadXmlNode(XmlElement dataE, Dictionary<string, Lst> lstDic, int depth)
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

                Var _var = SetVar( xe);
                Lst _lst = SetLst( listE, ref _var,  depth);

                if (string.IsNullOrEmpty(_lst.m_SheetName)==false)
                {
                    if (lstDic.ContainsKey(_lst.m_SheetName)==false)
                    {
                        
                        foreach (XmlNode insideNode in listE.ChildNodes)            //获取该类下面所有变量
                        {
                            XmlElement insideE = (XmlElement)insideNode;
                            Var insideVar = SetVar( insideE);

                            _lst.m_VarList.Add(insideVar);
                        }
                        lstDic.Add(_lst.m_SheetName, _lst);
                    }
                }

                ReadXmlNode(listE, lstDic, depth);
            }
        }
    }




    #region 辅助


    static Var SetVar( XmlElement _xe)
    {
        global::Var _class = new global::Var()                      //list里面的varClass
        {
            m_Name = _xe.GetAttribute("name"),
            m_Type = _xe.GetAttribute("type"),
            m_Col = _xe.GetAttribute("col"),
            m_DeafultValue = _xe.GetAttribute("defaultValue"),
            m_Foregin = _xe.GetAttribute("foregin"),
            m_SplitStr = _xe.GetAttribute("split"),
        };
        if (_class.m_Type == "list")                        //是list,继续读
        {
            _class.m_LstName_SelfList = ((XmlElement)_xe.FirstChild).GetAttribute("name");
            _class.m_SheetName_SelfList = ((XmlElement)_xe.FirstChild).GetAttribute("sheetname");
        }

        return _class;
    }

    static Lst SetLst( XmlElement _xe, ref global::Var parentVar,  int depth)
    {
        Lst _class = new Lst()     //lstClass               
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



    static int Ref_List_Cnt(object lst,int idx)
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
    
    
    static object Ref_List_Get(object lst, int i)
    {
        return lst.GetType().InvokeMember("get_Item", BindingFlags.Default | BindingFlags.InvokeMethod, null, lst, new object[] { i });
    }


    static object Ref_List_New(Type type)
    {
        Type lstType = typeof(List<>);
        Type finalType = lstType.MakeGenericType(new Type[] { type });
        object lst = Activator.CreateInstance(finalType, new object[] { });

        return lst;
    }


    /// <summary>
    /// 设置类中属性的值 v2
    /// </summary>
    /// <param name="_class"></param>
    /// <param name="memberName"></param>
    /// <param name="memberVal"></param>
    static void Ref_Class_Member_SetValue(object _class, string memberName, object memberVal, string memberType)
    { 
        PropertyInfo pi = _class.GetType().GetProperty( memberName );


        switch (memberType)
        {
            case "int":
                {
                       memberVal = Convert.ToInt32(memberVal);
                }
                break;
            case "float":
                {
                      memberVal=Convert.ToSingle(memberVal);
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
            default:break;
        }

        pi.SetValue(_class, memberVal);
    }


    /// <summary>
    /// 设置类中属性的值  v1
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="memberName"></param>
    /// <param name="val"></param>
    static void Ref_Class_Member_SetValue(object obj, string memberName, object val)
    {
        PropertyInfo pi = obj.GetType().GetProperty(memberName);

        pi.SetValue(obj, val);
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
    static List<object> Ref_Class_List_Get(object _object, string memberName,int idx)
    {
        
        Debug.Log("Ref_Class_List_Get" + idx);
        object lst = Ref_Class_Member_Get( _object, memberName );
        int lstCnt = Ref_List_Cnt( lst ,2);
        //
        List<object> resLst = new List<object>();
        for (int i = 0; i < lstCnt; i++)
        {
            object item = lst.GetType().InvokeMember("get_Item", BindingFlags.Default | BindingFlags.InvokeMethod, null, lst, new object[] { i });
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
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
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
            miArr = type.GetMember(memberName, BindingFlags.Default|BindingFlags.Public);
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
            string xmlPath = m_Xml_InnerPath + className +".xml";
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



#region XML

/// <summary>
/// 变量类
/// </summary>
public class Var
{
  
    public string m_Name { get; set; }  //原类里面变量的名称
    public string m_Type { get; set; } //变量类型
    public string m_Col { get; set; }   //变量对应的Excel里的列
    public string m_DeafultValue { get; set; } //变量的默认值
    public string m_Foregin { get; set; }     //变量是list的话，外联部分列
    public string m_SplitStr { get; set; }    //分隔符
    public string m_LstName_SelfList { get; set; }   //如果自己是List，对应的list类名
    public string m_SheetName_SelfList { get; set; } //如果自己是list,对应的sheet名
}

/// <summary>
/// xml list的节点
/// </summary>
public class Lst
{
   
    public Var m_ParentVar { get; set; } //所属父级Var变量
    public int    m_Depth { get; set; }    //深度
    public string m_Name { get; set; }    //类名
    public string m_SheetName { get; set; } //类对应的sheet名
    public string m_MainKey { get; set; } //主键
    public string m_SplitStr { get; set; }    //分隔符

    public List<Var> m_VarList = new List<Var>();     //所包含的变量
}

#endregion


#region Excel
/// <summary>
/// Excel的数据 ,工作表
/// </summary>
public class Sheet
{
    public List<string> m_NameLst = new List<string>();
    public List<string> m_TypeLst = new List<string>();
    public List<Row> m_RowLst = new List<Row>();   //每一行的数据是
}


/// <summary>
/// 行
/// </summary>
public class Row
{
    public string m_ParentVal = "";
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

}  public class TestReflection02
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