using System.Collections.Generic;
using UnityEngine;
using CSTGames.Utility;

public class EnemySpawner : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private Transform container;
	
	[Header("Enemy Prefabs"), Space]
	[SerializeField] private List<GameObject> enemyPrefabs;
	[SerializeField, Min(1)] private int maxEnemies;

	[Header("Spawn Area"), Space]
	[SerializeField] private Vector2 spawnArea;
	[SerializeField] private float areaExtent;

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
			float startX = spawnArea.x * RandomUtils.RandomSign;
			float startY = spawnArea.y * RandomUtils.RandomSign;

			float x = Random.Range(startX, startX + areaExtent * Mathf.Sign(startX));
			float y = Random.Range(startY, startY + areaExtent * Mathf.Sign(startY));

			int index = Random.Range(0, enemyPrefabs.Count);

			GameObject enemy = Instantiate(enemyPrefabs[index], new Vector2(x, y), Quaternion.identity);
			enemy.transform.SetParent(container);

			_delay = spawnDelay;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;

		Gizmos.DrawWireCube(transform.position, spawnArea * 2f);
		Gizmos.DrawWireCube(transform.position, spawnArea * 2f + areaExtent * 2f * Vector2.one);
	}
}