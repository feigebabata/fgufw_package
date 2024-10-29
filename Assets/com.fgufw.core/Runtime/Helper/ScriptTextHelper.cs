using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FGUFW
{
    public static class ScriptTextHelper
    {
/*
解析规则:
    数组以'\n'分割
    向量以','分割
*/
        public const string NAME_SPACE = "|NAME_SPACE|";
        public const string CLASS_NAME = "|CLASS_NAME|";
        public const string MEMBERS = "|MEMBERS|";
        public const string MEMBER_SETS = "|MEMBER_SETS|";
        public const string CLASS_SUMMARY = "|CLASS_SUMMARY|";
        public const string FRIST_TYPE = "|FRIST_TYPE|";
        public const string FRIST_MEMBER = "|FRIST_MEMBER|";

        /// <summary>
        /// 第一行:类全名,class,类备注
        /// 第二行:字段类型 
        /// 第三行:字段名 
        /// 第四行:字段备注
        /// </summary>
        /// <param name="table"></param>
        /// <param name="typeLine"></param>
        /// <param name="nameLine"></param>
        /// <param name="summaryLine"></param>
        /// <returns></returns>
        public static string Csv2CsharpClass(string[,] table,int typeLine=1,int nameLine=2,int summaryLine=3)
        {
            string script = 
@"using FGUFW;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace |NAME_SPACE|
{
    /// <summary>
    /// |CLASS_SUMMARY|
    /// </summary>
    [Serializable]
    public class |CLASS_NAME|
    {
|MEMBERS|

        public |CLASS_NAME|(string[,] table,int lineIndex)
        {
|MEMBER_SETS|
        }

        public static |CLASS_NAME|[] ToArray(string csvText)
        {
            var table = CsvHelper.Parse2(csvText);
            int length = table.GetLength(0)-4;
            |CLASS_NAME|[] list = new |CLASS_NAME|[length];
            for (int i = 0; i < length; i++)
            {
                list[i] = new |CLASS_NAME|(table,i+4);
            }
            return list;
        }

        public static Dictionary<|FRIST_TYPE|,|CLASS_NAME|> ToDict(string csvText)
        {
            var table = CsvHelper.Parse2(csvText);
            int length = table.GetLength(0);
            Dictionary<|FRIST_TYPE|,|CLASS_NAME|> dict = new Dictionary<|FRIST_TYPE|,|CLASS_NAME|>();
            for (int i = 4; i < length; i++)
            {
                var data = new |CLASS_NAME|(table,i);
                dict.Add(data.|FRIST_MEMBER|,data);
            }
            return dict;
        }

    }
}
";


            string globalScript = 
@"using FGUFW;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// |CLASS_SUMMARY|
/// </summary>
[Serializable]
public class |CLASS_NAME|
{
|MEMBERS|

    public |CLASS_NAME|(string[,] table,int lineIndex)
    {
|MEMBER_SETS|
    }

    public static |CLASS_NAME|[] ToArray(string csvText)
    {
        var table = CsvHelper.Parse2(csvText);
        int length = table.GetLength(0)-4;
        |CLASS_NAME|[] list = new |CLASS_NAME|[length];
        for (int i = 0; i < length; i++)
        {
            list[i] = new |CLASS_NAME|(table,i+4);
        }
        return list;
    }

    public static Dictionary<|FRIST_TYPE|,|CLASS_NAME|> ToDict(string csvText)
    {
        var table = CsvHelper.Parse2(csvText);
        int length = table.GetLength(0);
        Dictionary<|FRIST_TYPE|,|CLASS_NAME|> dict = new Dictionary<|FRIST_TYPE|,|CLASS_NAME|>();
        for (int i = 4; i < length; i++)
        {
            var data = new |CLASS_NAME|(table,i);
            dict.Add(data.|FRIST_MEMBER|,data);
        }
        return dict;
    }

}

";
            var frist_type = table[typeLine,0];
            var frist_name = table[nameLine,0];
            var nameSpaceArr = table[0,0].Split('.');
            var nameSpace = string.Join(".",nameSpaceArr,0,nameSpaceArr.Length-1);

            if(string.IsNullOrEmpty(nameSpace))
            {
                script = globalScript;
            }
            var className = nameSpaceArr[nameSpaceArr.Length-1];
            var classSummary = table[0,2];
            var members = CSVMembers(table,typeLine,nameLine,summaryLine);
            var memberSets = CSVMemberSets(table,typeLine,nameLine);

            script = script.Replace(NAME_SPACE,nameSpace);
            script = script.Replace(CLASS_NAME,className);
            script = script.Replace(CLASS_SUMMARY,classSummary);
            script = script.Replace(FRIST_TYPE,frist_type);
            script = script.Replace(FRIST_MEMBER,frist_name);
            script = script.Replace(MEMBERS,members);
            script = script.Replace(MEMBER_SETS,memberSets);

            return script;
        }
        public static string Csv2CsharpStruct(string[,] table,int typeLine=1,int nameLine=2,int summaryLine=3)
        {
            string script = 
@"using FGUFW;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace |NAME_SPACE|
{
    /// <summary>
    /// |CLASS_SUMMARY|
    /// </summary>
    [Serializable]
    public struct |CLASS_NAME|
    {
|MEMBERS|

        public |CLASS_NAME|(string[,] table,int lineIndex)
        {
|MEMBER_SETS|
        }

        public static |CLASS_NAME|[] ToArray(string csvText)
        {
            var table = CsvHelper.Parse2(csvText);
            int length = table.GetLength(0)-4;
            |CLASS_NAME|[] list = new |CLASS_NAME|[length];
            for (int i = 0; i < length; i++)
            {
                list[i] = new |CLASS_NAME|(table,i+4);
            }
            return list;
        }

        public static Dictionary<|FRIST_TYPE|,|CLASS_NAME|> ToDict(string csvText)
        {
            var table = CsvHelper.Parse2(csvText);
            int length = table.GetLength(0);
            Dictionary<|FRIST_TYPE|,|CLASS_NAME|> dict = new Dictionary<|FRIST_TYPE|,|CLASS_NAME|>();
            for (int i = 4; i < length; i++)
            {
                var data = new |CLASS_NAME|(table,i);
                dict.Add(data.|FRIST_MEMBER|,data);
            }
            return dict;
        }

    }
}
";


            string globalScript = 
@"using FGUFW;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// |CLASS_SUMMARY|
/// </summary>
[Serializable]
public struct |CLASS_NAME|
{
|MEMBERS|

    public |CLASS_NAME|(string[,] table,int lineIndex)
    {
|MEMBER_SETS|
    }

    public static |CLASS_NAME|[] ToArray(string csvText)
    {
        var table = CsvHelper.Parse2(csvText);
        int length = table.GetLength(0)-4;
        |CLASS_NAME|[] list = new |CLASS_NAME|[length];
        for (int i = 0; i < length; i++)
        {
            list[i] = new |CLASS_NAME|(table,i+4);
        }
        return list;
    }

    public static Dictionary<|FRIST_TYPE|,|CLASS_NAME|> ToDict(string csvText)
    {
        var table = CsvHelper.Parse2(csvText);
        int length = table.GetLength(0);
        Dictionary<|FRIST_TYPE|,|CLASS_NAME|> dict = new Dictionary<|FRIST_TYPE|,|CLASS_NAME|>();
        for (int i = 4; i < length; i++)
        {
            var data = new |CLASS_NAME|(table,i);
            dict.Add(data.|FRIST_MEMBER|,data);
        }
        return dict;
    }

}

";

            var frist_type = table[typeLine,0];
            var frist_name = table[nameLine,0];
            var nameSpaceArr = table[0,0].Split('.');
            var nameSpace = string.Join(".",nameSpaceArr,0,nameSpaceArr.Length-1);

            if(string.IsNullOrEmpty(nameSpace))
            {
                script = globalScript;
            }
            var className = nameSpaceArr[nameSpaceArr.Length-1];
            var classSummary = table[0,2];
            var members = CSVMembers(table,typeLine,nameLine,summaryLine);
            var memberSets = CSVMemberSets(table,typeLine,nameLine);

            script = script.Replace(NAME_SPACE,nameSpace);
            script = script.Replace(CLASS_NAME,className);
            script = script.Replace(CLASS_SUMMARY,classSummary);
            script = script.Replace(FRIST_TYPE,frist_type);
            script = script.Replace(FRIST_MEMBER,frist_name);
            script = script.Replace(MEMBERS,members);
            script = script.Replace(MEMBER_SETS,memberSets);

            return script;
        }

        /// <summary>
        /// 第一行:枚举全名,enmu,枚举备注 
        /// 第二行:列注释(枚举名,枚举值,备注)
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string Csv2CsharpEnum(string[,] table)
        {
            string script = 
@"namespace |NAME_SPACE|
{
    /// <summary>
    /// |CLASS_SUMMARY|
    /// </summary>
    public enum |CLASS_NAME|
    {
|MEMBERS|
    }
}
";


            string globalScript = 
@"
/// <summary>
/// |CLASS_SUMMARY|
/// </summary>
public enum |CLASS_NAME|
{
|MEMBERS|
}

";
            StringBuilder sb = new StringBuilder();
            int length = table.GetLength(0);
            for (int i = 2; i < length; i++)
            {
                sb.AppendLine(
@$"        /// <summary>
        /// {table[i,2]}
        /// </summary>
        {table[i,0]} = {table[i,1]},
");
            }

            var nameSpaceArr = table[0,0].Split('.');
            var nameSpace = string.Join(".",nameSpaceArr,0,nameSpaceArr.Length-1);
            if(string.IsNullOrEmpty(nameSpace))
            {
                script = globalScript;
            }
            var className = nameSpaceArr[nameSpaceArr.Length-1];
            var classSummary = table[0,2];
            script = script.Replace(MEMBERS,sb.ToString());
            script = script.Replace(NAME_SPACE,nameSpace);
            script = script.Replace(CLASS_NAME,className);
            script = script.Replace(CLASS_SUMMARY,classSummary);
            return script;
        }

        public static string CSVMembers(string[,] table,int typeLine=1,int nameLine=2,int summaryLine=3)
        {
            StringBuilder sb = new StringBuilder();
            int length = 0;
            for (int i = 0; i < table.GetLength(typeLine); i++)
            {
                if(!string.IsNullOrEmpty(table[typeLine,i]))length++;
            }

            for (int i = 0; i < length; i++)
            {
                sb.AppendLine(@$"
        /// <summary>
        /// {table[summaryLine,i]}
        /// </summary>
        public {table[typeLine,i]} {table[nameLine,i]};
");
            }
            return sb.ToString();
        }

        public static string CSVMemberSets(string[,] table,int typeLine=1,int nameLine=2)
        {
            StringBuilder sb = new StringBuilder();
            int length = 0;
            for (int i = 0; i < table.GetLength(typeLine); i++)
            {
                if(!string.IsNullOrEmpty(table[typeLine,i]))length++;
            }
            for (int i = 0; i < length; i++)
            {
                var memberType = table[typeLine,i];
                var memberName = table[nameLine,i];
                string memberSet = null;
                if(memberType.IndexOf("[]")==-1)
                {
                    switch (memberType)
                    {
                        case "int":
                        case "float":
                        case "bool":
                        case "string":
                        case "Vector2":
                        case "Vector3":
                        case "Color":
                            memberSet = $"            {memberName}=ScriptTextHelper.Parse_{memberType}(table[lineIndex,{i}]);";
                        break;
                        default:
                            memberSet = $"            {memberName}=ScriptTextHelper.Parse_enum<{memberType}>(table[lineIndex,{i}]);";
                        break;
                    }
                }
                else
                {
                    memberType = memberType.Replace("[]","");
                    switch (memberType)
                    {
                        case "int":
                        case "float":
                        case "bool":
                        case "string":
                        case "Vector2":
                        case "Vector3":
                        case "Color":
                            memberSet = $"            {memberName}=ScriptTextHelper.Parse_{memberType}s(table[lineIndex,{i}]);";
                        break;
                        default:
                            memberSet = $"            {memberName}=ScriptTextHelper.Parse_enums<{memberType}>(table[lineIndex,{i}]);";
                        break;
                    }
                }
                
                sb.AppendLine(memberSet);
            }
            return sb.ToString();
        }

        public static int Parse_int(string memberText)
        {
            return string.IsNullOrEmpty(memberText)?0:int.Parse(memberText);
        }

        public static float Parse_float(string memberText)
        {
            return string.IsNullOrEmpty(memberText)?0:float.Parse(memberText);
        }

        public static bool Parse_bool(string memberText)
        {
            return string.IsNullOrEmpty(memberText)?false:int.Parse(memberText)==1;
        }

        public static string Parse_string(string memberText)
        {
            return memberText;
        }

        public static T Parse_enum<T>(string memberText) where T:Enum
        {
            if(string.IsNullOrEmpty(memberText))
            {
                return default(T);
            }
            else
            {
                return memberText.ToEnum<T>();
            }
        }

        public static Vector2 Parse_Vector2(string memberText)
        {
            Vector2 val = Vector2.zero;
            if(string.IsNullOrEmpty(memberText))
            {
                return val;
            }
            return VectorHelper.Parse(memberText);
        }

        public static Vector3 Parse_Vector3(string memberText)
        {
            Vector3 val = Vector3.zero;
            if(string.IsNullOrEmpty(memberText))
            {
                return val;
            }
            return VectorHelper.Parse(memberText);
        }

        public static Color Parse_Color(string memberText)
        {
            if(string.IsNullOrEmpty(memberText))
            {
                return Color.white;
            }
            return memberText.ToColor();
        }

        //------------------------------------------------------------------------

        public static int[] Parse_ints(string memberText)
        {
            if(string.IsNullOrEmpty(memberText))
            {
                return null;
            }
            else
            {
                var arr = memberText.Split('\n');
                int length = arr.Length;
                var vals = new int[length];
                for (int i = 0; i < length; i++)
                {
                    vals[i] = int.Parse(arr[i]);
                }
                return vals;
            }
        }

        public static float[] Parse_floats(string memberText)
        {
            if(string.IsNullOrEmpty(memberText))
            {
                return null;
            }
            else
            {
                var arr = memberText.Split('\n');
                int length = arr.Length;
                var vals = new float[length];
                for (int i = 0; i < length; i++)
                {
                    vals[i] = float.Parse(arr[i]);
                }
                return vals;
            }
        }

        public static bool[] Parse_bools(string memberText)
        {
            
            if(string.IsNullOrEmpty(memberText))
            {
                return null;
            }
            else
            {
                var arr = memberText.Split('\n');
                int length = arr.Length;
                var vals = new bool[length];
                for (int i = 0; i < length; i++)
                {
                    vals[i] = int.Parse(arr[i])==1;
                }
                return vals;
            }
        }

        public static string[] Parse_strings(string memberText)
        {
            
            if(string.IsNullOrEmpty(memberText))
            {
                return null;
            }
            else
            {
                return memberText.Split('\n');
            }
        }

        public static T[] Parse_enums<T>(string memberText) where T:Enum
        {
            if(string.IsNullOrEmpty(memberText))
            {
                return null;
            }
            else
            {
                var arr = memberText.Split('\n');
                int length = arr.Length;
                var vals = new T[length];
                for (int i = 0; i < length; i++)
                {
                    vals[i] = arr[i].ToEnum<T>();
                }
                return vals;
            }
        }

        public static Vector2[] Parse_Vector2s(string memberText)
        {
            if(string.IsNullOrEmpty(memberText))
            {
                return null;
            }
            else
            {
                var arr = memberText.Split('\n');
                int length = arr.Length;
                var vals = new Vector2[length];
                for (int i = 0; i < length; i++)
                {
                    vals[i] = VectorHelper.Parse(arr[i]);
                }
                return vals;
            }
        }

        public static Vector3[] Parse_Vector3s(string memberText)
        {
            if(string.IsNullOrEmpty(memberText))
            {
                return null;
            }
            else
            {
                var arr = memberText.Split('\n');
                int length = arr.Length;
                var vals = new Vector3[length];
                for (int i = 0; i < length; i++)
                {
                    vals[i] = VectorHelper.Parse(arr[i]);
                }
                return vals;
            }
        }

        public static Color[] Parse_Colors(string memberText)
        {
            if(string.IsNullOrEmpty(memberText))
            {
                return null;
            }
            else
            {
                var arr = memberText.Split('\n');
                int length = arr.Length;
                var vals = new Color[length];
                for (int i = 0; i < length; i++)
                {
                    vals[i] = Parse_Color(arr[i]);
                }
                return vals;
            }
        }


    }
}
