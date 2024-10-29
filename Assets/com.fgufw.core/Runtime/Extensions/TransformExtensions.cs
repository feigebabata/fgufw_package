using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FGUFW
{
    public static class TransformExtensions
    {
        public static RectTransform AsRT(this Transform t)
        {
            return t as RectTransform;
        }
        public static RectTransform GetChildRT(this Transform t,int index)
        {
            return t.GetChild(index) as RectTransform;
        }

        public static IEnumerator MoveWorld(this Transform transform,Vector3 endPos,float time)
        {
            float startTime = Time.time;
            while (Time.time-startTime<time)
            {
                float t = (Time.time-startTime)/time;
                transform.position = Vector3.Lerp(transform.position,endPos,t);
                yield return null;
            }
            transform.position = endPos;
        }

        public static IEnumerator MoveLocal(this Transform transform,Vector3 endPos,float time)
        {
            float startTime = Time.time;
            while (Time.time-startTime<time)
            {
                float t = (Time.time-startTime)/time;
                transform.localPosition = Vector3.Lerp(transform.localPosition,endPos,t);
                yield return null;
            }
            transform.localPosition = endPos;
        }

        public static IEnumerator RotateLocal(this Transform transform,Vector3 endAngle,float time)
        {
            float startTime = Time.time;
            Quaternion rotation = Quaternion.Euler(endAngle);
            while (Time.time-startTime<time)
            {
                float t = (Time.time-startTime)/time;
                transform.localRotation = Quaternion.Lerp(transform.localRotation,rotation,t);
                yield return null;
            }
            transform.localRotation = rotation;
        }

        public static IEnumerator RotateWorld(this Transform transform,Vector3 endAngle,float time)
        {
            float startTime = Time.time;
            Quaternion rotation = Quaternion.Euler(endAngle);
            while (Time.time-startTime<time)
            {
                float t = (Time.time-startTime)/time;
                transform.rotation = Quaternion.Lerp(transform.rotation,rotation,t);
                yield return null;
            }
            transform.rotation = rotation;
        }

        public static void Foreach<VALUE>(this Transform transform,IEnumerable list,Action<Transform,VALUE> callback)
        {
            int idx = 0;
            if(list!=null)
            {
                var enumerator = list.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Transform item_t = transform.GetOrCreateChild(idx);
                    callback?.Invoke(item_t,(VALUE)enumerator.Current);
                    item_t.gameObject.SetActive(true);
                    idx++;
                }
            }
            for (int i = idx; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        public static void Foreach<COMP,VALUE>(this Transform transform,IEnumerable list,Action<COMP,VALUE> callback)
        {
            int idx = 0;
            if(list!=null)
            {
                var enumerator = list.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Transform item_t = transform.GetOrCreateChild(idx);
                    callback?.Invoke(item_t.GetComponent<COMP>(),(VALUE)enumerator.Current);
                    item_t.gameObject.SetActive(true);
                    idx++;
                }
            }
            for (int i = idx; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        public static void For(this Transform transform,int count,Action<int,Transform> callback)
        {
            for (int i = 0; i < count; i++)
            {
                Transform item_t = transform.GetOrCreateChild(i);
                callback?.Invoke(i,item_t);
                item_t.gameObject.SetActive(true);
            }
            for (int i = count; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        public static void For<T>(this Transform transform,int count,Action<int,T> callback)
        {
            for (int i = 0; i < count; i++)
            {
                Transform item_t = transform.GetOrCreateChild(i);
                callback(i,item_t.GetComponent<T>());
                item_t.gameObject.SetActive(true);
            }
            for (int i = count; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        public static string FullPath(this Transform t)
        {
            StringBuilder s = new StringBuilder();
            do
            {
                s.Insert(0,$"\\{t.name}");
                t = t.parent;
            } 
            while (t!=null);
            return s.ToString();
        }

        public static void Sort<T>(this Transform t,Comparison<T> comparison) where T:MonoBehaviour
        {
            List<T> childs = new List<T>(t.childCount);
            foreach (Transform item in t)
            {
                childs.Add(item.GetComponent<T>());
            }
            childs.Sort(comparison);
            for (int i = 0; i < childs.Count; i++)
            {
                childs[i].transform.SetSiblingIndex(i);
            }
        }

        public static Transform RandomChild(this Transform t)
        {
            int idx = UnityEngine.Random.Range(0,t.childCount);
            return t.GetChild(idx);
        }

        public static T RandomChild<T>(this Transform t)
        {
            int idx = UnityEngine.Random.Range(0,t.childCount);
            return t.GetChild(idx).GetComponent<T>();
        }

        public static Transform GetOrCreateChild(this Transform t,int index,string createName=null)
        {
            while (index >= t.childCount)
            {
                var child = GameObject.Instantiate(t.GetChild(0).gameObject,t);
                if(createName!=null)child.name=createName;
            }
            return t.GetChild(index);
        }

        public static T GetOrCreateChild<T>(this Transform t,int index,string createName=null)
        {
            while (index >= t.childCount)
            {
                var child = GameObject.Instantiate(t.GetChild(0).gameObject,t);
                if(createName!=null)child.name=createName;
            }
            return t.GetChild(index).GetComponent<T>();
        }
    }
}