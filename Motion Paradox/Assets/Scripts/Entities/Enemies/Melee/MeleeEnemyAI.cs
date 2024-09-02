public sealed class MeleeEnemyAI : EnemyAI
{
	protected override void FixedUpdate()
	{		
		if (PlayerStats.IsDeath)
			return;

		ChaseTarget(PlayerController.Position);
	}
}