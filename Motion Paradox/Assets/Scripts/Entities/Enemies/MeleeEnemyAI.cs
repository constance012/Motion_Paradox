using UnityEngine;

public sealed class MeleeEnemyAI : EnemyAI
{
	protected override void FixedUpdate()
	{
		if (PlayerStats.IsDeath)
			return;

		Vector2 direction = (PlayerController.Position - rb2D.position).normalized;
		rb2D.velocity = CalculateVelocity(direction);
		
		CheckFlip();
	}
}