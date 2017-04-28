using UnityEngine;
using System.Collections;

namespace GiantSDK.Voice{
	
	public   class GiantVoiceSingletonForMono<T> : MonoBehaviour where T : GiantVoiceSingletonForMono<T>
{
	private static T _instance = null;
	private static bool _isDestroy = false;

	static GiantVoiceSingletonForMono()
	{
		_isDestroy = false;
	}

	public void OnDestroy()
	{
		_isDestroy = true;
	}

	public static T Instance
	{
		get
		{
			if (_isDestroy)
			{
				return null;
			}

			if (null == _instance)
			{
				_instance = CreateInstance();
			}

			return _instance;
		}
	}

	private static T CreateInstance()
	{
		//Debug.Log(" CreateInstance "+ typeof(T).ToString());

		T inst = (T)UnityEngine.Object.FindObjectOfType(typeof(T));
		if (UnityEngine.Object.FindObjectsOfType(typeof(T)).Length > 1)
		{
			Debug.LogError("Singleton error.multi-object");
		}

		if (null == inst)
		{
			GameObject root = GameObject.Find("Singleton");
			if (root == null)
			{
				root = new GameObject("Singleton");
				//root.transform.Reset ();
				GameObject.DontDestroyOnLoad(root);
			}

			GameObject obj = new GameObject(typeof(T).ToString());
			GameObject.DontDestroyOnLoad(obj);
			obj.transform.parent = root.transform;
			//obj.transform.Reset();
			inst = obj.AddComponent<T>();
		}

		inst.Initialise();
		return inst;
	}

	public static void Register()
	{
		if (_instance == null)
		{
			_instance = CreateInstance();
		}
	}

	public virtual void Initialise()
	{
	} 
}

}