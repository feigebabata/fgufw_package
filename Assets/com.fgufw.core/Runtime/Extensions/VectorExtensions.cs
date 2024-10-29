using UnityEngine;
using System;

namespace FGUFW
{
    public static class VectorExtensions
    {
        public static Vector2 xy(this Vector3 self)
        {
            return new Vector2(self.x,self.y);
        }
    }
}