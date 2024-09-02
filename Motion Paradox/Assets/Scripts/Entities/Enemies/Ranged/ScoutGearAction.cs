using System.Collections;
using UnityEngine;

public sealed class ScoutGearAction : RangedEnemyAction
{
	[Header("Attack Pattern"), Space]
	[SerializeField] private Vector2Int shotsBeforeRetreatRange;

	// Private fields.
	private int _shotCount = 0;
	private int _maxShotBeforeRetreat;

	protected override void Awake()
	{
		base.Awake();
		_maxShotBeforeRetreat = shotsBeforeRetreatRange.RandomBetweenEnds();
	}

	protected override IEnumerator DoAttack()
	{
		if (DistanceToPlayer <= attackRadius && !_AI.IsRetreating)
		{
			rb2d.velocity = Vector2.zero;
			_AI.enabled = false;

			FireProjectile(firePoint.right);
			ResetAttackInterval();
			
			if (++_shotCount == _maxShotBeforeRetreat)
			{
				_shotCount = 0;
				_maxShotBeforeRetreat = shotsBeforeRetreatRange.RandomBetweenEnds();
				_AI.BeginRetreating();
			}
			else
				yield return new WaitForSeconds(recoverTime);

			_AI.enabled = true;
		}
	}
}