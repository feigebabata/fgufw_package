using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace FGUFW
{
    public class OrderedLinkedList<T>:IEnumerable<OrderedLinkedNode<T>>,ICloneable
    {
        private OrderedLinkedNode<T> _first;

        public int Length{get;private set;}

        public OrderedLinkedNode<T> First => _first;

        public void Add(int weight,T val)
        {
            Length++;
            var newNode = new OrderedLinkedNode<T>(){Weight=weight,Value=val};
            if(_first==null)
            {
                _first = newNode;
                return;
            }

            OrderedLinkedNode<T> previous = null;
            for (var node = _first; node!=null; node=node.Next)
            {
                if(newNode.Weight>node.Weight)
                {
                    newNode.Next = node;
                    if(previous==null)
                    {
                        _first = newNode;
                    }
                    else
                    {
                        previous.Next = newNode;
                    }
                    return;
                }
                previous = node;
            }
            previous.Next = newNode;
        }

        public bool Remove(T val)
        {
            OrderedLinkedNode<T> previous=null;
            for (var node = _first; node!=null; node=node.Next)
            {
                if(val.Equals(node.Value))
                {
                    if(previous==null)
                    {
                        _first = node.Next;
                    }
                    else
                    {
                        previous.Next = node.Next;
                    }
                    node.Next = null;
                    Length--;
                    return true;
                }
                previous = node;
            }
            return false;
        }

        public void Dispose()
        {
            _first = null;
        }

        public bool Contains(T val)
        {
            for (var node = _first; node!=null; node=node.Next)
            {
                if(val.Equals(node.Value))
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerator<OrderedLinkedNode<T>> GetEnumerator()
        {
            for (var node = (OrderedLinkedNode<T>)this.Clone(); node!=null; node=node.Next)
            {
                yield return node;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (var node = (OrderedLinkedNode<T>)this.Clone(); node!=null; node=node.Next)
            {
                yield return node;
            }
        }

        public object Clone()
        {
            if(_first==null)return null;
            var f = new OrderedLinkedNode<T>();
            f.Weight = _first.Weight;
            f.Value = _first.Value;
            var n = f;
            var node = _first;
            while (node.Next!=null)
            {
                n.Next = new OrderedLinkedNode<T>();
                n.Next.Weight = node.Next.Weight;
                n.Next.Value = node.Next.Value;
                
                node = node.Next;
                n = n.Next;
            }
            return f;
        }

        #region IEnumerable

        #endregion
    }

    public class OrderedLinkedNode<T>
    {
        public int Weight;

        public T Value;

        public OrderedLinkedNode<T> Next;

        public override string ToString()
        {
            return $"{Weight} : {Value.ToString()}";
        }
    }
}