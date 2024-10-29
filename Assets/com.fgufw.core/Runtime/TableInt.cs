using System;
using System.Collections;
using System.Text;

namespace FGUFW
{
    /// <summary>
    /// 整型表
    /// </summary>
    public sealed class TableInt<V>
    {
        public struct Entry
        {
            public int Key;
            public int Next;
            public V Value;
        }

        private const int LINK_MAX_LENGTH = 128;

        private int[] _links;
        private Entry[] _entrys;
        private int _count;

        public int Count => _count;

        public V this[int key]
        {
            get => getVal(key);
            set => setVal(key,value);
        }

        private TableInt(){}

        public TableInt(int capacity=3)
        {
            init(Math.Max(capacity,3));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _count; i++)
            {
                sb.AppendLine($"{_entrys[i].Key} : {_entrys[i].Value}");
            }
            return sb.ToString();
        }

        private void init(int capacity)
        {
            var length = TableHelper.GetPrime(capacity);
            _links = new int[length];
            for (int i = 0; i < length; i++)_links[i]=-1;

            _entrys = new Entry[length];
        }

        private void setVal(int key, V value)
        {
            var entry = new Entry
            {
                Key = key,
                Value =value,
                Next =-1
            };

            int linkIdx = key.RoundIndex(_links.Length);
            int linkLength = 0;
            int idx = -1;

            for (int i = _links[linkIdx]; i >=0 ; i = _entrys[i].Next)
            {
                linkLength++;
                if(_entrys[i].Key==key)
                {
                    idx = i;
                    break;
                }
            }

            if(idx==-1)
            {
                entry.Next = _links[linkIdx];
                _links[linkIdx] = _count;
                _entrys[_count] = entry;
                _count++;
            }
            else
            {
                _entrys[idx].Value = value;
            }

            if(linkLength>LINK_MAX_LENGTH || _count>=_entrys.Length)
            {
                expandCapacity();
            }
        }

        private void expandCapacity()
        {
            var length = TableHelper.GetPrime(_entrys.Length*2);
            _links = new int[length];
            for (int i = 0; i < length; i++)_links[i]=-1;
            var newEntrys = new Entry[length];
            Array.Copy(_entrys,newEntrys,_count);
            for (int i = 0; i < _count; i++)
            {
                int linkIdx = newEntrys[i].Key.RoundIndex(length);
                newEntrys[i].Next = _links[linkIdx];
                _links[linkIdx] = i;
            }
            _entrys = newEntrys;
        }

        private V getVal(int key)
        {
            int index = findEntry(key);
            if(index==-1)
            {
                throw new Exception($"找不到key:{key}");
            }
            return _entrys[index].Value;
        }

        private int findEntry(int key)
        {
            var linkIdx = key.RoundIndex(_links.Length);
            for (int i = _links[linkIdx]; i >=0 ; i = _entrys[i].Next)
            {
                if(_entrys[i].Key==key)return i;
            }
            return -1;
        }

        public bool Remove(int key)
        {
            var linkIdx = key.RoundIndex(_links.Length);
            for (int i = _links[linkIdx],prev=-1; i >=0 ;prev=i, i = _entrys[i].Next)
            {
                if(_entrys[i].Key==key)
                {
                    if(prev==-1)
                    {
                        _links[linkIdx]=_entrys[i].Next;
                    }
                    else
                    {
                        _entrys[prev].Next=_entrys[i].Next;
                    }
                    _count--;

                    //最后一个元素替换被删除的 没有使用空闲链表的方案
                    key = _entrys[_count].Key;
                    linkIdx = key.RoundIndex(_links.Length);
                    _entrys[i] = _entrys[_count];
                    for (int j = _links[linkIdx],prev_j=-1; j >=0 ;prev_j =j, j = _entrys[j].Next)
                    {
                        if(_entrys[j].Key==key)
                        {
                            if(prev_j==-1)
                            {
                                _links[linkIdx] = i;
                            }
                            else
                            {
                                _entrys[prev_j].Next = i;
                            }
                        }
                    }
                    return true;
                }   
            }
            return false;
        }

        public void Clear()
        {
            int length = _links.Length;
            for (int i = 0; i < length; i++)_links[i]=-1;
            _count=0;
        }

        public void Foreach(Action<int,V> callback)
        {
            if(callback==null)return;
            for (int i = 0; i < _count; i++)
            {
                var entry = _entrys[i];
                callback(entry.Key,entry.Value);
            }
        }

    }
}