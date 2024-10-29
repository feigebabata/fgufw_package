using System;
using System.Collections.Generic;
using System.Reflection;

namespace FGUFW
{
    public static class TypeExtensions
    {
        private static Type[] reflectionNoneAges = new Type[0];
        private const BindingFlags All_MEMBER = BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance;
        private const BindingFlags STATIC_MEMBER = BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Static;

        /// <summary>
        /// 比较数据结构公开字段是否一致
        /// </summary>
        /// <param name="self"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool EqualsFileds(this Type self,Type t)
        {
            var fileds1 = self.GetFields();
            var fileds2 = t.GetFields();
            if (fileds1.Length != fileds2.Length) return false;
            foreach (var filed in fileds1)
            {
                if(!Array.Exists(fileds2,f=>f.Name==filed.Name && f.FieldType==filed.FieldType))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 字段编码
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ToScript(this FieldInfo self)
        {
            return $"public {GetFiledTypeName(self.FieldType)} {self.Name};";
        }
        
        /// <summary>
        /// 类型文本(泛型比较麻烦)
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetFiledTypeName(this Type t)
        {
            if (t.IsGenericType)
            {
                var index = t.Name.IndexOf('`');
                var name = t.Name.Substring(0,index);
                var generics = t.GetGenericArguments();
                int length = generics.Length;
                var types = new string[length];

                for (int i = 0; i < length; i++)
                {
                    types[i] = GetFiledTypeName(generics[i]);
                }
                return $"{t.Namespace}.{name}<{string.Join(",", types)}>";
            }
            else
            {
                return t.FullName;
            }
        }

        /// <summary>
        /// 包含特性
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool ContainsAttribute<T>(this Type self) where T:Attribute
        {
            var at = typeof(T);
            foreach (var item in self.GetCustomAttributesData())
            {
                if (item.AttributeType == at) return true;
            }
            return false;
        }

        /// <summary>
        /// 反射调用函数
        /// </summary>
        public static object ReflectionInvoke(this object self,string fun_name,params object[] ages)
        {
            Type[] ageTypes = null;
            if(ages==null && ages.Length==0)
            {
                ageTypes = reflectionNoneAges;
            }
            else
            {
                ageTypes = new Type[ages.Length];
                for (int i = 0; i < ages.Length; i++)
                {
                    ageTypes[i] = ages[i].GetType();
                }
            }
            MethodInfo method = self.GetType().GetMethod
            (
                fun_name,
                All_MEMBER,
                null,
                CallingConventions.Any,
                ageTypes,
                null
            );
            return method?.Invoke(self,ages);
        }


        /// <summary>
        /// 反射调用函数
        /// </summary>
        public static object ReflectionInvoke(this Type self,string fun_name,params object[] ages)
        {
            Type[] ageTypes = null;
            if(ages==null && ages.Length==0)
            {
                ageTypes = reflectionNoneAges;
            }
            else
            {
                ageTypes = new Type[ages.Length];
                for (int i = 0; i < ages.Length; i++)
                {
                    ageTypes[i] = ages[i].GetType();
                }
            }
            MethodInfo method = self.GetMethod
            (
                fun_name,
                STATIC_MEMBER,
                null,
                CallingConventions.Any,
                ageTypes,
                null
            );
            return method?.Invoke(self,ages);
        }

        /// <summary>
        /// 反射获取字段值
        /// </summary>
        /// <param name="self"></param>
        /// <param name="field_name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ReflectionField<T>(this object self,string field_name)
        {
            return (T)self.GetType().GetField(field_name,
            All_MEMBER).GetValue(self);
        }

        public static T Instance<T>(this Type self, params object[] args)
        {
            return (T)System.Activator.CreateInstance(self,args);
        }

        public static T GetAttribute<T>(this object self) where T:Attribute
        {
            return self.GetType().GetCustomAttribute<T>();
        }

    }
}
