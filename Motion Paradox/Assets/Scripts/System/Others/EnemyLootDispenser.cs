using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;

public sealed class EnemyLootDispenser : Singleton<EnemyLootDispenser>
{
	[Header("Loot Pool"), Space]
	[SerializeField] private SerializedDictionary<LootType, GameObject> lootPrefabs;

	[Header("Loot Container"), Space]
	[SerializeField] private Transform container;

	public void Dispense(IEnumerable<KeyValuePair<LootType, LootInfo>> lootTable, Vector2 position)
	{
		foreach (var loot in lootTable)
		{
			LootInfo info = loot.Value;
			if (Random.value <= info.chance)
			{
				if (lootPrefabs.TryGetValue(loot.Key, out GameObject prefab))
				{
					Debug.Log($"Dropping {prefab.name}...");
					GameObject lootObject = Instantiate(prefab, position + Random.insideUnitCircle.normalized, Quaternion.identity);
					lootObject.transform.SetParent(container);
					lootObject.GetComponent<PickableItem>().ItemQuantity = info.quantityRange.RandomBetweenEnds();
				}
			}
		}
	}
}