using System;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using CSTGames.Utility;

public sealed class ProjectilePool : Singleton<ProjectilePool>
{
	[Header("Pool Settings"), Space]
	[SerializeField] private SerializedDictionary<ProjectileType, PoolInfo> poolSettings;
	[SerializeField] private Transform sharedParent;

	// Private fields.
	private Dictionary<ProjectileType, ObjectPool<ProjectileBase>> _pools;

	protected override void Awake()
	{
		base.Awake();
		PrefillPools();
	}

	#region Spawn Overloads.
	public ProjectileBase Spawn(ProjectileType type)
	{
		return _pools[type].Spawn();
	}

	public ProjectileBase Spawn(ProjectileType type, Func<ProjectileBase, bool> predicate)
	{
		return _pools[type].Spawn(predicate);
	}

	public ProjectileBase Spawn(ProjectileType type, Vector3 position, Quaternion rotation, bool isLocal = false)
	{
		return _pools[type].Spawn(position, rotation, isLocal);
	}

	public ProjectileBase Spawn(ProjectileType type, Vector3 position, Quaternion rotation, Func<ProjectileBase, bool> predicate, bool isLocal = false)
	{
		return _pools[type].Spawn(position, rotation, predicate, isLocal);
	}
	#endregion

	private void PrefillPools()
	{
		_pools ??= new Dictionary<ProjectileType, ObjectPool<ProjectileBase>>();

		foreach (var pair in poolSettings)
		{
			PoolInfo info = pair.Value;
			Transform parent = new GameObject(pair.Key.ToString().AddWhitespaceBeforeCapital() + "s").transform;
			parent.SetParent(sharedParent);

			_pools[pair.Key] = new ObjectPool<ProjectileBase>(info.prefab, info.prefillAmount, parent);
		}
	}

	[Serializable]
	public struct PoolInfo
	{
		public GameObject prefab;
		public int prefillAmount;
	}
}

public enum ProjectileType
{
	NormalBullet,
	PiercingBullet,
	MetalPellet,
	MetalShard,
	MetalMissile
}