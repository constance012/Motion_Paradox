using System;
using UnityEngine;
using UnityEngine.Events;
using UnityRandom = UnityEngine.Random;

public sealed class EnemyStats : EntityStats, IPoolable
{
	[Header("Dropped Loots"), Space]
	[SerializeField] private EnemyLootTrigger lootTrigger;

	[Header("Events"), Space]
	public UnityEvent onDied;

	public string ID { get; private set; }

	private void Awake()
	{
		_mat = this.GetComponentInChildren<SpriteRenderer>("Graphic/Sprite").material;
		ID = Guid.NewGuid().ToString();
	}

	protected override void Start()
	{
		Allocate();
		healthBar.name = $"{gameObject.name} Health Bar";
	}

	public void Allocate()
	{
		gameObject.SetActive(true);
		healthBar.gameObject.SetActive(true);

		stats.ClearUpgrades();
		_currentHealth = stats.GetDynamicStat(Stat.MaxHealth);
		healthBar.SetMaxHealth(_currentHealth);
	}

	public void Deallocate()
	{
		onDied?.Invoke();

		StopAllCoroutines();
		_mat.SetFloat("_FlashIntensity", 0f);
		movementScript.enabled = true;

		healthBar.gameObject.SetActive(false);
		gameObject.SetActive(false);
	}

	public override void TakeDamage(Stats attackerStats, Vector3 attackerPos, float scaleFactor)
	{
		base.TakeDamage(attackerStats, attackerPos, scaleFactor);

		AudioManager.Instance.PlayWithRandomPitch("Metal Impact", .5f, 8f);
		EffectPool.Instance.Spawn(EffectType.SolidImpact, rb2D.position, UnityRandom.insideUnitCircle.normalized);
	}

	public override void Die()
	{
		EffectPool.Instance.Spawn(EffectType.Explosion, transform.position, Quaternion.identity);
		AudioManager.Instance.Play("Explosion");
		CameraShaker.Instance.ShakeCamera(4f, .2f);

		lootTrigger.DispenseLoots();

		Deallocate();
	}
}
