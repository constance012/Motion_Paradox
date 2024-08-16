using UnityEngine;

public abstract class EnemyAction : EntityAction
{
	[Header("Attack Filter"), Space]
	[SerializeField] protected float attackRadius;
	[SerializeField] protected LayerMask hitLayer;

	protected override void Update()
	{
		base.Update();
		TryAttack();
	}

	protected override void TryAttack()
	{
		if (_attackInterval <= 0f && !PlayerStats.IsDeath)
		{
			StopPreviousCoroutine();
			_attackCoroutine = StartCoroutine(DoAttack());
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, attackRadius);
	}
}