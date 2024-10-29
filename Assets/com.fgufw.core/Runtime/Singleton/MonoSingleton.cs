using UnityEngine;

namespace FGUFW
{
	public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
	{
		private static T instance = null;

		public static T I
		{
			get
			{
				if (instance == null)
				{
					instance = GameObject.FindObjectOfType(typeof(T)) as T;
					if (instance == null)
					{
						GameObject go = new GameObject(typeof(T).Name);
						instance = go.AddComponent<T>();
					}
				}

				return instance;
			}
		}


		private void Awake()
		{
			if (instance == null)
			{
				instance = this as T;

				if(IsDontDestroyOnLoad())
				{
					DontDestroyOnLoad(gameObject);
				}

				Init();
			}
			else
			{
				Debug.LogError("mono单例重复");
			}
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		void OnDestroy()
		{
			Dispose();
			MonoSingleton<T>.instance = null;
		}
	
		protected virtual void Init()
		{

		}

		public void DestroySelf()
		{
			UnityEngine.Object.Destroy(gameObject);
		}

		public virtual void Dispose()
		{

		}

		protected abstract bool IsDontDestroyOnLoad();

		public static bool NotNull()
		{
			return instance!=null;
		}

	}
}