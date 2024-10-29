using System;
using System.Collections.Generic;
using System.Text;
using Unity.Mathematics;

namespace FGUFW
{
    public static class BitHelper
    {
        /// <summary>
        /// 包含
        /// </summary>
        public static bool Contains(UInt32 source,UInt32 target)
        {
            return (source&target) == target;
        }

        /// <summary>
        /// 添加
        /// </summary>
        public static UInt32 Add(UInt32 source,UInt32 target)
        {
            return source|target;
        }

        /// <summary>
        /// 减去
        /// </summary>
        public static UInt32 Sub(UInt32 source,UInt32 target)
        {
            target = ~target;
            return source|target;
        }

        /// <summary>
        /// 交叉
        /// </summary>
        public static bool Overlap(UInt32 v1,UInt32 v2)
        {
            return (v1&v2) != 0;
        }
        
        /// <summary>
        /// 包含
        /// </summary>
        public static bool Contains(Int32 source,Int32 target)
        {
            return (source&target) == target;
        }

        /// <summary>
        /// 添加
        /// </summary>
        public static Int32 Add(Int32 source,Int32 target)
        {
            return source|target;
        }

        /// <summary>
        /// 减去
        /// </summary>
        public static Int32 Sub(Int32 source,Int32 target)
        {
            target = ~target;
            return source|target;
        }

        /// <summary>
        /// 交叉
        /// </summary>
        public static bool Overlap(Int32 v1,Int32 v2)
        {
            return (v1&v2) != 0;
        }

        /// <summary>
        /// 正数最高位的索引
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int BitIndex(Int32 val)
        {
            if(val<=0)return -1;
            return (int)math.log2(val);
        }


    }
}