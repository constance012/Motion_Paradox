using System.Collections;
using UnityEngine;

public sealed class InterceptorGearAction : RangedEnemyAction
{
	[Header("Attack Pattern"), Space]
	[SerializeField] private Vector2Int consecutiveShotRange;
	[SerializeField] private float shotInterval;

	protected override IEnumerator DoAttack()
	{
		if (DistanceToPlayer <= attackRadius && !_AI.IsRetreating)
		{
			rb2d.velocity = Vector2.zero;
			movementScript.enabled = false;

			int shotCount = consecutiveShotRange.RandomBetweenEnds();
			
			for (int i = 0; i < shotCount; i++)
			{
				FireProjectile(firePoint.right);
				ResetAttackInterval();
				yield return new WaitForSeconds(shotInterval);
			}

			yield return new WaitForSeconds(recoverTime);

			movementScript.enabled = true;
		}
	}
}