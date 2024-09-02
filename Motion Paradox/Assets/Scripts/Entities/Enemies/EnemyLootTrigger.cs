using UnityEngine;
using AYellowpaper.SerializedCollections;

public sealed class EnemyLootTrigger : MonoBehaviour
{
	[Header("Loot Table"), Space]
	[SerializeField] private SerializedDictionary<LootType, float> lootTable;

	public void DispenseLoots()
	{
		EnemyLootDispenser.Instance.Dispense(lootTable, transform.position);
	}
}