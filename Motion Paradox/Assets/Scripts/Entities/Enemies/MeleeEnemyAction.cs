using System.Collections;
using UnityEngine;

public sealed class MeleeEnemyAction : EnemyAction
{
	[Header("Attack Settings"), Space]
	[SerializeField] private float chargeForce;
	[SerializeField] private float recoverTime;

	private void OnTriggerEnter2D(Collider2D other)
	{
		Entity target = other.GetComponentInParent<Entity>();

		if (target != null)
		{
			target.TakeDamage(stats.GetDynamicStat(Stat.Damage), false, rb2d.position, stats.GetStaticStat(Stat.KnockBackStrength));
		}
	}

	protected override IEnumerator DoAttack()
	{
		if (Vector2.Distance(PlayerController.Position, rb2d.position) <= attackRadius)
		{
			rb2d.velocity = Vector2.zero;
			movementScript.enabled = false;

			Vector2 chargeDirection = (PlayerController.Position- rb2d.position).normalized;
			rb2d.AddForce(chargeDirection * chargeForce * TimeManager.LocalTimeScale, ForceMode2D.Impulse);

			_attackInterval = BaseAttackInterval;

			yield return new WaitForSeconds(recoverTime);

			movementScript.enabled = true;
		}
	}
}