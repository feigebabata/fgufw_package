using System.Text.RegularExpressions;

namespace FGUFW
{
    public static class RegexHelper
    {
        /// <summary>
        /// 中文字符
        /// </summary>
        public const string CHINESE_RANGE = @"[\u4e00-\u9fa5]";

        /// <summary>
        /// 非空结尾
        /// </summary>
        public const string END_NO_EMPTY = @"\S$";
    }
}