using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW
{
    public static class GameObjectExtensions
    {
        public static GameObject Copy(this GameObject self,Transform parent)
        {
            return GameObject.Instantiate(self,parent);
        }

        
        public static bool IsNull(this UnityEngine.Object self)
        {
            return self==null || !(self is UnityEngine.Object);
        }
    }
}