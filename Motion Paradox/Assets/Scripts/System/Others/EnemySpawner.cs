using System.Collections.Generic;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private Transform container;
	
	[Header("Enemy Runtime Data"), Space]
	[SerializeField] private List<EnemyRuntimeData> enemyRuntimeData;
	[SerializeField, Min(1)] private int maxEnemies;

	[Header("Spawn Radius"), Space]
	[SerializeField] private float spawnRadius;

	[Header("Spawn Delay"), Space]
	[SerializeField] private Vector2 spawnDelayRange;

	// Private fields.
	private float _maxDelay;
	private float _delay;

	private void Start()
	{		
		_maxDelay = spawnDelayRange.x;
		_delay = _maxDelay;

		enemyRuntimeData.ForEach(data => data.Reset());
		
		GameManager.Instance.OnGameVictory += (sender, e) => DestroyAllEnemies();
		DifficultyScaler.Instance.OnNextDifficultyReached += NextDifficulty_Reached;
	}

	private void Update()
	{
		_delay -= Time.deltaTime * TimeManager.LocalTimeScale;

		if (_delay <= 0f)
		{
			TrySpawnEnemies();
		}
	}

	private void NextDifficulty_Reached(object sender, DifficultyReachedEventArgs e)
	{
		enemyRuntimeData.ForEach(data => data.UpdateSpawnChance(e.curveValue));
		_maxDelay = spawnDelayRange.Interpolate(e.curveValue);
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
		float spawnChance = UnityRandom.value;

		foreach (var data in enemyRuntimeData)
		{
			if (spawnChance <= data.SpawnChance && container.childCount < maxEnemies)
			{
				Vector2 spawnPos = UnityRandom.insideUnitCircle.normalized * spawnRadius;

				GameObject enemy = Instantiate(data.prefab, spawnPos, Quaternion.identity);
				enemy.transform.SetParent(container);
			}
		}
		
		_delay = _maxDelay;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, spawnRadius);
	}
}