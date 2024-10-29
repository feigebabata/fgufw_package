using UnityEngine;

namespace FGUFW
{
    /// <summary>
    /// 层级碰撞矩阵
    /// </summary>
    public static class CollisionMatrix
    {
        private static bool[] layerIgnore;

        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            if(layerIgnore==null)layerIgnore = new bool[528];
            int index = 0;
            for (int line = 0; line<32; line++)
            {
                for (int i = 31; i >= line; i--)
                {
                    layerIgnore[index] = Physics.GetIgnoreLayerCollision(line,i);
                    index++;
                }
            }
        }

        /// <summary>
        /// 层与层是否能作用 基于unity物理设置3D
        /// </summary>
        /// <param name="layer1"></param>
        /// <param name="layer2"></param>
        /// <returns></returns>
        public static bool GetIgnoreLayerCollision(int layer1,int layer2)
        {
            if(layerIgnore==null)Init();
            int indxe = 0;
            for (int i = 0,line=32; i < layer1; i++,line--)
            {
                indxe+=line;
            }
            indxe+=layer2;
            return layerIgnore[indxe];
        }
    }
}