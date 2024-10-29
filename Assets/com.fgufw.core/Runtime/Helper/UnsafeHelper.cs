using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace FGUFW
{
    public unsafe static class UnsafeHelper
    {
        public static T* Malloc<T>(int length=1,Allocator allocator = Allocator.Persistent) where T:unmanaged
        {
            long size = UnsafeUtility.SizeOf<T>();
            int align = UnsafeUtility.AlignOf<T>();
            var intPtr = UnsafeUtility.Malloc(size*length,align,allocator);
            return (T*)intPtr;
        }

        public static void Free(IntPtr intPtr,Allocator allocator = Allocator.Persistent)
        {
            UnsafeUtility.Free(intPtr.ToPointer(),allocator);
        }

        public static ref T AsRef<T>(void* intPtr,int index=0) where T:unmanaged
        {
            T* t_ip = (T*)intPtr;
            t_ip += index;
            return ref UnsafeUtility.AsRef<T>(t_ip);
        }

    }
}