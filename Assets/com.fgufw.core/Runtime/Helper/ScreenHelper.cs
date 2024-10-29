using System;
using UnityEngine;

namespace FGUFW
{
    public static class ScreenHelper
    {
        /// <summary>
        /// 当前帧率
        /// </summary>
        /// <value></value>
        public static int FPS { get; private set; } = 60;

        /// <summary>
        /// 平滑帧率
        /// </summary>
        /// <value></value>
        public static int SmoothFPS { get; private set; } = 60;

        private static int fpsCount, second;

        [RuntimeInitializeOnLoadMethod]
        private static void addListenerUpdate()
        {
            PlayerLoopHelper.RemoveToLoop<UnityEngine.PlayerLoop.PreUpdate>(updateCallvack);
            PlayerLoopHelper.AddToLoop<UnityEngine.PlayerLoop.PreUpdate,FPSCount>(updateCallvack);
        }

        private static void updateCallvack()
        {
            fpsCount++;
            int time = (int)TimeHelper.Time;
            if (time>second)
            {
                int newFPS = fpsCount/(time - second);
                SmoothFPS = (int)Mathf.Lerp(FPS,newFPS,0.5f);
                FPS = newFPS;
                fpsCount = 0;
                second = time;
            }
        }

        public static void Landscape()
        {
            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
        }

        public static void Portrait()
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
    }

    public class FPSCount{}
}