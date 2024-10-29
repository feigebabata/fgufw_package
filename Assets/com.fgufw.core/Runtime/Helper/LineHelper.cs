using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace FGUFW
{
    public static class LineHelper
    {
        /// <summary>
        /// 获取平行线
        /// </summary>
        /// <param name="line"></param>
        /// <param name="offset"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vector3[] GetParallel(Vector3[] line,float offset,Vector3 normal)
        {
            var newLine = new Vector3[line.Length];
            normal = normal.normalized;
            for (int i = 0; i < line.Length; i++)
            {
                Vector3 point = line[i];
                Vector3 lineDir;
                if(i==0)
                {
                    lineDir = (line[i+1]-line[i]).normalized;
                }
                else
                {
                    lineDir = (line[i]-line[i-1]).normalized;
                }
                var newPointDir = Vector3.Cross(lineDir,normal);
                var newPoint = point + newPointDir*offset;
                newLine[i]=newPoint;
            }
            return newLine;
        }

        /// <summary>
        /// 生成线的多边形2D碰撞体的点集合 (未完成)
        /// </summary>
        /// <param name="line"></param>
        /// <param name="offset"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vector2[] GetPolygon2DPoints(Vector3[] line,float offset,Vector3 normal)
        {
            var points = new Vector2[line.Length*2];
            normal = normal.normalized;
            for (int i = 0; i < line.Length; i++)
            {
                Vector3 point = line[i];
                Vector3 lineDir;
                if(i==0)
                {
                    lineDir = (line[i+1]-line[i]).normalized;
                }
                else
                {
                    lineDir = (line[i]-line[i-1]).normalized;
                }

                var newPointDir = Vector3.Cross(lineDir,normal);

                if(i%2==1)
                {
                    newPointDir *= -1;
                }
                points[i*2]=point + newPointDir*offset;
                newPointDir *= -1;
                points[i*2+1]=point + newPointDir*offset;
            }
            return points;
        }


    }

}