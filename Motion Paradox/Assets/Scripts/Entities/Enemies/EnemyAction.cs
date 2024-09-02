using UnityEngine;

public abstract class EnemyAction : EntityAction
{
	[Header("Attack Properties"), Space]
	[SerializeField] protected float attackRadius;
	[SerializeField] protected float recoverTime;

	public float AttackRadius => attackRadius;
	public float AttackRadiusSquared => attackRadius * attackRadius;
	protected override float BaseAttackInterval => 1f / stats.GetDynamicStat(Stat.AttackSpeed);
	protected float DistanceToPlayer => Vector2.Distance(PlayerController.Position, rb2d.position);

	protected override void Update()
	{
		_attackInterval -= Time.deltaTime * TimeManager.LocalTimeScale;
		TryAttack();
	}

	protected override void TryAttack()
	{
		if (_attackInterval <= 0f && !PlayerStats.IsDeath)
		{
			StopPreviousAttack();
			_attackCoroutine.StartNew(this, DoAttack());
		}
	}

	protected virtual void OnTriggerEnter2D(Collider2D other)
	{
		DamageOnContact(other, 1f);
	}

	protected void DamageOnContact(Collider2D other, float damageScale)
	{
		IDamageable target = other.GetComponentInParent<IDamageable>();
		target?.Damage(stats, rb2d.position, damageScale);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, attackRadius);
	}
}