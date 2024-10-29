using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace FGUFW
{
    public static class PlayerLoopHelper
    {
        /// <summary>
        /// 添加自定义的回调在循环
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="first">插入头 否则末尾</param>
        /// <typeparam name="T">T类型须在命名空间 UnityEngine.PlayerLoop</typeparam>
        /// <typeparam name="U">赋予Name 在Profiler可见</typeparam>
        /// <returns></returns>
        public static void AddToLoop<T,U>(PlayerLoopSystem.UpdateFunction callback,bool first=false)
        {
            AddToLoop<T>(callback,typeof(U),first);
        }

        public static void AddToLoop<T>(PlayerLoopSystem.UpdateFunction callback,Type from,bool first=false)
        {
            var currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            var updateIndex = currentPlayerLoop.subSystemList.IndexOf<PlayerLoopSystem>(p=> p.type==typeof(T));
            var updateSys = currentPlayerLoop.subSystemList[updateIndex];
            var ls = new List<PlayerLoopSystem>(updateSys.subSystemList);
            var newLoop = new PlayerLoopSystem
            {
                type = from,
                updateDelegate = callback
            };
            if(first)
            {
                ls.Insert(0,newLoop);
            }
            else
            {
                ls.Add(newLoop);
            }
            updateSys.subSystemList = ls.ToArray();
            currentPlayerLoop.subSystemList[updateIndex] = updateSys;
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
        }
        
        public static void RemoveToLoop<T>(PlayerLoopSystem.UpdateFunction callback)
        {
            var currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            var updateIndex = currentPlayerLoop.subSystemList.IndexOf<PlayerLoopSystem>(p=> p.type==typeof(T));
            var updateSys = currentPlayerLoop.subSystemList[updateIndex];
            var ls = new List<PlayerLoopSystem>(updateSys.subSystemList);
            ls.RemoveAll(pls=>pls.updateDelegate==callback);
            updateSys.subSystemList = ls.ToArray();
            currentPlayerLoop.subSystemList[updateIndex] = updateSys;
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
        }

    }
}