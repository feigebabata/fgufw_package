using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace FGUFW
{
    public class OrderedMessenger<K> : IOrderedMessenger<K>
    {
        Dictionary<K,OrderedLinkedList<Delegate>> _dict = new Dictionary<K, OrderedLinkedList<Delegate>>();
        HashSet<K> _aborts = new HashSet<K>();

        public void Abort(K msgId)
        {
            // Debug.LogWarning("Abort "+msgId);
            _aborts.Add(msgId);
        }

        public void Add(K msgId, Action callback, int weight = 0)
        {
            if(callback==null)Debug.LogError($"回调不能为空:msgId={msgId}");

            if(!_dict.ContainsKey(msgId))
            {
                _dict.Add(msgId,new OrderedLinkedList<Delegate>());
            }
            var link = _dict[msgId];

            if(link.Length>0 && link.First.Value.GetType()!=callback.GetType())
            {
                Debug.LogError($"回调类型不统一:msgId={msgId},callback={callback}");
                return;
            }
            
            link.Add(weight,callback);
        }

        public void Add<T>(K msgId, Action<T> callback, int weight = 0)
        {
            if(callback==null)Debug.LogError($"回调不能为空:msgId={msgId}");

            if(!_dict.ContainsKey(msgId))
            {
                _dict.Add(msgId,new OrderedLinkedList<Delegate>());
            }
            var link = _dict[msgId];

            if(link.Length>0 && link.First.Value.GetType()!=callback.GetType())
            {
                Debug.LogError($"回调类型不统一:msgId={msgId},callback={callback}");
                return;
            }
            
            link.Add(weight,callback);
        }

        public void Add<T, U>(K msgId, Action<T, U> callback, int weight = 0)
        {
            if(callback==null)Debug.LogError($"回调不能为空:msgId={msgId}");

            if(!_dict.ContainsKey(msgId))
            {
                _dict.Add(msgId,new OrderedLinkedList<Delegate>());
            }
            var link = _dict[msgId];

            if(link.Length>0 && link.First.Value.GetType()!=callback.GetType())
            {
                Debug.LogError($"回调类型不统一:msgId={msgId},callback={callback}");
                return;
            }
            
            link.Add(weight,callback);
        }

        public void Add<T, U, V>(K msgId, Action<T, U, V> callback, int weight = 0)
        {
            if(callback==null)Debug.LogError($"回调不能为空:msgId={msgId}");

            if(!_dict.ContainsKey(msgId))
            {
                _dict.Add(msgId,new OrderedLinkedList<Delegate>());
            }
            var link = _dict[msgId];

            if(link.Length>0 && link.First.Value.GetType()!=callback.GetType())
            {
                Debug.LogError($"回调类型不统一:msgId={msgId},callback={callback}");
                return;
            }
            
            link.Add(weight,callback);
        }

        public void Broadcast(K msgId)
        {
            if(!_dict.ContainsKey(msgId) || _dict[msgId].Length==0)return;

            var link = _dict[msgId];

            if(link.First.Value.GetType()!=typeof(Action))
            {
                Debug.LogError($"回调类型不统一:msgId={msgId},callback={typeof(Action)}");
                return;
            }

            foreach (var node in link)
            {
                (node.Value as Action)();

                if(_aborts.Contains(msgId))
                {
                    _aborts.Remove(msgId);
                    return;
                }
            }
        }

        public void Broadcast<T>(K msgId, T arg1)
        {
            if(!_dict.ContainsKey(msgId) || _dict[msgId].Length==0)return;

            var link = _dict[msgId];

            if(link.First.Value.GetType()!=typeof(Action<T>))
            {
                Debug.LogError($"回调类型不统一:msgId={msgId},callback={typeof(Action<T>)}");
                return;
            }

            foreach (var node in link)
            {
                (node.Value as Action<T>)(arg1);

                if(_aborts.Contains(msgId))
                {
                    _aborts.Remove(msgId);
                    return;
                }
            }
        }

        public void Broadcast<T, U>(K msgId, T arg1, U arg2)
        {
            if(!_dict.ContainsKey(msgId) || _dict[msgId].Length==0)return;

            var link = _dict[msgId];

            if(link.First.Value.GetType()!=typeof(Action<T,U>))
            {
                Debug.LogError($"回调类型不统一:msgId={msgId},callback={typeof(Action<T,U>)}");
                return;
            }
            
            foreach (var node in link)
            {
                (node.Value as Action<T,U>)(arg1,arg2);

                if(_aborts.Contains(msgId))
                {
                    _aborts.Remove(msgId);
                    return;
                }
            }
        }

        public void Broadcast<T, U, V>(K msgId, T arg1, U arg2, V arg3)
        {
            if(!_dict.ContainsKey(msgId) || _dict[msgId].Length==0)return;

            var link = _dict[msgId];

            if(link.First.Value.GetType()!=typeof(Action<T,U,V>))
            {
                Debug.LogError($"回调类型不统一:msgId={msgId},callback={typeof(Action<T,U,V>)}");
                return;
            }
            
            foreach (var node in link)
            {
                (node.Value as Action<T,U,V>)(arg1,arg2,arg3);

                if(_aborts.Contains(msgId))
                {
                    _aborts.Remove(msgId);
                    return;
                }
            }
        }

        public bool Remove(K msgId, Action callback)
        {
            if(callback==null)Debug.LogError($"回调不能为空:msgId={msgId}");

            if(!_dict.ContainsKey(msgId) || _dict[msgId].Length==0)return false;

            var link = _dict[msgId];

            if(link.First.Value.GetType()!=callback.GetType())
            {
                Debug.LogError($"回调类型不统一:msgId={msgId},callback={callback}");
                return false;
            }

            return link.Remove(callback);
        }

        public bool Remove<T>(K msgId, Action<T> callback)
        {
            if(callback==null)Debug.LogError($"回调不能为空:msgId={msgId}");

            if(!_dict.ContainsKey(msgId) || _dict[msgId].Length==0)return false;

            var link = _dict[msgId];

            if(link.First.Value.GetType()!=callback.GetType())
            {
                Debug.LogError($"回调类型不统一:msgId={msgId},callback={callback}");
                return false;
            }

            return link.Remove(callback);
        }

        public bool Remove<T, U>(K msgId, Action<T, U> callback)
        {
            if(callback==null)Debug.LogError($"回调不能为空:msgId={msgId}");

            if(!_dict.ContainsKey(msgId) || _dict[msgId].Length==0)return false;

            var link = _dict[msgId];

            if(link.First.Value.GetType()!=callback.GetType())
            {
                Debug.LogError($"回调类型不统一:msgId={msgId},callback={callback}");
                return false;
            }

            return link.Remove(callback);
        }

        public bool Remove<T, U, V>(K msgId, Action<T, U, V> callback)
        {
            if(callback==null)Debug.LogError($"回调不能为空:msgId={msgId}");

            if(!_dict.ContainsKey(msgId) || _dict[msgId].Length==0)return false;

            var link = _dict[msgId];

            if(link.First.Value.GetType()!=callback.GetType())
            {
                Debug.LogError($"回调类型不统一:msgId={msgId},callback={callback}");
                return false;
            }

            return link.Remove(callback);
        }
    }
}