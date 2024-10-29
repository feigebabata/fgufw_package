using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FGUFW
{
    public static class NovelTextHelper
    {
        public readonly static Encoding GB2312 = Encoding.GetEncoding("gb2312");
        private const string CHAPER_END = "章卷篇节回话部集季";
        private const int CHAPER_MAX_LENGTH = 25;
        private const string LINE_END = ";；。!！」…”\"》）\\-※";
        private const string NUMBER = "0123456789零〇一二三四五六七八九十百千ⅠⅡⅢⅣⅤⅥⅦⅧⅨⅩⅪⅫⅬⅭⅮⅯⅰⅱⅲⅳⅴⅵⅶⅷⅸⅹⅺⅻⅼⅽⅾⅿ";
        private static Regex regexChapter = new Regex($"第[{NUMBER}]+?[{CHAPER_END}]{{1}}");
        private static Regex regexLineEnd = new Regex($"[{LINE_END}]$");
        private static Regex regexChapter2_1 = new Regex($@"\([{NUMBER}]+\)$");
        private static Regex regexChapter2_2 = new Regex($@"\([{NUMBER}]+-[{NUMBER}]+\)$");
        private static Regex regexChapter3_1 = new Regex($"（[{NUMBER}]+）$");
        private static Regex regexChapter3_2 = new Regex($"（[{NUMBER}]+-[{NUMBER}]+）$");

        /// <summary>
        /// 章节分割 v2.x:起点 v2.y:长度
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static Vector2Int[] SplitChapter(string[] lines)
        {
            List<Vector2Int> indexs = new List<Vector2Int>(lines.Length/100);
            for (int i = 0; i < lines.Length; i++)
            {
                if(LineIsChapter(lines[i]))
                {
                    var last = indexs[indexs.Count-1];
                    last.y = i-last.x;
                    indexs[indexs.Count-1] = last;

                    indexs.Add(new Vector2Int(i,0));
                }
                else if(i==0)
                {
                    indexs.Add(new Vector2Int(i,0));
                }
                else if(i==lines.Length-1)
                {
                    var last = indexs[indexs.Count-1];
                    last.y = i-last.x;
                    indexs[indexs.Count-1] = last;
                    
                }
            }
            return indexs.ToArray();
        }

        /// <summary>
        /// 格式化行数据: 
        /// 行首4空格缩进 
        /// 行之间添加空行
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static string[] FormatLines(string[] lines)
        {
            List<string> ls = new List<string>(lines.Length);
            foreach (var line in lines)
            {
                if(string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line))continue;
                
                if(LineIsChapter(line))
                {
                    ls.Add($"{line.Trim()}");
                }
                else
                {
                    ls.Add($"    {line.Trim()}");
                }
                ls.Add(string.Empty);
            }
            return ls.ToArray();
        }

        public static string GetChapter(string[] lines,Vector2Int chapterIndex)
        {
            return string.Join("\n",lines,chapterIndex.x,chapterIndex.y);
        }

        /// <summary>
        /// 是否为章节名
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool LineIsChapter(string line)
        {
            if(line.Length>CHAPER_MAX_LENGTH)return false;
            return regexChapter.IsMatch(line) 
            || regexChapter2_1.IsMatch(line)
            || regexChapter2_2.IsMatch(line)
            || regexChapter3_1.IsMatch(line)
            || regexChapter3_2.IsMatch(line);
        }

        /// <summary>
        /// 是否为正常结尾
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool LineIsEnd(string line)
        {
            return regexLineEnd.IsMatch(line);
        }
        


    }
}