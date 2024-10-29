using UnityEngine;
using System;

namespace FGUFW
{
    public static class BoundsExtensions
    {
        public static ValueTuple<bool,Bounds> Overlap(this Bounds self, Bounds other)
        {
            Vector3 min_1 = self.min;
            Vector3 size_1 = self.size;
            var l_x_1 = new Vector2(min_1.x,size_1.x);
            var l_y_1 = new Vector2(min_1.y,size_1.y);
            var l_z_1 = new Vector2(min_1.z,size_1.z);

            Vector3 min_2 = self.min;
            Vector3 size_2 = self.size;
            var l_x_2 = new Vector2(min_2.x,size_2.x);
            var l_y_2 = new Vector2(min_2.y,size_2.y);
            var l_z_2 = new Vector2(min_2.z,size_2.z);

            var (overlapX,l_x_3) = MathHelper.LineOverlap(l_x_1,l_x_2);
            var (overlapY,l_y_3) = MathHelper.LineOverlap(l_y_1,l_y_2);
            var (overlapZ,l_z_3) = MathHelper.LineOverlap(l_z_1,l_z_2);

            if(overlapX && overlapY && overlapZ)
            {
                var size = new Vector3(l_x_3.y , l_y_3.y , l_z_3.y);
                var min = new Vector3(l_x_3.x , l_y_3.x , l_z_3.x);
                var bounds = new Bounds(min+size*0.5f,size);
                return (true,bounds);
            }
            return (false,default(Bounds));
        }

        public static void ForeachGrid(this Bounds self,Vector3Int gridCount,Vector3 pivot,Action<Vector3Int,int,Vector3> callback)
        {
            GeometryHelper.SpaceForeachGrid(self.min,self.max,gridCount,pivot,callback);
        }
    }
}