using System.Collections;
using UnityEngine;

public sealed class SuppressorGearAction : MeleeEnemyAction
{
	[Header("Attack Pattern"), Space]
	[SerializeField] private Vector2Int consecutiveChargeRange;
	[SerializeField] private float chargeInterval;

	protected override IEnumerator DoAttack()
	{
		if (DistanceToPlayer <= attackRadius)
		{
			rb2d.velocity = Vector2.zero;
			movementScript.enabled = false;

			int chargeCount = consecutiveChargeRange.RandomBetweenEnds();

			for (int i = 0; i < chargeCount; i++)
			{
				Charge();
				ResetAttackInterval();
				yield return new WaitForSeconds(chargeInterval);
			}

			yield return new WaitForSeconds(recoverTime);
			
			movementScript.enabled = true;
		}
	}
}