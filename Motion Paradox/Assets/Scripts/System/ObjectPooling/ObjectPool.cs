using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class ObjectPool<TObject> where TObject : MonoBehaviour, IPoolable
{
	private List<TObject> _pool;
	private GameObject _prefab;
	private Transform _parent;

	public ObjectPool(Transform parent = null)
	{
		_pool = new List<TObject>();
		_parent = parent;
	}

	public ObjectPool(GameObject prefab, int prefillAmount, Transform parent = null)
	{
		_pool = new List<TObject>();
		_prefab = prefab;
		_parent = parent;

		for (int i = 0; i < prefillAmount; i++)
		{
			Prefill(_prefab);
		}
	}

	#region Spawn Overloads.
	public TObject Spawn()
	{
		return TrySpawnObject(obj => !obj.gameObject.activeInHierarchy);
	}

	public TObject Spawn(Func<TObject, bool> predicate)
	{
		return TrySpawnObject(predicate);
	}

	public TObject Spawn(Vector3 position, Quaternion rotation, bool isLocal = false)
	{
		TObject newObject = TrySpawnObject(obj => !obj.gameObject.activeInHierarchy);

		if (!isLocal)
			newObject.transform.SetPositionAndRotation(position, rotation);
		else
			newObject.transform.SetLocalPositionAndRotation(position, rotation);

		return newObject;
	}

	public TObject Spawn(Vector3 position, Quaternion rotation, Func<TObject, bool> predicate, bool isLocal = false)
	{
		TObject newObject = TrySpawnObject(predicate);

		if (!isLocal)
			newObject.transform.SetPositionAndRotation(position, rotation);
		else
			newObject.transform.SetLocalPositionAndRotation(position, rotation);

		return newObject;
	}
	#endregion

	public TObject Prefill(GameObject prefab)
	{
		TObject obj = GameObject.Instantiate(prefab, _parent).GetComponent<TObject>();
		obj.Deallocate();
		_pool.Add(obj);

		return obj;
	}

	private TObject TrySpawnObject(Func<TObject, bool> predicate)
	{
		TObject newObject;
		
		if (_pool.Count == 0)
		{
			Debug.LogWarning($"The pool of {typeof(TObject)} is empty, spawning a new one.");
			newObject = GameObject.Instantiate(_prefab, _parent).GetComponent<TObject>();
			newObject.Allocate();
		}
		else
		{
			newObject = _pool.First(predicate);
			newObject.Allocate();
		}
		
		return newObject;
	}
}