using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class ObjectPool<TObject> where TObject : MonoBehaviour, IPoolable
{
	private HashSet<TObject> _pool;
	private GameObject _prefab;
	private Transform _parent;

	public ObjectPool(Transform parent = null)
	{
		_pool = new HashSet<TObject>();
		_parent = parent;
	}

	public ObjectPool(GameObject prefab, int prefillAmount, Transform parent = null)
	{
		_pool = new HashSet<TObject>();
		_prefab = prefab;
		_parent = parent;

		Prefill(prefab, prefillAmount);
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

	#region Control Methods.
	public void Empty()
	{
		_pool.Clear();
	}

	public TObject AddToPool(GameObject prefab)
	{
		TObject obj = GameObject.Instantiate(prefab, _parent).GetComponent<TObject>();
		obj.name = prefab.name.TrimStart('_');
		obj.Deallocate();
		_pool.Add(obj);

		return obj;
	}

	public void Prefill(GameObject prefab, int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			AddToPool(prefab);
		}
	}
	#endregion

	private TObject TrySpawnObject(Func<TObject, bool> predicate)
	{
		TObject newObject = null;

		if (_pool.Count == 0)
		{
			Debug.LogWarning($"The pool of {typeof(TObject)} is empty, spawning a new one.");
			newObject = AddToPool(_prefab);
		}
		else
		{
			try
			{
				newObject = _pool.First(predicate);
				newObject.Allocate();
			}
			catch (InvalidOperationException)
			{
				Debug.LogWarning($"No available object found in the pool of {typeof(TObject)}. Spawning a new one.");
				newObject = AddToPool(_prefab);
			}
		}
		
		return newObject;
	}
}