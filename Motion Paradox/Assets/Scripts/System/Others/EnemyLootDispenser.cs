using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;

public sealed class EnemyLootDispenser : Singleton<EnemyLootDispenser>
{
	[Header("Loot Pool"), Space]
	[SerializeField] private SerializedDictionary<LootType, GameObject> lootPrefabs;

	[Header("Loot Container"), Space]
	[SerializeField] private Transform container;

	public void Dispense(IEnumerable<KeyValuePair<LootType, float>> lootTable, Vector3 position)
	{
		foreach (var loot in lootTable)
		{
			if (Random.value <= loot.Value)
			{
				if (lootPrefabs.TryGetValue(loot.Key, out GameObject prefab))
				{
					GameObject lootObject = Instantiate(prefab, position, Quaternion.identity);
					lootObject.transform.SetParent(container);
				}
			}
		}
	}
}

public enum LootType
{
	HealingHeart,
	Coin,
	Material
}