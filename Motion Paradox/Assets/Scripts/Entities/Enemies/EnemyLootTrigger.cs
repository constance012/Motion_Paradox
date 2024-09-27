using UnityEngine;

public sealed class EnemyLootTrigger : MonoBehaviour
{
	[Header("Runtime Data"), Space]
	[SerializeField] private EnemyRuntimeData data;

	public void DispenseLoots()
	{
		EnemyLootDispenser.Instance.Dispense(data.lootTable, (Vector2)transform.position);
	}
}