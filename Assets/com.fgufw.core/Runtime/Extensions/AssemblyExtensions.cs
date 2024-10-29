using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FGUFW
{
    public static class AssemblyExtensions
    {
        public static void FilterClassAndStruct<T>(Action<Type> callback)
        {
            if (callback==null) return;

            var t_type = typeof(T);

            var assemblyName = typeof(T).Assembly.FullName;
            var assemblys = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblys)
            {
                if (assembly.FullName!=assemblyName && !Array.Exists(assembly.GetReferencedAssemblies(), a => a.FullName == assemblyName)) continue;
                foreach (var type in assembly.GetTypes())
                {
                    if (!(type.IsClass || type.IsValueType)) continue;
                    
                    //被继承/实现
                    if(t_type.IsAssignableFrom(type))
                    {
                        callback(type);
                    }
                    
                }
            }
        }

        public static void Filter(Func<Type, bool> match,Action<Type> callback)
        {
            if (callback == null || match == null) return;

            var assemblys = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblys)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (match(type))
                    {
                        callback(type);
                    }
                }
            }
        }

        public static void FilterAssembly<T>(string assemblyName, Action<Type> callback)
        {
            if (callback == null) return;
            var assembly = Array.Find<System.Reflection.Assembly>(AppDomain.CurrentDomain.GetAssemblies(), a => assemblyName.Contains(a.GetName().Name));
            if (assembly == null) return;

            var t_type = typeof(T);
            foreach (var type in assembly.GetTypes())
            {
                if (!(type.IsClass || type.IsValueType)) continue;

                //被继承/实现
                if (t_type.IsAssignableFrom(type))
                {
                    callback(type);
                }

            }
        }




    }
}
