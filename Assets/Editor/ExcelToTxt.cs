using Excel;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ExcelToTxt : Editor {

    private static string path = Application.dataPath + "/AssetsLibrary/Config/excel";
    private static string txtPath = Application.dataPath + "/Resources/GameAssets/Configs";
    private static string CSharpVOPath = Application.dataPath + "/Scripts/vo";
    [MenuItem("config/txt")]
    public static void test()
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            EditorUtility.DisplayDialog("标题", "不存在Excel路径，已创建请添加Excel", "确认", "取消");

        }
        else
        {
            DirectoryInfo folder = new DirectoryInfo(path);
            FileInfo[] fileInfo = folder.GetFiles();
            cleanTxt();
            for (int i = 0; i < fileInfo.Length; i++)
            {
                if (fileInfo[i].Name.Contains(".xlsx") && (!fileInfo[i].Name.Contains(".meta")))
                {
                    Debug.Log(fileInfo[i].Name);
                    excelToTxt(fileInfo[i].Name);
                }
            }
        }
        AssetDatabase.Refresh();
    }

    private static void cleanTxt()
    {
        if (!Directory.Exists(txtPath))
        {
            Directory.CreateDirectory(txtPath);
            AssetDatabase.Refresh();
        }

        DirectoryInfo folder = new DirectoryInfo(txtPath);
        FileInfo[] fileInfo = folder.GetFiles();
        if (fileInfo.Length != 0)
        {
            for (int i = 0; i < fileInfo.Length; i++)
            {
                fileInfo[i].Delete();
            }
        }

    }

    private static void excelToTxt(string name)
    {
        Debug.Log(">>>>>>>>>>>"+name);
        FileStream stream = File.Open(path + "/" + name, FileMode.Open, FileAccess.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        DataSet result = excelReader.AsDataSet();
        for (int num = 0; num < result.Tables.Count; num++)
        {
            Debug.Log(">>>>>>>>>>>" + result.Tables[num].TableName);
            excelToVo(result.Tables[num]);
            int columns = result.Tables[num].Columns.Count;
            int rows = result.Tables[num].Rows.Count;
            string className = result.Tables[num].TableName;
            StreamWriter sw = new StreamWriter(txtPath + "/" + className + ".txt", true, Encoding.UTF8);
            for (int i = 0; i < rows; i++)
            {
                string str = "";
                for (int j = 0; j < columns; j++)
                {
                    string nvalue = result.Tables[0].Rows[i][j].ToString();
                    if (string.IsNullOrEmpty(str))
                    {
                        str += nvalue;
                    }
                    else
                    {
                        str += "," + nvalue;
                    }
                   
                }
                sw.WriteLine(str);
            }
            sw.Close();
        }

        excelReader.Close();
        stream.Close();


    }

    private static void excelToVo(DataTable table)
    {
        int columns = table.Columns.Count;
        int rows = table.Rows.Count;
        string className = table.TableName + "VO";
        if (File.Exists(CSharpVOPath + "/" + className + ".cs"))
        {
            File.Delete(CSharpVOPath + "/" + className + ".cs");
        }
        StreamWriter sw = new StreamWriter(CSharpVOPath + "/" + className + ".cs",false, Encoding.UTF8);
        sw.WriteLine("using System;");
        sw.WriteLine("using System.Collections;");
        sw.WriteLine("using System.Collections.Generic;");
        sw.WriteLine("using UnityEngine;");
        sw.WriteLine("public class " + className+ " {");
        sw.WriteLine("");
        sw.WriteLine("  static int[] _Keys;");
        sw.WriteLine("  static SortedList<int, "+className+"> _Data;");
        string[] setDataStrs = new string[columns];
        string id = "Id";
        for (int i = 0; i < columns; i++)
        {
            string str = "";
            for (int j = 1; j < rows; j++)
            {
                string nvalue = table.Rows[j][i].ToString();
                str += "\t" + nvalue;
            }
            if (i == 0)
            {
                id = table.Rows[1][i].ToString();
            }
            sw.WriteLine("  public " + table.Rows[2][i].ToString() + " " + table.Rows[1][i].ToString() + ";");

            switch (table.Rows[2][i].ToString())
            {
                case ExcelType.type1:
                    setDataStrs[i] = "      int.TryParse(tokens[" + i + "], out data." + table.Rows[1][i].ToString() + ");";
                    break;
                case ExcelType.type2:
                    setDataStrs[i] = "      float.TryParse(tokens[" + i + "], out data." + table.Rows[1][i].ToString() + ");";
                    break;
                case ExcelType.type3:
                    setDataStrs[i] = "      if (tokens[" + i + "] != null)\n" +
                                     "           data." + table.Rows[1][i].ToString() + " = tokens[" + i + "];";
                    break;
                case ExcelType.type4:
                    setDataStrs[i] = "      if (tokens[" + i + "] != null)\n" +
                                     "           data." + table.Rows[1][i].ToString() + " = new Table(tokens[" + i + "]); ";
                    break;
                case ExcelType.type5:
                    setDataStrs[i] = "      bool.TryParse(tokens[" + i + "], out data." + table.Rows[1][i].ToString() + ");";
                    break;
                default:
                    break;
            }
            Debug.Log(str);
        }
        //SetVo  TODO
        sw.WriteLine("  public static void SetVO(string text)");
        sw.WriteLine("  {");
        sw.WriteLine("      "+className+" data = new "+className+"();");
        sw.WriteLine("      string[] tokens = Table.GetTokens(text);");
        for (int i = 0; i < setDataStrs.Length; i++)
        {
            sw.WriteLine(setDataStrs[i]);
        }
        sw.WriteLine("      _Data[data."+id+"] = data;");
        sw.WriteLine("  }");
        //GetVo
        sw.WriteLine("  public static " + className + " GetConfig(int id)");
        sw.WriteLine("  {");
        sw.WriteLine("      if(_Data==null)");
        sw.WriteLine("          LoadVO();");
        sw.WriteLine("     "+className + " data = null;");
        sw.WriteLine("      _Data.TryGetValue(id, out data);");
        sw.WriteLine("      return data;");
        sw.WriteLine("  }");
        //zgetKeys
        sw.WriteLine("  public static int[] GetKeys()");
        sw.WriteLine("  {");
        sw.WriteLine("      if (_Data == null)");
        sw.WriteLine("           LoadVO();");
        sw.WriteLine("      if (_Keys == null)");
        sw.WriteLine("      {");
        sw.WriteLine("          IList<int> keys = _Data.Keys;");
        sw.WriteLine("          _Keys = new int[_Data.Count];");
        sw.WriteLine("          for (int i = 0; i < _Keys.Length; i++)");
        sw.WriteLine("          {");
        sw.WriteLine("              _Keys[i] = keys[i];");
        sw.WriteLine("          }");
        sw.WriteLine("          return _Keys;");
        sw.WriteLine("      }");
        sw.WriteLine("       return null;");
        sw.WriteLine("  }");
        //LoadVO
        sw.WriteLine("  static void LoadVO()");
        sw.WriteLine("  {");
        sw.WriteLine("      _Data = new SortedList<int, "+className+">();");
        sw.WriteLine("      string text = ConfigManager.GetData(\""+table.TableName+"\");");
        sw.WriteLine("      string[] rows = text.Split(new char[]{'\\r','\\n'},StringSplitOptions.RemoveEmptyEntries);");
        sw.WriteLine("      int count = rows.Length;");
        sw.WriteLine("      for (int i = 3; i < count; i++)");
        sw.WriteLine("          SetVO(rows[i]);");
        sw.WriteLine("  }");


        sw.WriteLine("}");
        sw.Close();

    }
}

public class ExcelType
{
    public const string type1 = "int";
    public const string type2 = "float";
    public const string type3 = "string";
    public const string type4 = "table";
    public const string type5 = "bool";
}
