using System;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

public sealed class EnemyStats : EntityStats
{
	[Header("Dropped Loots"), Space]
	[SerializeField] private EnemyLootTrigger lootTrigger;

	public string ID { get; private set; }

	private void Awake()
	{
		_mat = this.GetComponentInChildren<SpriteRenderer>("Graphic/Sprite").material;
		ID = Guid.NewGuid().ToString();
	}

	protected override void Start()
	{
		base.Start();
		healthBar.name = $"{gameObject.name} Health Bar";
	}

	protected override void TakeDamage(Stats attackerStats, Vector3 attackerPos, float scaleFactor)
	{
		base.TakeDamage(attackerStats, attackerPos, scaleFactor);

		AudioManager.Instance.PlayWithRandomPitch("Metal Impact", .5f, 8f);
		EffectInstantiator.Instance.Instantiate<ParticleSystem>(EffectType.SolidImpact, rb2D.position, UnityRandom.insideUnitCircle.normalized);
	}

	public override void Die()
	{
		EffectInstantiator.Instance.Instantiate<ParticleSystem>(EffectType.Explosion, transform.position, Quaternion.identity);
		AudioManager.Instance.Play("Explosion");
		CameraShaker.Instance.ShakeCamera(4f, .2f);

		lootTrigger.DispenseLoots();

		DestroyGameObject();
	}

	public void DestroyGameObject()
	{
		Destroy(healthBar.gameObject);
		Destroy(gameObject);
	}
}
