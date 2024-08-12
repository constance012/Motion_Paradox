using System;
using UnityEngine;

public class EnemyStats : Entity
{
	public string ID { get; private set; }

	[Header("Attack Filter"), Space]
	[SerializeField] private Vector2 attackRange;
	[SerializeField] private LayerMask hitLayer;

	[Header("Health Bar"), Space]
	[SerializeField] private WorldHealthBar healthBar;

	private void Awake()
	{
		_mat = this.GetComponentInChildren<SpriteRenderer>("Graphics").material;
		ID = Guid.NewGuid().ToString();
	}

	protected override void Start()
	{
		base.Start();

		healthBar.SetMaxHealth(stats.GetDynamicStat(Stat.MaxHealth));
		healthBar.name = $"{gameObject.name} Health Bar";
	}

	private void LateUpdate()
	{
		Collider2D collider = Physics2D.OverlapBox(transform.position, attackRange, 0f, hitLayer);

		if (collider != null)
		{
			if (collider.TryGetComponent(out PlayerStats player))
				player.TakeDamage(stats.GetDynamicStat(Stat.Damage), false, transform.position, stats.GetDynamicStat(Stat.KnockBackStrength));
		}
	}

	public override void TakeDamage(float amount, bool weakpointHit, Vector3 attackerPos = default, float knockBackStrength = 0)
	{
		base.TakeDamage(amount, weakpointHit, attackerPos, knockBackStrength);

		healthBar.SetCurrentHealth(_currentHealth);
	}

	public override void Die()
	{
		base.Die();
		Destroy(healthBar.gameObject);
		Destroy(gameObject);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(transform.position, attackRange);
	}
}
