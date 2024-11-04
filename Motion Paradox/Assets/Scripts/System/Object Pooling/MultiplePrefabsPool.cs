using System;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using CSTGames.Utility;

public abstract class MultiplePrefabsPool<TSingleton, TType, TPoolable> : Singleton<TSingleton>
															where TType : Enum
															where TPoolable : MonoBehaviour, IPoolable
															where TSingleton : MultiplePrefabsPool<TSingleton, TType, TPoolable>
{
	[Header("Singleton Settings"), Space]
	[SerializeField] protected bool markAsSingleton;

	[Header("Pool Settings"), Space]
	[SerializeField] protected SerializedDictionary<TType, PoolInfo> poolSettings;
	[SerializeField] protected Transform sharedParent;

	// Protected fields.
	protected Dictionary<TType, ObjectPool<TPoolable>> _pools;

	protected override void Awake()
	{
		if (markAsSingleton)
			base.Awake();

		PrefillPools();
	}

	#region Spawn Overloads.
	public TPoolable Spawn(TType type)
	{
		return _pools[type].Spawn();
	}

	public TPoolable Spawn(TType type, Func<TPoolable, bool> predicate)
	{
		return _pools[type].Spawn(predicate);
	}

	public TPoolable Spawn(TType type, Vector3 position, Quaternion rotation, bool isLocal = false)
	{
		return _pools[type].Spawn(position, rotation, isLocal);
	}

	public TPoolable Spawn(TType type, Vector3 position, Quaternion rotation, Func<TPoolable, bool> predicate, bool isLocal = false)
	{
		return _pools[type].Spawn(position, rotation, predicate, isLocal);
	}
	#endregion

	protected virtual void PrefillPools()
	{
		_pools ??= new Dictionary<TType, ObjectPool<TPoolable>>();

		foreach (var pair in poolSettings)
		{
			PoolInfo info = pair.Value;
			Transform parent = new GameObject(pair.Key.ToString().AddWhitespaceBeforeCapital() + "s").transform;
			parent.SetParent(sharedParent);

			_pools[pair.Key] = new ObjectPool<TPoolable>(info.prefab, info.prefillAmount, parent);
		}
	}

	[Serializable]
	public struct PoolInfo
	{
		public GameObject prefab;
		public int prefillAmount;
	}
}