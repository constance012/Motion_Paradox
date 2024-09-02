using System.Collections;
using UnityEngine;

public sealed class PawnGearAction : MeleeEnemyAction
{
	[Header("Attack Pattern"), Space]
	[SerializeField, Range(0f, 1f)] private float explodeChance;

	protected override IEnumerator DoAttack()
	{
		if (DistanceToPlayer <= attackRadius)
		{
			rb2d.velocity = Vector2.zero;
			movementScript.enabled = false;

			Charge();
			ResetAttackInterval();

			yield return new WaitForSeconds(recoverTime);
			
			movementScript.enabled = true;
		}
	}

	protected override void OnTriggerEnter2D(Collider2D other)
	{
		if (Random.value <= explodeChance)
		{
			DamageOnContact(other, 2f);
			GetComponent<EnemyStats>().Die();
		}
		else
		{
			base.OnTriggerEnter2D(other);
		}
	}
}