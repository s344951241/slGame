//using Excel;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Data;
//using System.IO;
//using System.Text;
//using UnityEditor;
//using UnityEngine;

//public class ExcelRead : Editor {

//    private static string path = Application.dataPath + "AssetsLibrary/Config/excel";

//    private static string CSharpVOPath = Application.dataPath + "/Scripts/VO";

//    private static string EditorPath = Application.dataPath + "/Editor";

//    //[MenuItem("config/obj")]
//    //public static void tes()
//    //{
//    //    //ScriptableObject demo = ScriptableObject.CreateInstance("DemoList");
//    //    DemoList demo = ScriptableObject.CreateInstance<DemoList>();
//    //    Demo de = new Demo();
//    //    de.Id = 1;
//    //    de.Name = "sl";
//    //    de.FloatCol = 0.1f;
//    //    demo.demoList.Add(de);
        
//    //    string path = "Assets/Resources/Data/DemoList.asset";
//    //    AssetDatabase.CreateAsset(demo, path);
//    //}

//    [MenuItem("config/excel配置")]
//    public static void readExcels()
//    {
//        if (!Directory.Exists(path))
//        {
//            Directory.CreateDirectory(path);
//            EditorUtility.DisplayDialog("标题", "不存在Excel路径，已创建请添加Excel", "确认", "取消");
           
//        }
//        else
//        {
//            DirectoryInfo folder = new DirectoryInfo(path);
//            FileInfo[] fileInfo = folder.GetFiles();
//            cleanCS();
//            for (int i = 0; i < fileInfo.Length; i++)
//            {
//                if (fileInfo[i].Name.Contains(".xlsx") && (!fileInfo[i].Name.Contains(".meta")))
//                {
//                    Debug.Log(fileInfo[i].Name);
//                    readExcel(fileInfo[i].Name);
//                }
//            }
//        }
//        AssetDatabase.Refresh();
//    }

//    private static void readExcel(string name)
//    {
//        FileStream stream = File.Open(path + "/"+name, FileMode.Open, FileAccess.Read);
//        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
//        DataSet result = excelReader.AsDataSet();

//        int columns = result.Tables[0].Columns.Count;
//        int rows = result.Tables[0].Rows.Count;
//        string className = name.Replace(".xlsx", "");
//        StreamWriter sw = new StreamWriter(CSharpVOPath + "/" + className + ".cs",true, Encoding.UTF8);
//        sw.WriteLine("using System;");
//        sw.WriteLine("using System.Collections;");
//        sw.WriteLine("using System.Collections.Generic;");
//        sw.WriteLine("using UnityEngine;");
//        sw.WriteLine("[Serializable]");
//        sw.WriteLine("public class "+ className + " {");
//        sw.WriteLine("");
//        for (int i = 0; i < columns; i++)
//        {
//            string str = "";
//            for (int j = 1; j < rows; j++)
//            {
//                string nvalue = result.Tables[0].Rows[j][i].ToString();
//                str += "\t" + nvalue;
//            }
//            sw.WriteLine("[SerializeField]");
//            sw.WriteLine("public "+ result.Tables[0].Rows[2][i].ToString()+" "+ result.Tables[0].Rows[1][i].ToString()+";");
//            Debug.Log(str);
//        }
//        sw.WriteLine("}");
//        sw.Close();
//        /////create List
//        //StreamWriter swL = new StreamWriter(CSharpVOPath + "/" + className+"List"+ ".cs", true, Encoding.UTF8);
//        //swL.WriteLine("using System.Collections;");
//        //swL.WriteLine("using System.Collections.Generic;");
//        //swL.WriteLine("using UnityEngine;");
//        //swL.WriteLine("public class "+className+ "List:ScriptableObject{");
//        //swL.WriteLine("    public List<" + className + "> demoList = new List<"+className+">();");
//        //swL.WriteLine("}");
//        //swL.Close();

//        ////create objEditor
//        //StreamWriter swE = new StreamWriter(EditorPath + "/ObjectCreate.cs", true, Encoding.UTF8);
//        //swE.WriteLine("using System.Collections;");
//        //swE.WriteLine("using System.Collections.Generic;");
//        //swE.WriteLine("using UnityEngine;");
//        //swE.WriteLine("using UnityEditor;");
//        //swE.WriteLine("public class ObjectCreate:Editor{");
//        //swE.WriteLine("    [MenuItem(\"config/obj\")]");
//        //swE.WriteLine("    public static void test()");
//        //swE.WriteLine("    {");
//        //swE.WriteLine("         DemoList demo = ScriptableObject.CreateInstance<DemoList>();");
//        //swE.WriteLine("         Demo de = new Demo();");
//        //swE.WriteLine("         de.Id = 1;");
//        //swE.WriteLine("         de.Name = \"sl\";");
//        //swE.WriteLine("         de.FloatCol = 0.1f;");
//        //swE.WriteLine("         demo.demoList.Add(de);");
//        //swE.WriteLine("         string path = \"Assets/Resources/Data/DemoList.asset\";");
//        //swE.WriteLine("         AssetDatabase.CreateAsset(demo, path);");
//        //swE.WriteLine("    }");
//        //swE.WriteLine("}");
//        //swE.Close();
//    }
//    private static void cleanCS()
//    {
//        if (!Directory.Exists(CSharpVOPath))
//        {
//            Directory.CreateDirectory(CSharpVOPath);
//            AssetDatabase.Refresh();
//        }

//        DirectoryInfo folder = new DirectoryInfo(CSharpVOPath);
//        FileInfo[] fileInfo = folder.GetFiles();
//        if (fileInfo.Length != 0)
//        {
//            for (int i = 0; i < fileInfo.Length; i++)
//            {
//                fileInfo[i].Delete();
//            }
//        }

//    }
//}
