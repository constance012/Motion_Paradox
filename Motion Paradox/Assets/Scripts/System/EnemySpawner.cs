using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private Transform container;
	
	[Header("Enemy Prefabs"), Space]
	[SerializeField] private List<GameObject> enemyPrefabs;
	[SerializeField, Min(1)] private int maxEnemies;

	[Header("Spawn Area"), Space]
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
		if (transform.childCount < maxEnemies)
		{
			Vector2 spawnPos = Random.insideUnitCircle.normalized * spawnRadius;

			GameObject enemy = Instantiate(SelectRandomEnemy(), spawnPos, Quaternion.identity);
			enemy.transform.SetParent(container);

			_delay = spawnDelay;
		}
	}

	private GameObject SelectRandomEnemy()
	{
		int index = Random.Range(0, enemyPrefabs.Count);
		return enemyPrefabs[index];
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, spawnRadius);
	}
}