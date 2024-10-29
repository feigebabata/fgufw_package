using UnityEngine;
using System;
using Unity.Mathematics;

namespace FGUFW
{
    /// <summary>
    /// 几何助手
    /// </summary>
    public static class GeometryHelper
    {
        /// <summary>
        /// 多边形转三角形 顺时针 法线向内 
        /// n个顶点的多边形可以切成n-2个三角形
        /// </summary>
        public unsafe static int[] Polygon2Triangles(Vector3[] vertices,Vector3 normal,ref bool error,int[] triangles=null)
        {
            if(vertices==null || vertices.Length<3)return null;

            int triangleLength = (vertices.Length-2)*3;
            if(triangles==null || triangles.Length!=triangleLength)triangles=new int[triangleLength];
            int triangleIndex = 0;
            var polygonLength = vertices.Length;
            var polygonPointer = stackalloc Vector3[polygonLength];
            var polygonIndex = stackalloc int[polygonLength];
            var polygon = new StackPolygon(polygonPointer,polygonIndex,vertices,normal.normalized);
            error = false;
            while (!polygon.ConvexPolygon())
            {
                error = true;
                for (int i = 0; i < polygon.Length; i++)
                {
                    //切耳朵
                    if(polygon.CanCutConcaveAngle(i))
                    {
                        polygon.CutConvexAngle(i,triangles,ref triangleIndex);
                        error = false;
                        break;
                    }
                }
                if(error)break;
            }
            polygon.SqlitConvex(triangles,ref triangleIndex);
            
            return triangles;
        }


        private unsafe struct StackPolygon
        {
            private Vector3* _vertices;
            private int* _verticeIndexs;
            private Vector3 _normal;
            public int Length{get;private set;}
            public StackPolygon(Vector3* vertices,int* verticeIndexs,Vector3[] polygon,Vector3 normal)
            {
                _vertices = vertices;
                _verticeIndexs = verticeIndexs;
                Length = polygon.Length;
                for (int i = 0; i < Length; i++)
                {
                    vertices[i] = polygon[i];
                    verticeIndexs[i] = i;
                }
                _normal = normal;
            }

            /// <summary>
            /// 凸多边形
            /// </summary>
            /// <returns></returns>
            public bool ConvexPolygon()
            {
                for (int i = 0; i < Length; i++)
                {
                    if(IsConcaveAngle(i))
                    {
                        return false;
                    }
                }
                return true;
            }
            
            /// <summary>
            /// 凹顶点
            /// </summary>
            public bool IsConcaveAngle(int vertexIndex)
            {
                var (v1,v2) = getVector(vertexIndex);

                // 计算该内角的外向法线
                Vector3 normal = Vector3.Cross(v1, v2).normalized;

                var val = Vector3.Dot(normal, _normal);
                return val < 0;
            }

            /// <summary>
            /// 能否切去耳朵
            /// </summary>
            /// <param name="vertexIndex"></param>
            /// <returns></returns>
            public bool CanCutConcaveAngle(int vertexIndex)
            {
                if(!IsConcaveAngle(vertexIndex))return false;
                int l_idx = (vertexIndex-1).RoundIndex(Length);
                int r_idx = (vertexIndex+1).RoundIndex(Length);
                return !IsConcaveAngle(l_idx) || !IsConcaveAngle(r_idx);
            }

            ValueTuple<Vector3,Vector3> getVector(int vertexIndex)
            {
                // 计算该内角对应的两个相邻顶点的向量
                Vector3 v1 = _vertices[(vertexIndex + Length - 1) % Length] - _vertices[vertexIndex];
                Vector3 v2 = _vertices[(vertexIndex + 1) % Length] - _vertices[vertexIndex];
                return (v1,v2);
            }

            /// <summary>
            /// 切去凹角旁边的凸角
            /// </summary>
            public void CutConvexAngle(int vertexIndex,int[] triangles,ref int triangleIndex)
            {
                int l_idx = (vertexIndex-1).RoundIndex(Length);
                int r_idx = (vertexIndex+1).RoundIndex(Length);
                if(!IsConcaveAngle(l_idx))
                {
                    vertexIndex-=2;
                    triangles[triangleIndex++] = _verticeIndexs[(vertexIndex++).RoundIndex(Length)];
                    triangles[triangleIndex++] = _verticeIndexs[(vertexIndex++).RoundIndex(Length)];
                    triangles[triangleIndex++] = _verticeIndexs[(vertexIndex++).RoundIndex(Length)];
                    RemoveVertex(l_idx);
                }
                else if(!IsConcaveAngle(r_idx))
                {
                    triangles[triangleIndex++] = _verticeIndexs[(vertexIndex--).RoundIndex(Length)];
                    triangles[triangleIndex++] = _verticeIndexs[(vertexIndex--).RoundIndex(Length)];
                    triangles[triangleIndex++] = _verticeIndexs[(vertexIndex--).RoundIndex(Length)];
                    RemoveVertex(r_idx);
                }
            }

            public void RemoveVertex(int vertexIndex)
            {
                if(vertexIndex>=Length)return;
                Length--;
                for (int i = vertexIndex; i < Length; i++)
                {
                    _vertices[i]=_vertices[i+1];
                    _verticeIndexs[i]=_verticeIndexs[i+1];
                }
            }

            /// <summary>
            /// 切割凸多边形
            /// </summary>
            public void SqlitConvex(int[] triangles,ref int triangleIndex)
            {
                if(Length<3)return;
                for (int i = 1; i < Length-1; i++)
                {
                    triangles[triangleIndex++] = _verticeIndexs[0];
                    triangles[triangleIndex++] = _verticeIndexs[(i).RoundIndex(Length)];
                    triangles[triangleIndex++] = _verticeIndexs[(i+1).RoundIndex(Length)];
                }
            }

            // static Vector3 centerPoint(Vector3* vertices,int length)
            // {
            //     // 计算多边形的中心点
            //     Vector3 center = Vector3.zero;
            //     for (int i = 0; i < length; i++)
            //     {
            //         center += vertices[i];
            //     }
            //     center /= length;
            //     return center;
            // }
        }

        

        /// <summary>
        /// 点在三角形内
        /// </summary>
        /// <param name="point"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool InTriangle(Vector3 point,Vector3 a,Vector3 b,Vector3 c)
        {
            Vector3 pa = a - point;
            Vector3 pb = b - point;
            Vector3 pc = c - point;
            Vector3 pab = Vector3.Cross(pa,pb);
            Vector3 pbc = Vector3.Cross(pb, pc);
            Vector3 pca = Vector3.Cross(pc, pa);
            
            float d1 = Vector3.Dot(pab, pbc);
            float d2 = Vector3.Dot(pab, pca);
            float d3 = Vector3.Dot(pbc, pca);
 
            if (d1 > 0 && d2 > 0 && d3 > 0) return true;
            return false;
        }

        /// <summary>
        /// 点是否在椭圆内 水平方向 无旋转
        /// </summary>
        /// <param name="point"></param>
        /// <param name="center">中心点</param>
        /// <param name="width">椭圆宽</param>
        /// <param name="height">椭圆高</param>
        /// <returns></returns>
        public static bool PointInEllipse(float3 point,float3 center,float width,float height)
        {
            float a = width/2;
            float b = height/2;

            float X = center.x;
            float Y = center.y;
            float x = point.x;
            float y = point.y;

            float cc = ((x-X)*(x-X)) / (a*a) + ((y-Y)*(y-Y)) / (b*b);

            return cc<=1f;
        }
        

        /// <summary>
        /// 椭圆形2D
        /// </summary>
        /// <param name="center"></param>
        /// <param name="width">长半轴</param>
        /// <param name="height">短半轴</param>
        /// <param name="rotation"></param>
        /// <param name="pointCount"></param>
        /// <returns></returns>
        public static Vector2[] Ellipse(Vector2 center, float width, float height, float rotation,int pointCount)
        {
            Vector2[] points = new Vector2[pointCount];
            for (float i = 0; i < pointCount; i++)
            {
                float rate = i/pointCount;
                float angle = 2*Mathf.PI*rate;
                points[(int)i] = getPointOnEllipse(center,width,height,angle,rotation);
            }
            return points;
        }

        /// <summary>
        /// 椭圆边上的点
        /// </summary>
        /// <param name="center"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="angle"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        static private Vector2 getPointOnEllipse(Vector2 center, float width, float height, float angle, float rotation)
        {
            float dLiXin = Mathf.Atan2(width*Mathf.Sin(angle), height*Mathf.Cos(angle));//离心角
            float x = width*Mathf.Cos(dLiXin)*Mathf.Cos(rotation) - height*Mathf.Sin(dLiXin)*Mathf.Sin(rotation) + center.x;
            float y = width*Mathf.Cos(dLiXin)*Mathf.Sin(rotation) + height*Mathf.Sin(dLiXin)*Mathf.Cos(rotation) + center.y;
            return new Vector2(x, y);
        }

        
        /// <summary>
        /// 遍历三维格子点 
        /// X->Z->Y
        /// </summary>
        public static void SpaceForeachGrid(Vector3 space_min,Vector3 space_max,Vector3Int girdCount,Vector3 pivot,Action<Vector3Int,int,Vector3> callback)
        {
            var space_size = space_max-space_min;
            Vector3 gridSize = VectorHelper.Division(space_size,girdCount);

            Vector3 pivotOffset = VectorHelper.Multiply(gridSize,pivot);
            int index = 0;
            for (int y_idx = 0; y_idx < gridSize.y; y_idx++)
            {
                for (int z_idx = 0; z_idx < gridSize.z; z_idx++)
                {
                    for (int x_idx = 0; x_idx < gridSize.x; x_idx++)
                    {
                        var coord = new Vector3Int(x_idx,y_idx,z_idx);
                        var point = SpaceCoord2Point(coord,space_min,gridSize,pivotOffset);
                        callback(coord,index++,point);
                    }
                }
            }
        }

        /// <summary>
        /// 空间索引的位置
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="space_min"></param>
        /// <param name="gridSize"></param>
        /// <param name="pivotOffset"></param>
        /// <returns></returns>
        public static Vector3 SpaceCoord2Point(Vector3Int coord,Vector3 space_min,Vector3 gridSize,Vector3 pivotOffset)
        {
            Vector3 point = space_min + VectorHelper.Multiply(coord,gridSize)+pivotOffset;
            return point;
        }

        /// <summary>
        /// 点在空间内
        /// </summary>
        /// <param name="space_min"></param>
        /// <param name="space_max"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool SpacePointInside(Vector3 space_min,Vector3 space_max,Vector3 point)
        {
            if(point.x<space_min.x || point.x>=space_max.x)return false;
            if(point.y<space_min.y || point.y>=space_max.y)return false;
            if(point.z<space_min.z || point.z>=space_max.z)return false;
            return true;
        }

        /// <summary>
        /// 坐标在空间内
        /// </summary>
        /// <param name="gridCount"></param>
        /// <param name="coord"></param>
        /// <returns></returns>
        public static bool SpaceCoordInside(Vector3Int gridCount,Vector3Int coord)
        {
            if(coord.x<0 || coord.x>=gridCount.x)return false;
            if(coord.y<0 || coord.y>=gridCount.y)return false;
            if(coord.z<0 || coord.z>=gridCount.z)return false;
            return true;
        }


        /// <summary>
        /// 空间格子索引 
        /// X->Z->Y
        /// </summary>
        public static int SpaceCoord2Index(Vector3Int gridCount,Vector3Int coord)
        {
            int idx = -1;
            if(!SpaceCoordInside(gridCount,coord))return idx;
            idx = coord.y*(gridCount.x*gridCount.z) + coord.z*gridCount.x + coord.x;
            return idx;
        }

        /// <summary>
        /// 点在空间的索引
        /// </summary>
        public static int SpacePoint2Index(Vector3 space_min,Vector3 space_max,Vector3 point,Vector3Int gridCount)
        {
            int idx = -1;
            idx = SpaceCoord2Index(gridCount,SpacePoint2Coord(space_min,space_max,point,gridCount));
            return idx;
        }

        /// <summary>
        /// 点在空间的坐标
        /// </summary>
        public static Vector3Int SpacePoint2Coord(Vector3 space_min,Vector3 space_max,Vector3 point,Vector3Int gridCount)
        {
            var spaceSize = space_max-space_min;
            int coord_x = MathHelper.IndexOf(gridCount.x,point.x,spaceSize.x,space_min.x);
            int coord_y = MathHelper.IndexOf(gridCount.y,point.y,spaceSize.y,space_min.y);
            int coord_z = MathHelper.IndexOf(gridCount.z,point.z,spaceSize.z,space_min.z);

            return new Vector3Int(coord_x,coord_y,coord_z);
        }

    
    }
}