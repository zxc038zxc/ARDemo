using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitySpawnPool : MonoBehaviour
{
	[SerializeField]
	private int _maxCacheCount;

	private  Dictionary<int, Queue<GameObject>> _poolTable = new Dictionary<int, Queue<GameObject>>(50);
	private Dictionary<GameObject, int> _tagTable = new Dictionary<GameObject, int>(50);

	private static UnitySpawnPool _instance;
	public static UnitySpawnPool Instance
	{
		get
		{
			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;
	}

	private void OnDestroy()
	{
		if (_instance != null)
		{
			_instance = null;
		}
	}

	public GameObject SpawnGameObj(GameObject sample)
	{
		int tag = sample.GetHashCode();
		if (!_poolTable.ContainsKey(tag))
		{
			_poolTable[tag] = new Queue<GameObject>();
		}

		GameObject obj;
		if (_poolTable[tag].Count > 0)
		{
			obj = _poolTable[tag].Dequeue();
		}
		else
		{
			obj = Instantiate(sample).gameObject;
		}

		_tagTable.Add(obj, tag);
		return obj;
	}

	public T SpawnGameObj<T>(T sample) where T : Component
	{
		GameObject obj = SpawnGameObj(sample.gameObject);
		return obj.GetComponent<T>();
	}

	public void Despawn(GameObject target)
	{
		if (_instance == null) return;

		if (!_tagTable.ContainsKey(target))
		{
			Debug.LogError($"[ObjectPool.DespawnGameObj] Cannot find tag as key, ObjName: {target.name}");
			return;
		}

		int tag = _tagTable[target];
		_tagTable.Remove(target);

		if (!_poolTable.ContainsKey(tag))
		{
			_poolTable[tag] = new Queue<GameObject>();
		}

		if (_poolTable[tag].Count < _maxCacheCount)
		{
			ResetTransform(target.transform);
			target.SetActive(false);
			_poolTable[tag].Enqueue(target);
		}
		else
		{
			Destroy(target);
		}
	}

	private void ResetTransform(Transform trans)
	{
		trans.localPosition = Vector3.zero;
		trans.localRotation = Quaternion.identity;
		trans.localScale = Vector3.one;
	}
}
