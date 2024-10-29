using System;
using Unity.Mathematics;
using UnityEngine;
using Unity.Burst;

namespace FGUFW
{
    public static class VectorHelper
    {
        /// <summary>
        /// 约等于 ≈
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool Approximately(Vector3 v1,Vector3 v2)
        {
            return Mathf.Approximately(v1.x,v2.x) && Mathf.Approximately(v1.y,v2.y) && Mathf.Approximately(v1.z,v2.z);
        }

        /// <summary>
        /// 约等于 ≈
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static bool Approximately(Vector2 v1,Vector2 v2)
        {
            return Mathf.Approximately(v1.x,v2.x) && Mathf.Approximately(v1.y,v2.y);
        }


        /// <summary>
        /// 加速运动的轨迹
        /// </summary>
        /// <param name="startPoint">起点</param>
        /// <param name="startVelocity">初速度</param>
        /// <param name="gravityVelocity">加速度</param>
        /// <param name="t">时刻</param>
        /// <returns></returns>
        public static Vector3 Acceleration(Vector3 startPoint,Vector3 startVelocity,Vector3 gravityVelocity,float t)
        {
            Vector3 offset = Vector3.zero;
            offset.x = getMovingDistance(startVelocity.x,gravityVelocity.x,t);
            offset.z = getMovingDistance(startVelocity.z,gravityVelocity.z,t);
            offset.y = getMovingDistance(startVelocity.y,gravityVelocity.y,t);
            return startPoint+offset;
        }    
        
        /// <summary>
        /// 加速运动位移
        /// </summary>
        /// <param name="startSpeed">初速度</param>
        /// <param name="acceleration">加速度</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        static private float getMovingDistance(float startSpeed,float acceleration,float time)
        {
            return startSpeed*time+acceleration*time*time/2;
        } 

        /// <summary>
        /// 求加速运动 起点到终点的初速度
        /// </summary>
        /// <param name="startPoint">起点</param>
        /// <param name="endPoint">终点</param>
        /// <param name="gravityVelocity">加速度</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public static Vector3 AccelerationStartVelocity(Vector3 startPoint,Vector3 endPoint,Vector3 gravityVelocity,float time)
        {
            Vector3 space = endPoint-startPoint;
            Vector3 startVelocity = Vector3.zero;
            startVelocity.x = getMovingStartVelocity(space.x,gravityVelocity.x,time);
            startVelocity.y = getMovingStartVelocity(space.y,gravityVelocity.y,time);
            startVelocity.z = getMovingStartVelocity(space.z,gravityVelocity.z,time);
            return startVelocity;
        }    
        
        /// <summary>
        /// 获取移动初速度 加速运动的初速度
        /// </summary>
        /// <param name="distance">距离</param>
        /// <param name="acceleration">加速度</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        static private float getMovingStartVelocity(float distance,float acceleration,float time)
        {
            return (distance-acceleration*time*time/2)/time;
        }

        /// <summary>
        /// 点乘
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float Dot(Vector3 v1,Vector3 v2)
        {
            return (v1.x*v2.x+v1.y*v2.y+v1.z+v2.z)/(Mathf.Pow((v1.x*v1.x+v1.y*v1.y+v1.z+v1.z),0.5f)*Mathf.Pow((v2.x*v2.x+v2.y*v2.y+v2.z+v2.z),0.5f));
        }

        /// <summary>
        /// 向量求模
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float Magnitude(Vector3 v)
        {
            return Mathf.Pow(v.x*v.x+v.y*v.y+v.z+v.z,0.5f);
        }

        /// <summary>
        /// 计算AB与CD两条线段的交点.
        /// </summary>
        /// <param name="a">A点</param>
        /// <param name="b">B点</param>
        /// <param name="c">C点</param>
        /// <param name="d">D点</param>
        /// <param name="intersectPos">AB与CD的交点</param>
        /// <returns>是否相交 true:相交 false:未相交</returns>
        public static bool TryGetIntersectPoint(Vector3 a, Vector3 b, Vector3 c, Vector3 d, out Vector3 intersectPos)
        {
            intersectPos = Vector3.zero;

            Vector3 ab = b - a;
            Vector3 ca = a - c;
            Vector3 cd = d - c;

            Vector3 v1 = Vector3.Cross(ca, cd);

            if (Mathf.Abs(Vector3.Dot(v1, ab)) > 1e-6)
            {
                // 不共面
                return false;
            }

            if (Vector3.Cross(ab, cd).sqrMagnitude <= 1e-6)
            {
                // 平行
                return false;
            }

            Vector3 ad = d - a;
            Vector3 cb = b - c;
            // 快速排斥
            if (Mathf.Min(a.x, b.x) > Mathf.Max(c.x, d.x) || Mathf.Max(a.x, b.x) < Mathf.Min(c.x, d.x)
            || Mathf.Min(a.y, b.y) > Mathf.Max(c.y, d.y) || Mathf.Max(a.y, b.y) < Mathf.Min(c.y, d.y)
            || Mathf.Min(a.z, b.z) > Mathf.Max(c.z, d.z) || Mathf.Max(a.z, b.z) < Mathf.Min(c.z, d.z)
            )
                return false;

            // 跨立试验
            if (Vector3.Dot(Vector3.Cross(-ca, ab), Vector3.Cross(ab, ad)) > 0
                && Vector3.Dot(Vector3.Cross(ca, cd), Vector3.Cross(cd, cb)) > 0)
            {
                Vector3 v2 = Vector3.Cross(cd, ab);
                float ratio = Vector3.Dot(v1, v2) / v2.sqrMagnitude;
                intersectPos = a + ab * ratio;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 取y轴夹角 12点方向顺时针
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static float Angle_Y(Vector3 dir)
        {
            dir = dir.normalized;
            float angle = 0;
            if(dir.x>=0)
            {
                angle = Vector3.Angle(Vector3.up,dir);
            }
            else
            {
                angle = 360 - Vector3.Angle(Vector3.up,dir);
            }
            return angle%360;
        }

        public static bool AngleInRange(float angle,Vector2 range)
        {
            if(range==Vector2.zero)return true;

            float min = range.x;
            float max = range.y;
            if(min<max)
            {
                return angle>=min && angle<max;
            }
            else
            {
                return (angle>=min && angle<360) || (angle>=0 && angle<max);
            }
        }

        /// <summary>
        /// 点到线的距离
        /// </summary>
        /// <param name="point"></param>
        /// <param name="l_p1"></param>
        /// <param name="l_p2"></param>
        /// <returns></returns>
        public static float PointLineSpace(Vector3 point, Vector3 l_p1,Vector3 l_p2)
        {
            if(l_p1==l_p2)return Vector3.Distance(l_p1,point);
            
            Vector3 v1 = l_p1 - l_p2;
            Vector3 v2 = point - l_p2;

            //平行四边形面积
            float area = Vector3.Cross(v1,v2).magnitude;

            //高等于面积除底
            float space = area/v1.magnitude;
            return space;
        }

        /// <summary>
        /// 点到线段的距离 点到显得距离不能越过线段的两点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="l_p1"></param>
        /// <param name="l_p2"></param>
        /// <returns></returns>
        public static float PointInLineSpace(Vector3 point, Vector3 l_p1,Vector3 l_p2)
        {
            if(l_p1==l_p2)return Vector3.Distance(l_p1,point);

            Vector3 v1 = l_p1 - l_p2;
            Vector3 v2 = point - l_p2;

            //投影
            Vector3 f = Vector3.Project(v2,v1);
            float space = 0;
            if(f.normalized==v1.normalized)
            {
                if(f.magnitude>v1.magnitude)
                {
                    //点在l_p1侧
                    space = Vector3.Distance(point,l_p1);
                }
                else
                {
                    space = Vector3.Distance(point,l_p2+f);
                }
            }
            else
            {
                //点在l_p2侧
                space = Vector3.Distance(point,l_p2);
            }
            
            return space;
        }

        /// <summary>
        /// 判断两个点是否接近
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="distance"></param>
        /// <param name="d">最大允许间距</param>
        /// <returns></returns>
        public static bool IsPointNear(Vector3 p1, Vector3 p2, out float distance, float d = 1f)
        {
            distance = (p1 - p2).magnitude;
            return distance <= d;
        }


        /// <summary>
        /// Transform转Rect
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        public static Rect GetWorldRect(RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            float width = Math.Abs(Vector2.Distance(corners[0], corners[3]));
            float height = Math.Abs(Vector2.Distance(corners[0], corners[1]));
            return new Rect(corners[0], new Vector2(width, height));
        }

        /// <summary>
        /// 判断坐标点是否在Transform内
        /// </summary>
        /// <param name="trs"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool IsPointInRectTransform(Transform trs, Vector2 position)
        {
            RectTransform rect = trs as RectTransform;
            if (rect != null)
            {
                Rect worldRect = GetWorldRect(rect);
                return worldRect.Contains(position);
            }
            return false;
        }

        public static Vector3 Parse(string text)
        {
            var arr = text.Split(',');
            return new Vector3
            (
                arr.Length>0?arr[0].ToFloat():0,
                arr.Length>1?arr[1].ToFloat():0,
                arr.Length>2?arr[2].ToFloat():0
            );
        }

        /// <summary>
        /// 乘法 各维度相乘
        /// </summary>
        /// <returns></returns>
        public static Vector3 Multiply(Vector3 l,Vector3 r)
        {
            return new Vector3
            (
                l.x*r.x,
                l.y*r.y,
                l.z*r.z
            );
        }

        /// <summary>
        /// 乘法 各维度相乘
        /// </summary>
        /// <returns></returns>
        public static Vector2 Multiply(Vector2 l,Vector2 r)
        {
            return new Vector2
            (
                l.x*r.x,
                l.y*r.y
            );
        }

        /// <summary>
        /// 乘法 各维度相乘
        /// </summary>
        /// <returns></returns>
        public static Vector3 Multiply(Vector3 l,Vector3Int r)
        {
            return new Vector3
            (
                l.x*r.x,
                l.y*r.y,
                l.z*r.z
            );
        }

        /// <summary>
        /// 除法 各维度相除
        /// </summary>
        /// <returns></returns>
        public static Vector3 Division(Vector3 l,Vector3 r)
        {
            Vector3 val = Vector3.zero;
            if(l.x!=0 && r.x!=0)val.x=l.x/r.x;
            if(l.y!=0 && r.y!=0)val.y=l.y/r.y;
            if(l.z!=0 && r.z!=0)val.z=l.z/r.z;
            return val;
        }

        /// <summary>
        /// 除法 各维度相除
        /// </summary>
        /// <returns></returns>
        public static Vector3 Division(Vector3 l,Vector3Int r)
        {
            Vector3 val = Vector3.zero;
            if(l.x!=0 && r.x!=0)val.x=l.x/r.x;
            if(l.y!=0 && r.y!=0)val.y=l.y/r.y;
            if(l.z!=0 && r.z!=0)val.z=l.z/r.z;
            return val;
        }

        /// <summary>
        /// 射击夹角
        /// </summary>
        [BurstCompile]
        public static float3 ShootAngle(float3 direction,float3 axis,float interval,int length,int index)
        {
            if(length<=1)return direction;
            float angle = index*interval-interval*(length-1)/2;
            var rotate = float4x4.AxisAngle(axis,math.radians(angle));
            var dir = math.mul(new float4(direction,0),rotate);
            return dir.xyz;
        }

        [BurstCompile]
        public static float2 RandomInCircle(uint seed,float min,float max)
        {
            var random = new Unity.Mathematics.Random();
            random.InitState(seed);
            var point = new float2(random.NextFloat(min,max),0);
            var rotate = float4x4.AxisAngle(new float3(0,0,1),random.NextFloat(0,math.PI));
            point = math.mul(new float4(point,0,0),rotate).xy;
            Debug.Log(point);
            return point;
        }
        
        /// <summary>
        /// 扇形排布向量 旋转
        /// </summary>
        /// <returns></returns>
        public static Vector3 FanVectorAngle(Vector3 dir,Vector3 normal,float count,float idx,float angle)
        {
            
            angle *= idx - (count-1)/2;
            
            dir = Quaternion.AngleAxis(angle,normal)*dir;
            return dir;
        }
        
        /// <summary>
        /// 扇形排布向量 横向偏移
        /// </summary>
        /// <returns></returns>
        public static Vector3 FanVectorOffset(Vector3 point,Vector3 dir,Vector3 normal,float count,float idx,float offset)
        {
            offset *= idx-(count-1)/2;
            var offsetDir = Vector3.Cross(dir,normal);
            return point+offsetDir*offset;
        }

    }
}