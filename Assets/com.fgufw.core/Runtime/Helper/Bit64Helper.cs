using System;
using System.Collections.Generic;
using System.Text;

namespace FGUFW
{
    public static class Bit64Helper
    {
        /// <summary>
        /// 包含
        /// </summary>
        public static bool Contains(UInt64 source,UInt64 target)
        {
            return (source&target) == target;
        }

        /// <summary>
        /// 添加
        /// </summary>
        public static UInt64 Add(UInt64 source,UInt64 target)
        {
            return source|target;
        }

        /// <summary>
        /// 减去
        /// </summary>
        public static UInt64 Sub(UInt64 source,UInt64 target)
        {
            target = ~target;
            return source|target;
        }

        /// <summary>
        /// 交叉
        /// </summary>
        public static bool Overlap(UInt64 v1,UInt64 v2)
        {
            return (v1&v2) != 0;
        }

    }
}