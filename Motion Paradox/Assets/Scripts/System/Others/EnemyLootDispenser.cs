using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;

public sealed class EnemyLootDispenser : Singleton<EnemyLootDispenser>
{
	[Header("Loot Pool"), Space]
	[SerializeField] private SerializedDictionary<LootType, Item> lootScriptableObjects;

	[Header("Pool Settings"), Space]
	[SerializeField] private GameObject prefab;
	[SerializeField] private Transform container;
	[SerializeField] private int prefillAmount;

	// Private fields.
	private ObjectPool<PickableItem> _pool;

	protected override void Awake()
	{
		base.Awake();
		_pool = new ObjectPool<PickableItem>(prefab, prefillAmount, container);
	}

	public void Dispense(IEnumerable<KeyValuePair<LootType, LootInfo>> lootTable, Vector2 position)
	{
		foreach (var loot in lootTable)
		{
			LootInfo info = loot.Value;
			if (Random.value <= info.chance)
			{
				if (lootScriptableObjects.TryGetValue(loot.Key, out Item itemSO))
				{
					Debug.Log($"Dropping {itemSO.name}...");

					PickableItem droppedItem = _pool.Spawn(position + Random.insideUnitCircle.normalized, Quaternion.identity);
					droppedItem.Initialize(itemSO, info.quantityRange.RandomBetweenEnds());
				}
			}
		}
	}
}