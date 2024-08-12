using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static bool _shuttingDown = false;
	private static object _lock = new object();
	private static T _instance;

	public static T Instance
	{
		get
		{
			if(_shuttingDown)
			{
				Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed, Return null.");
				return null;
			}

			lock(_lock				)
			{
				if(_instance == null)
				{
					_instance = (T)FindObjectOfType(typeof(T));

					if( _instance == null )
					{
						var singletonObj = new GameObject();
						_instance = singletonObj.AddComponent<T>();
						singletonObj.name = $"{typeof(T).ToString()} (Singleton)";

						DontDestroyOnLoad(singletonObj);
					}
				}
			}

			return _instance;
		}
	}

	private void OnApplicationQuit()
	{
		_shuttingDown = true;
	}

	private void OnDestroy()
	{
		_shuttingDown = true;
	}
}
