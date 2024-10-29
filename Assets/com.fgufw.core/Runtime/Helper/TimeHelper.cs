using System;
using UnityEngine;

namespace FGUFW
{
    public static class TimeHelper
    {
        public static bool UnscaleTimeMode = true;

        public static float Time => UnscaleTimeMode ? UnityEngine.Time.unscaledTime : UnityEngine.Time.time;
        public static float DeltaTime => UnscaleTimeMode ? UnityEngine.Time.unscaledDeltaTime : UnityEngine.Time.deltaTime;
    }
}