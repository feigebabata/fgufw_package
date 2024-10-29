using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.PlayerLoop;
using UnityEngine.EventSystems;

namespace FGUFW
{
    /// <summary>
    /// UInt64为4个key(UInt16)组合
    /// </summary>
    public static class KeyboardHelper
    {
        static IList<KeyCode> keyDownListener = new List<KeyCode>();

        static IOrderedMessenger<UInt64> keyDownMessenger=new OrderedMessenger<UInt64>();
        
        static Dictionary<KeyCode,int> keyDownCounter = new Dictionary<KeyCode, int>();

        static OrderedLinkedList<Action> clickScreenNonUI = new OrderedLinkedList<Action>();
        static bool abortClickScreen;

        public static void AddListener_KeyDown(Action callback,int weight,params KeyCode[] keyCodes)
        {
            var keys = ToKey(keyCodes);
            keyDownMessenger.Add(keys,callback,weight);

            if(keyDownListener.Count==0)
            {
                PlayerLoopHelper.AddToLoop<PreUpdate,CheckKeyDownEvent>(checkKeyDownEvent);
            }

            foreach (var keyCode in keyCodes)
            {
                if(keyDownCounter.ContainsKey(keyCode))
                {
                    keyDownCounter[keyCode]++;
                }
                else
                {
                    keyDownCounter[keyCode]=1;
                }

                if(!keyDownListener.Contains(keyCode))
                {
                    keyDownListener.Add(keyCode);
                }
            }
        }

        public static void RemoveListener_KeyDown(Action callback,params KeyCode[] keyCodes)
        {
            var keys = ToKey(keyCodes);
            if(keyDownMessenger.Remove(keys,callback))
            {
                foreach (var keyCode in keyCodes)
                {
                    if(keyDownCounter.ContainsKey(keyCode))
                    {
                        keyDownCounter[keyCode]--;

                        if(keyDownCounter[keyCode]==0)
                        {
                            keyDownCounter.Remove(keyCode);
                            keyDownListener.Remove(keyCode);
                        }
                    }
                }
                if(keyDownListener.Count==0)
                {
                    PlayerLoopHelper.RemoveToLoop<PreUpdate>(checkKeyDownEvent);
                }
            }
            
        }

        public static void Abort_KeyDown(UInt64 keys)
        {
            keyDownMessenger.Abort(keys);
        }

        public static void Abort_KeyDown(params KeyCode[] keyCodes)
        {
            var keys = ToKey(keyCodes);
            keyDownMessenger.Abort(keys);
        }

        public static UInt64 ToKey(params KeyCode[] keys)
        {
            UInt64 key = 0;
            if(keys==null || keys.Length==0)return key;

            Array.Sort<KeyCode>(keys,(l,r)=>{return l-r;});

            for (int i = 0; i < keys.Length; i++)
            {
                UInt64 val = (UInt64)keys[i];
                val = val<<(i*16);
                key = key | val;
            }

            return key;
        }

        class CheckKeyDownEvent{}

        static void checkKeyDownEvent()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                // Debug.Log("");
            }
            var (keys,count) = getDownKeys();
            var isKeyDown = false;
            for (int i = 0; i < keyDownListener.Count; i++)
            {
                var key = keyDownListener[i];
                if(Input.GetKeyDown(key))
                {
                    if(!KeyInKeys(key,keys))AddKey(key,ref keys,ref count);
                    isKeyDown = true;
                }
                if(count>=4)break;
            }

            if(!isKeyDown)return;
            keyDownMessenger.Broadcast(keys);
        }

        static ValueTuple<UInt64,int> getDownKeys()
        {
            UInt64 keys = 0b_0000000000000000_0000000000000000_0000000000000000_0000000000000000;
            int count = 0;
            int length = keyDownListener.Count;
            for (int i = 0; i < length; i++)
            {
                var key = keyDownListener[i];
                if(Input.GetKey(key))
                {
                    AddKey(key,ref keys,ref count);
                }
                if(count>=4)break;
            }
            return (keys,count);
        }

        public static bool KeyInKeys(KeyCode key,UInt64 keys)
        {
            for (int i = 0; i < 4; i++)
            {
                UInt64 val = (UInt64)key;
                val = val<<(i*16);
                UInt64 origin = val;
                if((val & keys)==origin)return true;
            }
            return false;
        }

        public static void AddKey(KeyCode key,ref UInt64 keys,ref int count)
        {
            UInt64 val = (UInt64)key;
            val = val << (count*16);
            keys = keys | val;
            count++;
        }

        public static void AddListener_ClickScreenNonUI(Action callback,int weight=0)
        {
            if(clickScreenNonUI.Length==0)
            {
                PlayerLoopHelper.AddToLoop<Update,CheckClickScreen>(checkClickScreen);
            }
            clickScreenNonUI.Add(weight,callback);
        }

        class CheckClickScreen{}

        private static void checkClickScreen()
        {
            if(Input.GetMouseButtonDown(0))
            {
                if(EventSystem.current!=null && EventSystem.current.currentSelectedGameObject!=null)return;

                foreach (var item in clickScreenNonUI)
                {
                    if(!abortClickScreen)
                    {
                        item.Value();
                        abortClickScreen = false;
                        return;
                    }
                }
            }
        }

        public static void RemoveListener_ClickScreenNonUI(Action callback)
        {
            clickScreenNonUI.Remove(callback);
            if(clickScreenNonUI.Length==0)
            {
                PlayerLoopHelper.RemoveToLoop<Update>(checkClickScreen);
            }
        }

        public static void Abort_ClickScreenNonUI()
        {
            abortClickScreen = true;
        }
    }
}