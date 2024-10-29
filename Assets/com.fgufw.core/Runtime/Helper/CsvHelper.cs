using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
Csv语法
1.矩形表格
2.行以[\r\n]CRLF结尾 以[,]分割
3.格子内可以使用[\n]换行 LF
4.格子含有[,][\n]["]时格子用[""]框起来
5.格子内的["]会替换为[""]
*/
namespace FGUFW
{
    public static class CsvHelper
    {
        private const string TABLE_SPLITE = "\r\n";
        private const char FLAG = '\"';
        private const char LINE_SPLITE = ',';
        private const char LF = '\n';
        private static readonly string[] tableSpite = new string[]{TABLE_SPLITE};

        public static string[][] Parse(string text)
        {
            var lines = text.Split(tableSpite,StringSplitOptions.RemoveEmptyEntries);
            string[][] table = new string[lines.Length][];
            
            var tempLine = new List<string>();
            var lineIndex=0;
            foreach (var line in lines)
            {
                int flagCount=0;
                int lineLength = line.Length;
                int itemIndex = 0;
                for (int i = 0; i < lineLength; i++)
                {
                    var c = line[i];

                    if(c==FLAG)
                    {
                        flagCount++;
                    }
                    else if(c==LINE_SPLITE && flagCount%2==0)
                    {
                        var item = line.Substring(itemIndex,i-itemIndex);
                        itemIndex = i+1;
                        tempLine.Add(UnpackItem(item));
                    }
                    
                    if(i==lineLength-1)
                    {
                        var item = line.Substring(itemIndex,lineLength-itemIndex);
                        tempLine.Add(UnpackItem(item));
                    }
                }
                table[lineIndex++] = tempLine.ToArray();
                tempLine.Clear();
            }

            return table;
        }

        public static string[,] Parse2(string text)
        {
            var lines = text.Split(tableSpite,StringSplitOptions.RemoveEmptyEntries);
            int lnLength = lines.Length;
            int colLength = 0;

            var firstLine = lines[0];
            int lineLength = firstLine.Length;
            int flagCount=0;

            for (int i = 0; i < lineLength; i++)
            {
                var c = firstLine[i];

                if(c==FLAG)
                {
                    flagCount++;
                }
                else if(c==LINE_SPLITE && flagCount%2==0)
                {
                    colLength++;
                }
                
                if(i==lineLength-1)
                {
                    colLength++;
                }
            }


            string[,] table = new string[lnLength,colLength];

            
            for (int lineIndex = 0; lineIndex < lnLength; lineIndex++)
            {
                flagCount=0;
                int itemIndex = 0;
                var line = lines[lineIndex];
                lineLength = line.Length;
                int index = 0;
                for (int i = 0; i < lineLength; i++)
                {
                    var c = line[i];

                    if(c==FLAG)
                    {
                        flagCount++;
                    }
                    else if(c==LINE_SPLITE && flagCount%2==0)
                    {
                        var item = line.Substring(itemIndex,i-itemIndex);
                        itemIndex = i+1;
                        item = UnpackItem(item);
                        table[lineIndex,index++] = item;
                    }
                    
                    if(i==lineLength-1)
                    {
                        var item = line.Substring(itemIndex,lineLength-itemIndex);
                        item = UnpackItem(item);
                        table[lineIndex,index++] = item;
                    }
                }
            }

            return table;
        }

        /// <summary>
        /// 解析表格数据
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string UnpackItem(string item)
        {
            item = item.Replace($"{FLAG}{FLAG}",$"{FLAG}");
            if(item.IndexOf(FLAG)==0 && item.LastIndexOf(FLAG)==item.Length-1)
            {
                item = item.Substring(1,item.Length-2);
            }
            return item;
        }

        /// <summary>
        /// 打包成存储格式
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string PackItem(string item)
        {
            item = item.Replace(TABLE_SPLITE,$"{LF}");
            item = item.Replace($"{FLAG}",$"{FLAG}{FLAG}");
            if(item.Contains(LF.ToString()) || item.Contains(FLAG.ToString()) || item.Contains(LINE_SPLITE.ToString()))
            {
                item = $"\"{item}\"";
            }
            return item;
        }

        public static string ToCsvText(string[][] table)
        {
            string[] lines = new string[table.Length];
            var tempLine = new List<string>();
            for(int lineIndex=0;lineIndex<table.Length;lineIndex++)
            {
                var line = table[lineIndex];
                int length = line.Length;

                for (int i = 0; i < length; i++)
                {
                    tempLine.Add(PackItem(line[i]));
                }

                lines[lineIndex] = string.Join(LINE_SPLITE.ToString(),tempLine);
                tempLine.Clear();
            }
            return string.Join(TABLE_SPLITE,lines);
        }

        public static string ToCsvText2(string[,] table)
        {
            string[] lines = new string[table.Length];
            var tempLine = new string[table.GetLength(1)];
            for(int lineIndex=0;lineIndex<table.Length;lineIndex++)
            {
                for (int i = 0; i < tempLine.Length; i++)
                {
                    tempLine[i] = PackItem(table[lineIndex,i]);
                }

                lines[lineIndex] = string.Join(LINE_SPLITE.ToString(),tempLine);
            }
            return string.Join(TABLE_SPLITE,lines);
        }


    }

    public sealed class CsvTable
    {
        private string[][] _table;

        private CsvTable(){}

        public CsvTable(string[][] table)
        {
            _table = table;
        }

        public string this[int ln,int col]
        {
            get
            {
                return _table[ln][col];
            }
            set
            {
                _table[ln][col] = value;
            }
        }

        public string[] this[int ln]
        {
            get
            {
                return _table[ln];
            }
            set
            {
                _table[ln] = value;
            }
        }

        public string[] this[string key]
        {
            get
            {
                int index = lineIndex(key);
                if(index!=-1)
                {
                    return _table[index];
                }
                return null;
            }
            set
            {
                int index = lineIndex(key);
                if(index!=-1)
                {
                    _table[index] = value;
                }
            }
        }

        private int lineIndex(string key)
        {
            int length = _table.Length;
            int index = -1;
            for (int i = 0; i < length; i++)
            {
                if(_table[i][0]==key)
                {
                    if(index!=-1)
                    {
                        throw new ArgumentException($"表格存在多个Key:{key}");
                    }
                    else
                    {
                        index = i;
                    }
                }
            }

            if(index==-1) throw new KeyNotFoundException($"表格不存在Key:{key}");

            return index;
        }

        



    }

}