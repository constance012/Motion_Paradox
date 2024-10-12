using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spawn Data", menuName = "Data/Enemy Spawn Data")]
public sealed class EnemyRuntimeData : ScriptableObject
{
	[Header("Type"), Space]
	public EnemyType type;
	public GameObject prefab;

	[Header("Spawn Data"), Space]
	public Vector2 spawnLimitRange;

	[Header("Loot Table"), Space]
	public SerializedDictionary<LootType, LootInfo> lootTable;

	public float SpawnChance => _spawnChance;

	// Private fields.
	private float _spawnChance;

	public void Reset()
	{
		_spawnChance = spawnLimitRange.x;
	}

	public void UpdateSpawnChance(float curveValue)
	{
		_spawnChance = spawnLimitRange.Interpolate(curveValue);
	}
}

public enum EnemyType
{
	Pawn,
	Suppressor,
	Scout,
	Interceptor,
	Destroyer
}

public enum LootType
{
	HealingHeart,
	MetalScrap,
	Experience
}

[Serializable]
public struct LootInfo
{
	public float chance;
	public Vector2Int quantityRange;
}