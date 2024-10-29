using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace FGUFW
{
/*
想做代码文本解析功能 最终返回bool值
数据分:变量(数据源),常量,运算符,优先级(括号)
数据源用于用反射还是整形Id(#123)
常量解析后存储为固定值免得每次调用都Parse字符串
但目前这些符号只能做数值比较
&(且)的优先级高于|(或)
所有匹配符号:()=≠>≥<≤∈∉&|#,
*/
    /// <summary>
    /// 规则匹配 用于配表中的触发判定
    /// </summary>
    public static class TextRulesHelper
    {
        public const char Code_DomainStart = '(';
        public const char Code_DomainEnd = ')';
        //---------------------------------------------
        public const char Code_Equal = '=';
        public const char Code_NotEqual = '≠';
        public const char Code_Greater = '>';
        public const char Code_GreaterAndEqual = '≥';
        public const char Code_Less = '<';
        public const char Code_LessAndEqual = '≤';
        public const char Code_In = '∈';
        public const char Code_Out = '∉';
        //---------------------------------------------
        public const char Code_And = '&';
        public const char Code_Or = '|';
        //---------------------------------------------
        public const char Code_Variate = '#';
        public const char Code_Split = ',';
        //---------------------------------------------
        public const char Code_Multiply = '*';
        public const char Code_Divide = '/';
        //---------------------------------------------
        public const char Code_Add = '+';
        public const char Code_Subtract = '-';

        public static bool IsMatchCode(char code)
        {
            return
            code==Code_DomainStart || 
            code==Code_DomainEnd ||
            code==Code_Greater ||
            code==Code_Less ||
            code==Code_Equal ||
            code==Code_GreaterAndEqual ||
            code==Code_LessAndEqual ||
            code==Code_NotEqual ||
            code==Code_In ||
            code==Code_Out ||
            code==Code_And ||
            code==Code_Or ;
        }

        public static bool IsCodeL1(char code)
        {
            return code==Code_DomainStart || code==Code_DomainEnd;
        }
        
        public static bool IsMatchCodeL2(char code)
        {
            return 
            code==Code_In ||
            code==Code_Out ||
            code==Code_Greater ||
            code==Code_Less ||
            code==Code_Equal ||
            code==Code_GreaterAndEqual ||
            code==Code_LessAndEqual ||
            code==Code_NotEqual ;
        }

        public static bool IsMatchCodeL3(char code)
        {
            return code==Code_And || code==Code_Or;
        }

        public static int FindCodeOR(ReadOnlySpan<char> text,int start)
        {
            int domainCount = 0;
            for (int i = start; i < text.Length; i++)
            {
                var c = text[i];
                if(c==Code_DomainStart) domainCount++;
                else if(c==Code_DomainEnd) domainCount--;
                else if(c==Code_Or && domainCount==0)return i;
            }
            return -1;
        }

        public static int FindCodeAND(ReadOnlySpan<char> text,int start)
        {
            int domainCount = 0;
            for (int i = start; i < text.Length; i++)
            {
                var c = text[i];
                if(c==Code_DomainStart) domainCount++;
                else if(c==Code_DomainEnd) domainCount--;
                else if(c==Code_And && domainCount==0)return i;
            }
            return -1;
        }

        public static int FindMatchCodeL2(ReadOnlySpan<char> text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if(IsMatchCodeL2(text[i]))return i;
            }
            return -1;
        }

        public static bool IsMatchValText(ReadOnlySpan<char> text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if(IsMatchCode(text[i]))return false;
            }
            return true;
        }

        public static bool IsValueValText(ReadOnlySpan<char> text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if(IsValueCode(c))
                {
                    if(c==Code_Subtract && i==0)continue;
                    return false;
                }
            }
            return true;
        }

        public static bool IsMatchL2Text(ReadOnlySpan<char> text)
        {
            int l2_code_count = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if(IsMatchCodeL2(text[i]))l2_code_count++;
            }
            return l2_code_count==1;
        }

        public static bool IsValueCode(char code)
        {
            return
            code==Code_DomainStart || 
            code==Code_DomainEnd ||
            code==Code_Add ||
            code==Code_Subtract ||
            code==Code_Multiply ||
            code==Code_Divide ;
        }

        public static bool IsValueCodeL2_3(char code)
        {
            return
            code==Code_Add ||
            code==Code_Subtract ||
            code==Code_Multiply ||
            code==Code_Divide ;
        }

        public static int FindValueCodeL3(ReadOnlySpan<char> text,int start)
        {
            int domainCount = 0;
            for (int i = start; i < text.Length; i++)
            {
                var c = text[i];
                if(c==Code_DomainStart) domainCount++;
                else if(c==Code_DomainEnd) domainCount--;

                if(domainCount!=0)continue;

                if(c==Code_Add)return i;
                if(c==Code_Subtract && i!=0 && !IsValueCodeL2_3(text[i-1]))return i;
                
            }
            return -1;
        }

        public static int FindValueCodeL2(ReadOnlySpan<char> text,int start)
        {
            int domainCount = 0;
            for (int i = start; i < text.Length; i++)
            {
                var c = text[i];
                if(c==Code_DomainStart) domainCount++;
                else if(c==Code_DomainEnd) domainCount--;

                if(domainCount!=0)continue;

                if(c==Code_Multiply)return i;
                if(c==Code_Divide)return i;
            }
            return -1;
        }


    }

    public interface IVariateOperate
    {
        float VariateNameToKey(string name);

        /// <summary>
        /// 小心数值获取时导致死循环
        /// </summary>
        float GetVariateValue(float key);
        // void SetVariateValue(float key,float val);
    }

    // [Serializable]
    public class TextRulesMatch
    {
        public float[] Value;//为了In,Out逻辑使用了数组 要去掉数组可以考虑把In,Out转成Or,And集合
        public bool[] Variate;
        public List<TextRulesMatch> Children;
        public char Rule;

        private TextRulesMatch(){}

        public TextRulesMatch(IVariateOperate variateGet,ReadOnlySpan<char> text)
        {
            //去掉空字符
            text = text.Trim();

            //去掉首尾括号
            if(text[0]==TextRulesHelper.Code_DomainStart && text[text.Length-1]==TextRulesHelper.Code_DomainEnd)
            {
                text = text.Slice(1,text.Length-2);
            }

            if(TextRulesHelper.IsMatchValText(text))//纯数值
            {
                var items = text.ToString().Split(TextRulesHelper.Code_Split);
                int length = items.Length;
                Value = new float[length];
                Variate = new bool[length];

                for (int i = 0; i < length; i++)
                {
                    var item = items[i];
                    if(item[0]==TextRulesHelper.Code_Variate)//变量
                    {
                        Variate[i] = true;
                        Value[i] = variateGet.VariateNameToKey(item.AsSpan().Slice(1).ToString());
                    }
                    else//常量
                    {
                        Variate[i] = false;
                        Value[i] = item.ToFloat();
                    }
                }
            }
            else if(TextRulesHelper.IsMatchL2Text(text))//比较文本
            {
                Children = new List<TextRulesMatch>(2);
                int idx = TextRulesHelper.FindMatchCodeL2(text);
                Rule = text[idx];
                Children.Add(new TextRulesMatch(variateGet,text.Slice(0,idx)));
                Children.Add(new TextRulesMatch(variateGet,text.Slice(idx+1)));
            }
            else if(TextRulesHelper.FindCodeOR(text,0)!=-1)//或分隔
            {
                Children = new List<TextRulesMatch>();
                Rule = TextRulesHelper.Code_Or;

                int startIdx = 0;
                while (true)
                {
                    int idx = TextRulesHelper.FindCodeOR(text,startIdx);
                    if(idx==-1)
                    {
                        Children.Add(new TextRulesMatch(variateGet,text.Slice(startIdx)));
                        break;
                    }
                    else
                    {
                        
                        Children.Add(new TextRulesMatch(variateGet,text.Slice(startIdx,idx-startIdx)));
                        startIdx = idx+1;
                    }
                }
            }
            else if(TextRulesHelper.FindCodeAND(text,0)!=-1)//且分隔
            {
                Children = new List<TextRulesMatch>();
                Rule = TextRulesHelper.Code_And;

                int startIdx = 0;
                while (true)
                {
                    int idx = TextRulesHelper.FindCodeAND(text,startIdx);
                    if(idx==-1)
                    {
                        Children.Add(new TextRulesMatch(variateGet,text.Slice(startIdx)));
                        break;
                    }
                    else
                    {
                        
                        Children.Add(new TextRulesMatch(variateGet,text.Slice(startIdx,idx-startIdx)));
                        startIdx = idx+1;
                    }
                }
            }
            else
            {
                throw new FormatException($"格式错误:{text.ToString()}");
            }
        }

        public float GetResult(IVariateOperate variateGet)
        {
            float result = 0;
            switch (Rule)
            {
                case TextRulesHelper.Code_Or:
                {
                    foreach (var child in Children)
                    {
                        if(child.GetResult(variateGet)==1)
                        {
                            result = 1;
                            break;
                        }
                    }                    
                }
                break;
                case TextRulesHelper.Code_And:
                {
                    result = 1;
                    foreach (var child in Children)
                    {
                        if(child.GetResult(variateGet)==0)
                        {
                            result = 0;
                            break;
                        }
                    }                    
                }
                break;
                //-----------------------------------------
                case TextRulesHelper.Code_In:
                {
                    result = 0;
                    var left = getValue(variateGet,Children[0].Variate[0],Children[0].Value[0]);
                    
                    int length = Children[1].Value.Length;
                    for (int i = 0; i < length; i++)
                    {
                        var right = getValue(variateGet,Children[1].Variate[i],Children[1].Value[i]);
                        if(left==right)
                        {
                            result=1;
                            break;
                        }
                    }
                }
                break;
                case TextRulesHelper.Code_Out:
                {
                    result = 1;
                    var left = getValue(variateGet,Children[0].Variate[0],Children[0].Value[0]);
                    
                    int length = Children[1].Value.Length;
                    for (int i = 0; i < length; i++)
                    {
                        var right = getValue(variateGet,Children[1].Variate[i],Children[1].Value[i]);
                        if(left==right)
                        {
                            result=0;
                            break;
                        }
                    }
                }
                break;
                //---------------------------------------------
                case TextRulesHelper.Code_Equal:
                {
                    var left = getValue(variateGet,Children[0].Variate[0],Children[0].Value[0]);
                    var right = getValue(variateGet,Children[1].Variate[0],Children[1].Value[0]);
                    result = left==right?1:0;           
                }
                break;
                case TextRulesHelper.Code_NotEqual:
                {
                    var left = getValue(variateGet,Children[0].Variate[0],Children[0].Value[0]);
                    var right = getValue(variateGet,Children[1].Variate[0],Children[1].Value[0]);
                    result = left!=right?1:0;           
                }
                break;
                case TextRulesHelper.Code_Greater:
                {
                    var left = getValue(variateGet,Children[0].Variate[0],Children[0].Value[0]);
                    var right = getValue(variateGet,Children[1].Variate[0],Children[1].Value[0]);
                    result = left>right?1:0;           
                }
                break;
                case TextRulesHelper.Code_GreaterAndEqual:
                {
                    var left = getValue(variateGet,Children[0].Variate[0],Children[0].Value[0]);
                    var right = getValue(variateGet,Children[1].Variate[0],Children[1].Value[0]);
                    result = left>=right?1:0;           
                }
                break;
                case TextRulesHelper.Code_Less:
                {
                    var left = getValue(variateGet,Children[0].Variate[0],Children[0].Value[0]);
                    var right = getValue(variateGet,Children[1].Variate[0],Children[1].Value[0]);
                    result = left<right?1:0;           
                }
                break;
                case TextRulesHelper.Code_LessAndEqual:
                {
                    var left = getValue(variateGet,Children[0].Variate[0],Children[0].Value[0]);
                    var right = getValue(variateGet,Children[1].Variate[0],Children[1].Value[0]);
                    result = left<=right?1:0;           
                }
                break;
            }
            return result;
        }

        private float getValue(IVariateOperate variateGet,bool variate,float val)
        {
            if(variate)
            {
                return variateGet.GetVariateValue(val);
            }
            else
            {
                return val;
            }
        }

        public bool Match(IVariateOperate variateGet)
        {
            return GetResult(variateGet)==1;
        }
    }

    // [Serializable]
    public class TextRuleValueGet
    {
        public List<TextRuleValueGet> Children;
        public List<char> Rule;

        // [SerializeField]
        private float _value;

        // [SerializeField]
        private bool _variate;

        private TextRuleValueGet(){}

        public TextRuleValueGet(IVariateOperate variateGet,ReadOnlySpan<char> text)
        {
            //去掉空字符
            text = text.Trim();

            //去掉首尾括号
            if(text[0]==TextRulesHelper.Code_DomainStart && text[text.Length-1]==TextRulesHelper.Code_DomainEnd)
            {
                text = text.Slice(1,text.Length-2);
            }

            if(TextRulesHelper.IsValueValText(text))//纯数值
            {
                if(text[0]==TextRulesHelper.Code_Variate)//变量
                {
                    _variate = true;
                    _value = variateGet.VariateNameToKey(text.Slice(1).ToString());
                }
                else//常量
                {
                    _variate = false;
                    _value = text.ToFloat();
                }
            }
            else if(TextRulesHelper.FindValueCodeL3(text,0)!=-1)//+-
            {
                Children = new List<TextRuleValueGet>();
                Rule = new List<char>();
                int idx = 0;
                int startIdx = 0;

                while (true)
                {
                    idx = TextRulesHelper.FindValueCodeL3(text,startIdx);
                    if(idx==-1)
                    {
                        Children.Add(new TextRuleValueGet(variateGet,text.Slice(startIdx)));
                        break;
                    }
                    else
                    {
                        Children.Add(new TextRuleValueGet(variateGet,text.Slice(startIdx,idx-startIdx)));
                        Rule.Add(text[idx]);
                        startIdx = idx+1;
                    }
                }
            }
            else if(TextRulesHelper.FindValueCodeL2(text,0)!=-1)//*/
            {
                Children = new List<TextRuleValueGet>();
                Rule = new List<char>();
                int idx = 0;
                int startIdx = 0;

                while (true)
                {
                    idx = TextRulesHelper.FindValueCodeL2(text,startIdx);
                    if(idx==-1)
                    {
                        Children.Add(new TextRuleValueGet(variateGet,text.Slice(startIdx)));
                        break;
                    }
                    else
                    {
                        Children.Add(new TextRuleValueGet(variateGet,text.Slice(startIdx,idx-startIdx)));
                        Rule.Add(text[idx]);
                        startIdx = idx+1;
                    }
                }
            }
            else
            {
                throw new FormatException($"格式错误:{text.ToString()}");
            }

        }

        public float Value(IVariateOperate variateGet)
        {
            float val = 0;
            if(Children==null)//值
            {
                val = getValue(variateGet,_variate,_value);
            }
            else
            {
                val = Children[0].Value(variateGet);
                for (int i = 0; i < Rule.Count; i++)
                {
                    var rule = Rule[i];
                    var childVal = Children[i+1].Value(variateGet);
                    switch (rule)
                    {
                        case TextRulesHelper.Code_Add:
                            val += childVal;
                        break;
                        case TextRulesHelper.Code_Subtract:
                            val -= childVal;
                        break;
                        case TextRulesHelper.Code_Multiply:
                            val *= childVal;
                        break;
                        case TextRulesHelper.Code_Divide:
                            val /= childVal;
                        break;
                    }
                }
            }
            return val;
        }

        private float getValue(IVariateOperate variateGet,bool variate,float val)
        {
            if(variate)
            {
                return variateGet.GetVariateValue(val);
            }
            else
            {
                return val;
            }
        }

    }



}