#define SHOW

using System;
using System.Collections;
using System.Collections.Generic;

namespace FGUFW
{
    [Serializable]
    public class Table<TKey,TValue>:IEnumerable<KeyValuePair<TKey, TValue>>
    {
#if UNITY_EDITOR && SHOW
        [Serializable]
        public struct ItemData
        {
            public TKey Key;
            public TValue Value;
        }

        public List<ItemData> Items = new List<ItemData>();
#endif
        private Dictionary<TKey,TValue> _dict = new Dictionary<TKey, TValue>();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        public TValue this[TKey key]
        {
            get
            {
                return _dict[key];
            }
            set
            {
                _dict[key] = value;
            }
        }

        public void Add(TKey key, TValue value)
        {
            _dict.Add(key,value);

            #if UNITY_EDITOR && SHOW
            Items.Add(new ItemData{Key=key,Value=value});
            #endif
        }

        public void Clear()
        {
            _dict.Clear();

            #if UNITY_EDITOR && SHOW
            Items.Clear();
            #endif
        }

        public bool ContainsKey(TKey key)
        {
            return _dict.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            #if UNITY_EDITOR && SHOW
            Items.RemoveSwapBack(item=>item.Key.Equals(key));
            #endif

            return _dict.Remove(key);
        }
    }
}
