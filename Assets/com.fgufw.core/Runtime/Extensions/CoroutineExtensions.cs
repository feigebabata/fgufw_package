using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW
{
    public static class CoroutineExtensions
    {
		/// <summary>
		/// 启动协程 需要主动结束协程调用Coroutine.Stop()
		/// </summary>
		/// <param name="self"></param>
		/// <returns></returns>
		public static Coroutine Start(this IEnumerator self)
		{
			return GlobalCoroutineSystem.I.StartCoroutine(self);
		}

		public static void Stop(this Coroutine self)
		{
			GlobalCoroutineSystem.I.StopCoroutine(self);
		}

		public static void StopAll()
		{
			GlobalCoroutineSystem.I.StopAllCoroutines();
		}

		/// <summary>
		/// 文件读写或网络请求用这个 CoroutineCore内部做了并行数量的限制
		/// </summary>
		/// <param name="self"></param>
		/// <returns>返回值可以用在CoroutineCore.I.StopIO(int id) 来结束加载协程</returns>
		// public static int StartIO(this IEnumerator self)
		// {
		// 	return GlobalCoroutineSystem.I.StartIO(self);
		// }

		// public static void Enqueue(this IEnumerator self,int queueID=0)
		// {
		// 	lock(queueDicLock)
		// 	{
		// 		if(runTable==null)
		// 		{
		// 			runTable = new HashSet<int>();
		// 			GlobalAppEventSystem.I.UpdateListener+=Update;
		// 		}
		// 		if(!queueDic.ContainsKey(queueID))
		// 		{
		// 			queueDic.Add(queueID,new Queue<IEnumerator>());
		// 		}
		// 		queueDic[queueID].Enqueue(self);
		// 	}
		// }

		// private static Dictionary<int,Queue<IEnumerator>> queueDic = new Dictionary<int, Queue<IEnumerator>>();
		// private static object queueDicLock = new object();
		// private static HashSet<int> runTable;
		// private static IEnumerator queueCor(IEnumerator cor,int queueID)
		// {
		// 	runTable.Add(queueID);
		// 	yield return cor;
		// 	runTable.Remove(queueID);
		// }

		// private static void Update()
		// {
		// 	int removeKey=int.MinValue;
		// 	foreach (var item in queueDic)
		// 	{
		// 		if(!runTable.Contains(item.Key))
		// 		{
		// 			queueCor(item.Value.Dequeue(),item.Key).Start();
		// 			if(item.Value.Count==0)
		// 			{
		// 				removeKey = item.Key;
		// 			}
		// 		}
		// 	}
		// 	if(removeKey!=int.MinValue)
		// 	{
		// 		queueDic.Remove(removeKey);
		// 	}
		// }

    }    
}