using UnityEngine;

public sealed class MeleeEnemyAI : EnemyAI
{
	[Header("Turning Settings"), Space]
	[SerializeField, Tooltip("How quick does the enemy change its flying direction? In deg/s")]
	private float turnSpeed;

	protected override void FixedUpdate()
	{		
		if (PlayerStats.IsDeath)
			return;

		Vector2 targetDirection = (PlayerController.Position - rb2D.position).normalized;
		float turnAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

		rb2D.velocity = CalculateVelocity(transform.right);
		rb2D.rotation = Mathf.MoveTowardsAngle(rb2D.rotation, turnAngle, turnSpeed * Time.deltaTime * TimeManager.LocalTimeScale);
	}
}