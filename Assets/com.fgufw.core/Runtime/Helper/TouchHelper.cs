using System;
using UnityEngine;

namespace FGUFW
{
    public static class TouchHelper
    {
        public static bool GetTouch(int fingerId,ref Touch touch)
        {
            foreach (var item in Input.touches)
            {
                if(item.fingerId==fingerId)
                {
                    touch = item;
                    return true;
                }
            }
            return false;
        }

        public static bool GetLeftTouch(ref Touch touch)
        {
            foreach (var item in Input.touches)
            {
                if(item.position.x <= Screen.width/2)
                {
                    touch = item;
                    return true;
                }
            }
            return false;
        }

        public static bool GetRightTouch(ref Touch touch)
        {
            foreach (var item in Input.touches)
            {
                if(item.position.x > Screen.width/2)
                {
                    touch = item;
                    return true;
                }
            }
            return false;
        }

    }
}