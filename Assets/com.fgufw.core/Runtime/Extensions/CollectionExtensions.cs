using System;
using System.Collections;
using System.Collections.Generic;

namespace FGUFW
{
    public static class CollectionExtensions
    {
        public static T Random<T>(this List<T> self)
        {
            if(self==null || self.Count==0)
            {
                return default(T);
            }
            int idx = UnityEngine.Random.Range(0,self.Count);
            return self[idx];
        }

        public static T Random<T>(this T[] self)
        {
            if(self==null || self.Length==0)
            {
                return default(T);
            }
            int idx = UnityEngine.Random.Range(0,self.Length);
            return self[idx];
        }

        public static int IndexOf<T>(this T[] self,T val,int startIndex=0)
        {
            if(self!=null)
            {
                return Array.IndexOf<T>(self,val,startIndex);
            }
            return -1;
        }

        public static int IndexOf<T>(this T[] self,Predicate<T> match,int startIndex=0,int length=0)
        {
            if(self!=null)
            {
                if(length==0)length = self.Length;
                
                for (int i = startIndex; i < length; i++)
                {
                    var t_obj = self[i];
                    if(match(t_obj))return i;
                }
            }
            return -1;
        }

        public static T Find<T>(this T[] self,Predicate<T> match)
        {
            if(self!=null)
            {
                int length = self.Length;
                for (int i = 0; i < length; i++)
                {
                    var t_obj = self[i];
                    if(match(t_obj))return t_obj;
                }
            }
            return default(T);
        }

        
        /// <summary>
        /// 按权重随机
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="getWeight"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RandomByWeight<T>(this IEnumerable collection,Func<T,float> getWeight)
        {
            float maxValue = 0;
            foreach (T item in collection)
            {
                maxValue += getWeight(item);
            }
            float val = UnityEngine.Random.Range(0,maxValue);
            float weight = 0;
            foreach (T item in collection)
            {
                weight += getWeight(item);
                if(val<weight)
                {
                    return item;
                }
            }
            return default(T);
        }

        public static T[] Copy<T>(this T[] self,int length)
        {
            if(self==null || self.Length==0)
            {
                return null;
            }
            T[] newArray = new T[length];
            Array.Copy(self,newArray,length);
            return newArray;
        }

        public static U[] To<T,U>(this T[] self,Func<T,U> convert)
        {
            var length = self.Length;
            U[] array = new U[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = convert(self[i]);
            }
            return array;
        }

        public static void RemoveAtSwapBack<T>(this List<T> self,int index)
        {
            int backIndex = self.Count-1;
            self[index] = self[backIndex];
            self.RemoveAt(backIndex);
        }

        public static void RemoveSwapBack<T>(this List<T> self,T item)
        {
            int index = self.IndexOf(item);
            if(index==-1)return;
            int backIndex = self.Count-1;
            self[index] = self[backIndex];
            self.RemoveAt(backIndex);
        }

        public static void RemoveSwapBack<T>(this List<T> self,Predicate<T> match)
        {
            int index = self.FindIndex(match);
            if(index==-1)return;
            int backIndex = self.Count-1;
            self[index] = self[backIndex];
            self.RemoveAt(backIndex);
        }

        public static void RemoveAllSwapBack<T>(this List<T> self,Predicate<T> match)
        {
            int length = self.Count;
            for (int i = 0; i < length; i++)
            {
                int index = self.FindIndex(match);
                if(index==-1)return;
                int backIndex = self.Count-1;
                self[index] = self[backIndex];
                self.RemoveAt(backIndex);       
            }
        }

        public static void ReplaceAllData<T>(this List<T> self,T[] array)
        {
            int length = array==null?0:array.Length;

            for (int i = self.Count; i > length; i--)
            {
                self.RemoveAt(i-1);
            }

            for (int i = 0; i < length; i++)
            {
                if(i<self.Count)
                {
                    self[i] = array[i];
                }
                else
                {
                    self.Add(array[i]);
                }
            }
        }

        public static void Clean<T>(this List<T> self)
        {
            for (int i = self.Count-1; i >= 0; i--)
            {
                self.RemoveAt(i);
            }
        }

    }
}