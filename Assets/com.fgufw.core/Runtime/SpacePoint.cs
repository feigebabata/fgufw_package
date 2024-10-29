using System;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW
{
    /// <summary>
    /// 空间划分 点模式
    /// 空间由X*Y*Z个格子构成 格子顺序X >> Y >> Z
    /// 点在格子内: gridMin >= 点 < gridMax
    /// 空间之外点的所在格子索引为-1
    /// </summary>
    public sealed class SpacePoint
    {
        private const int EXPAND = 1024;

        public int GridLength{get;private set;}
        public int X{get;private set;}
        public int Y{get;private set;}
        public int Z{get;private set;}

        /// <summary>
        /// 每个格子里点的数量
        /// </summary>
        private int[] _gridCapacitys;
        private Vector3 _center,_girdSize,_spaceMinPoint,_spaceMaxPoint;
        private Bounds _spaceBounds;

        //----------------------------------------------

        private int _newPointId=0;
        public int PointCount{get;private set;}

        /// <summary>
        /// 所有点索引的集合 按格子顺序 先存outSide
        /// </summary>
        private int[] _pointIds;
        private Vector3[] _points;

        private SpacePoint(){}

        public SpacePoint(Vector3Int spaceSize, Vector3 gridSize,Vector3 center,int capacity=8)
        {
            GridLength = spaceSize.x*spaceSize.y*spaceSize.z;
            X = spaceSize.x;
            Y = spaceSize.y;
            Z = spaceSize.z;
            _gridCapacitys = new int[GridLength+1];
            _center = center;
            _girdSize = gridSize;

            var size = new Vector3(gridSize.x*X,gridSize.y*Y,gridSize.z*Z);
            Vector3 half = size*0.5f;
            _spaceMinPoint = center - half;
            _spaceMaxPoint = center + half;
            _spaceBounds = new Bounds(center,size);

            if(capacity<=0)capacity=8;
            _pointIds = new int[capacity];
            _points = new Vector3[capacity];
        }

        /// <summary>
        /// 遍历某个格子里所有点的Id
        /// </summary>
        public void ForeachGrid(int gridIndex,Action<int,Vector3> callback)
        {
            if(callback==null)return;
            int pointIndex = getGridFirstPointIndex(gridIndex);
            int length = pointIndex+_gridCapacitys[gridIndex+1];
            for (;pointIndex < length; pointIndex++)
            {
                callback(_pointIds[pointIndex],_points[pointIndex]);
            }
        }

        /// <summary>
        /// 获取点的位置
        /// </summary>
        public Vector3 GetPoint(int pointId)
        {
            int pointIndex = getPointIndex(pointId);
            return _points[pointIndex];
        }

        private int getPointIndex(int pointId)
        {
            for (int i = 0; i < PointCount; i++)
            {
                if(_pointIds[i]==pointId)
                {
                    return i;
                }
            }            
            return -1;
        }

        private int getGridFirstPointIndex(int gridIndex)
        {
            int pointIndex=0;
            for (int i = -1; i < gridIndex; i++)
            {
                pointIndex+=_gridCapacitys[i+1];
            }            
            return pointIndex;
        }

        /// <summary>
        /// 添加新点对象 返回点Id
        /// </summary>
        public int Add(Vector3 point)
        {
            if(PointCount+1>_pointIds.Length)ensureCapacity();

            var pointId = _newPointId++;
            int gridIndex = GetGridIndex(point);
            int pointIndex = getGridFirstPointIndex(gridIndex);
            moveBack(gridIndex);
            _pointIds[pointIndex] = pointId;
            _points[pointIndex] = point;
            PointCount++;
            _gridCapacitys[gridIndex+1]++;

            return pointId;
        }

        /// <summary>
        /// 移除点
        /// </summary>
        public void Remove(int pointId)
        {
            int pointIndex = getPointIndex(pointId);
            int gridIndex = GetGridIndex(_points[pointIndex]);
            moveForward(pointIndex,gridIndex);
            _gridCapacitys[gridIndex+1]--;
            PointCount--;
        }

        /// <summary>
        /// 点位置改变 返回移动后所在格子Id
        /// </summary>
        public int PointMove(int pointId,Vector3 newPos)
        {
            int pointIndex = getPointIndex(pointId);
            int oldGridIndex = GetGridIndex(_points[pointIndex]);
            int newGridIndex = GetGridIndex(newPos);
            if(oldGridIndex==newGridIndex)return newGridIndex;
            movePointIndex(pointIndex,oldGridIndex,newGridIndex);
            _gridCapacitys[newGridIndex+1]++;
            _gridCapacitys[oldGridIndex+1]--;
            return newGridIndex;
        }

        private void ensureCapacity() 
        {
            int capacity = _pointIds.Length;
            var expand = Math.Min(capacity,EXPAND);
            var newCapacity = capacity+expand;
            int[] newPointIndexs = new int[newCapacity];
            Vector3[] newPoints = new Vector3[newCapacity];
            Array.Copy(_pointIds,newPointIndexs,capacity);
            Array.Copy(_points,newPoints,capacity);
            _pointIds = newPointIndexs;
            _points = newPoints;
        }

        /// <summary>
        /// 获取格子容量
        /// </summary>
        public int GetGridCapacity(int gridIndex)
        {
            return _gridCapacitys[gridIndex+1];
        }


        /// <summary>
        /// 获取点在空间中的格子索引
        /// </summary>
        public int GetGridIndex(Vector3 point)
        {
            int gridIndex = -1;
            int idx_x = MathHelper.IndexOf(X,point.x,_spaceMaxPoint.x-_spaceMinPoint.x,_spaceMinPoint.x);
            int idx_y = MathHelper.IndexOf(Y,point.y,_spaceMaxPoint.y-_spaceMinPoint.y,_spaceMinPoint.y);
            int idx_z = MathHelper.IndexOf(Z,point.z,_spaceMaxPoint.z-_spaceMinPoint.z,_spaceMinPoint.z);
            if(idx_x==-1 || idx_y==-1 || idx_z==-1)
            {
                return gridIndex;
            }
            gridIndex = getGridIndex(idx_x,idx_y,idx_z);
            return gridIndex;
        }

        private int getGridIndex(int x,int y,int z)
        {
            return z*X*Y + y*X + x;
        }

        /// <summary>
        /// 从gridIndex开始整体后移 留出gridindex第一个point的空位
        /// </summary>
        private void moveBack(int startGridIndex)
        {
            for (int gridIndex = GridLength,index=PointCount; gridIndex >= startGridIndex; gridIndex--)
            {
                int capacity = _gridCapacitys[gridIndex+1];
                if(capacity>0)
                {
                    _pointIds[index] = _pointIds[index-capacity];
                    _points[index] = _points[index-capacity];
                    index -= capacity;
                }
            }
        }

        /// <summary>
        /// 从gridIndex开始整体前移 覆盖pointIndex
        /// </summary>
        private void moveForward(int startPointIndex,int startGridIndex)
        {
            int tempId=_pointIds[PointCount-1];
            Vector3 tempPoint=_points[PointCount-1];
            for (int gridIndex = GridLength,pointIndex=PointCount-1; gridIndex >= startGridIndex; gridIndex--)
            {
                int capacity = _gridCapacitys[gridIndex+1];
                if(capacity>0)
                {
                    if(gridIndex==startGridIndex)
                    {
                        _pointIds[startPointIndex] = tempId;
                        _points[startPointIndex] = tempPoint;
                    }
                    else
                    {
                        int index = pointIndex-capacity;
                        int id = _pointIds[index];
                        Vector3 point = _points[index];
                        _pointIds[index] = tempId;
                        _points[index] = tempPoint;

                        tempId = id;
                        tempPoint = point;
                    }
                    pointIndex -= capacity;
                }
            }
        }

        /// <summary>
        /// 移动point的位置
        /// </summary>
        private void movePointIndex(int startPointIndex,int startGridIndex,int endGridIndex)
        {
            int tempId=_pointIds[startPointIndex];
            Vector3 tempPoint=_points[startPointIndex];

            if(endGridIndex>startGridIndex)
            {
                int pointIndex = getGridFirstPointIndex(startGridIndex);
                for (int gridIndex = startGridIndex; gridIndex <= endGridIndex; gridIndex++)
                {
                    int capacity = _gridCapacitys[gridIndex+1];
                    if(capacity>0)
                    {
                        if(gridIndex==startGridIndex)
                        {
                            _pointIds[startPointIndex]=_pointIds[pointIndex+capacity-1];
                            _points[startPointIndex]=_points[pointIndex+capacity-1];
                        }
                        else
                        {
                            _pointIds[pointIndex-1]=_pointIds[pointIndex+capacity-1];
                            _points[pointIndex-1]=_points[pointIndex+capacity-1];
                        }
                    }
                    pointIndex+=capacity;
                }
                _pointIds[pointIndex-1]=tempId;
                _points[pointIndex-1]=tempPoint;   

            }
            else
            {
                int pointIndex = getGridFirstPointIndex(startGridIndex);
                for (int gridIndex = startGridIndex; gridIndex >= endGridIndex; gridIndex--)
                {
                    
                    int capacity = _gridCapacitys[gridIndex+1];
                    if(capacity>0)
                    {
                        if(gridIndex==startGridIndex)
                        {
                            _pointIds[startPointIndex]=_pointIds[pointIndex];
                            _points[startPointIndex]=_points[pointIndex];
                        }
                        else
                        {
                            _pointIds[pointIndex-capacity]=_pointIds[pointIndex];
                            _points[pointIndex-capacity]=_points[pointIndex];
                        }
                    }
                    pointIndex-=capacity;
                }
                _pointIds[pointIndex]=tempId;
                _points[pointIndex]=tempPoint;  
            }
        }

        /// <summary>
        /// 返回重叠的所有格子索引
        /// </summary>
        public int[] OverlapGrids(Bounds bounds)
        {
            Vector3Int boxInGridSize = Vector3Int.zero;
            Vector3Int boxInGridIndex = Vector3Int.zero;

            var (overlap,overlapBounds) = _spaceBounds.Overlap(bounds);
            if(!overlap)return new int[]{-1};
            var min = overlapBounds.min;
            var size = overlapBounds.size;
            
            boxInGridIndex.x = MathHelper.IndexOf(X,min.x,_spaceMaxPoint.x-_spaceMinPoint.x,_spaceMinPoint.x);
            boxInGridIndex.y = MathHelper.IndexOf(Y,min.y,_spaceMaxPoint.y-_spaceMinPoint.y,_spaceMinPoint.y);
            boxInGridIndex.z = MathHelper.IndexOf(Z,min.z,_spaceMaxPoint.z-_spaceMinPoint.z,_spaceMinPoint.z);

            float length = 0;
            length = (size.x - (boxInGridIndex.x+1)*_girdSize.x - min.x)/_girdSize.x;
            boxInGridSize.x = length<0?1:2+(int)(length/_girdSize.x);

            length = (size.y - (boxInGridIndex.y+1)*_girdSize.y - min.y)/_girdSize.y;
            boxInGridSize.y = length<0?1:2+(int)(length/_girdSize.y);

            length = (size.z - (boxInGridIndex.z+1)*_girdSize.z - min.z)/_girdSize.z;
            boxInGridSize.z = length<0?1:2+(int)(length/_girdSize.z);

            int[] ls;
            int count = boxInGridSize.x*boxInGridSize.y*boxInGridSize.z;
            if(
                bounds.min.x<_spaceBounds.min.x ||
                bounds.min.y<_spaceBounds.min.y ||
                bounds.min.z<_spaceBounds.min.z ||
                bounds.max.x>=_spaceBounds.max.x ||
                bounds.max.y>=_spaceBounds.max.y ||
                bounds.max.z>=_spaceBounds.max.z
            )
            {
                ls = new int[count+1];
                ls[count] = -1;
            }
            else
            {
                ls = new int[count];
            }

            for (int z = 0,i=0; z < boxInGridSize.z; z++)
            {
                for (int y = 0; y < boxInGridSize.y; y++)
                {
                    for (int x = 0; x < boxInGridSize.x; x++,i++)
                    {
                        ls[i] = getGridIndex(boxInGridIndex.x+x,boxInGridIndex.y+y,boxInGridIndex.z+z);
                    }
                }
            }

            return ls;
        }

        /// <summary>
        /// 返回重叠的所有格子索引
        /// </summary>
        public void OverlapGrids(Bounds bounds,Action<int> callback)
        {
            if(callback==null)return;
            Vector3Int boxInGridSize = Vector3Int.zero;
            Vector3Int boxInGridIndex = Vector3Int.zero;

            var (overlap,overlapBounds) = _spaceBounds.Overlap(bounds);
            if(!overlap)
            {
                callback(-1);
                return;
            }
            var min = overlapBounds.min;
            var size = overlapBounds.size;
            
            boxInGridIndex.x = MathHelper.IndexOf(X,min.x,_spaceMaxPoint.x-_spaceMinPoint.x,_spaceMinPoint.x);
            boxInGridIndex.y = MathHelper.IndexOf(Y,min.y,_spaceMaxPoint.y-_spaceMinPoint.y,_spaceMinPoint.y);
            boxInGridIndex.z = MathHelper.IndexOf(Z,min.z,_spaceMaxPoint.z-_spaceMinPoint.z,_spaceMinPoint.z);

            float length = 0;
            length = (size.x - (boxInGridIndex.x+1)*_girdSize.x - min.x)/_girdSize.x;
            boxInGridSize.x = length<0?1:2+(int)(length/_girdSize.x);

            length = (size.y - (boxInGridIndex.y+1)*_girdSize.y - min.y)/_girdSize.y;
            boxInGridSize.y = length<0?1:2+(int)(length/_girdSize.y);

            length = (size.z - (boxInGridIndex.z+1)*_girdSize.z - min.z)/_girdSize.z;
            boxInGridSize.z = length<0?1:2+(int)(length/_girdSize.z);

            int count = boxInGridSize.x*boxInGridSize.y*boxInGridSize.z;
            if(
                bounds.min.x<_spaceBounds.min.x ||
                bounds.min.y<_spaceBounds.min.y ||
                bounds.min.z<_spaceBounds.min.z ||
                bounds.max.x>=_spaceBounds.max.x ||
                bounds.max.y>=_spaceBounds.max.y ||
                bounds.max.z>=_spaceBounds.max.z
            )
            {
                callback(-1);
            }

            for (int z = 0,i=0; z < boxInGridSize.z; z++)
            {
                for (int y = 0; y < boxInGridSize.y; y++)
                {
                    for (int x = 0; x < boxInGridSize.x; x++,i++)
                    {
                        var gridIndex = getGridIndex(boxInGridIndex.x+x,boxInGridIndex.y+y,boxInGridIndex.z+z);
                        callback(gridIndex);
                    }
                }
            }
        }
        

    }
}