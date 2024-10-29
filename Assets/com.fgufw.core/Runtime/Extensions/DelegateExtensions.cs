using System;

namespace FGUFW
{
    public static class DelegateExtensions
    {
        public static void ForEach<T>(this Delegate d,Action<T> callback) where T:Delegate
        {
            var arr = d.GetInvocationList();
            foreach (T item in arr)
            {
                callback(item);
            }
            
        }
    }
}