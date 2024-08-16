using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private Transform container;
	
	[Header("Enemy Prefabs"), Space]
	[SerializeField] private List<GameObject> enemyPrefabs;

	[Header("Spawn Settings"), Space]
	[SerializeField] private float initialDelay;
	[SerializeField] private float spawnDelay;
	[SerializeField] private float offscreenOffset;

	// Private fields.
	private float _delay;
	private float _cameraExtent;

	private void Start()
	{
		_delay = initialDelay;
		_cameraExtent = Camera.main.orthographicSize;
	}

	private void Update()
	{
		_delay -= Time.deltaTime * TimeManager.LocalTimeScale;
		if (_delay <= 0f)
		{
			Spawn();
		}
	}

	private void Spawn()
	{
		float spawnExtent = _cameraExtent * Screen.width / Screen.height + offscreenOffset;
		
		float x = spawnExtent * Mathf.Sign(Random.value * 2f - 1f);
		float y = spawnExtent * Mathf.Sign(Random.value * 2f - 1f);
		int index = Random.Range(0, enemyPrefabs.Count);

		GameObject enemy = Instantiate(enemyPrefabs[index], new Vector2(x, y), Quaternion.identity);
		enemy.transform.SetParent(container);

		_delay = spawnDelay;
	}
}