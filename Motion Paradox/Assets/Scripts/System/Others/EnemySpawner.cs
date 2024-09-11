using UnityEngine;
using AYellowpaper.SerializedCollections;

public class EnemySpawner : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private Transform container;
	
	[Header("Enemy Prefabs"), Space]
	[SerializeField] private SerializedDictionary<EnemyType, GameObject> enemyPrefabs;

	[Header("Spawn Chances"), Space]
	[SerializeField] private SerializedDictionary<EnemyType, float> enemySpawnInfo;
	[SerializeField, Min(1)] private int maxEnemies;

	[Header("Spawn Radius"), Space]
	[SerializeField] private float spawnRadius;

	[Header("Spawn Delay"), Space]
	[SerializeField] private float initialDelay;
	[SerializeField] private float spawnDelay;

	// Private fields.
	private float _delay;

	private void Start()
	{
		_delay = initialDelay;
		
		#if UNITY_EDITOR
			_delay = spawnDelay;
		#endif

		GameManager.Instance.onGameVictory += (sender, e) => DestroyAllEnemies();
	}

	private void Update()
	{
		_delay -= Time.deltaTime * TimeManager.LocalTimeScale;

		if (_delay <= 0f)
		{
			TrySpawnEnemies();
		}
	}

	private void DestroyAllEnemies()
	{
		foreach (Transform enemy in container)
		{
			enemy.GetComponent<EnemyStats>().DestroyGameObject();
		}

		gameObject.SetActive(false);
	}

	private void TrySpawnEnemies()
	{
		float spawnChance = Random.value;

		foreach (var enemyInfo in enemySpawnInfo)
		{
			if (spawnChance <= enemyInfo.Value && container.childCount < maxEnemies)
			{
				Vector2 spawnPos = Random.insideUnitCircle.normalized * spawnRadius;

				GameObject enemy = Instantiate(enemyPrefabs[enemyInfo.Key], spawnPos, Quaternion.identity);
				enemy.transform.SetParent(container);
			}
		}
		
		_delay = spawnDelay;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, spawnRadius);
	}

	enum EnemyType
	{
		Pawn,
		Suppression,
		Scout,
		Interceptor,
		Destroyer
	}
}