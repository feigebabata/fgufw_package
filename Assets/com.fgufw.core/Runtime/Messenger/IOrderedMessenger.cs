using System;

namespace FGUFW
{
    /// <summary>
    /// 有序广播 可以打断消息调用
    /// </summary>
    public interface IOrderedMessenger<K>
    {
		void Add(K msgID,Action callback,int weight=0);
		void Add<T>(K msgID,Action<T> callback,int weight=0);
		void Add<T,U>(K msgID,Action<T,U> callback,int weight=0);
		void Add<T,U,V>(K msgID,Action<T,U,V> callback,int weight=0);

		bool Remove(K msgID,Action callback);
		bool Remove<T>(K msgID,Action<T> callback);
		bool Remove<T,U>(K msgID,Action<T,U> callback);
		bool Remove<T,U,V>(K msgID,Action<T,U,V> callback);

		void Broadcast(K msgID);
		void Broadcast<T>(K msgID,T arg1);
		void Broadcast<T,U>(K msgID,T arg1,U arg2);
		void Broadcast<T,U,V>(K msgID,T arg1,U arg2,V arg3);

        void Abort(K msgID);
    }
}