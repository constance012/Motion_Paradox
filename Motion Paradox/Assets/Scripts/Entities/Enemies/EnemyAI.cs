using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAI : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] protected Rigidbody2D rb2D;
	[SerializeField] protected EnemyAction action;

	[Header("Stats"), Space]
	[SerializeField] protected Stats stats;

	[Header("Repeling"), Space]
	[SerializeField] private float repelRange;
	[SerializeField] private float repelAmplitude;

	[Header("Turning"), Space]
	[SerializeField, Tooltip("How quick does the enemy change its flying direction? In deg/s")]
	protected float turnSpeed;

	// Protected fields.
	protected Vector2 _rawTargetDirection = Vector2.one * 1000f;
	
	// Private fields.
	private static HashSet<Rigidbody2D> _alertedEnemies;

	protected virtual void Start()
	{
		_alertedEnemies ??= new HashSet<Rigidbody2D>();
		_alertedEnemies.Add(rb2D);
	}

	private void OnDestroy()
	{
		_alertedEnemies.Remove(rb2D);
	}

	private void Update()
	{
		_rawTargetDirection = PlayerController.Position - rb2D.position;
	}

	protected abstract void FixedUpdate();

	protected void ChaseTarget(Vector2 targetPosition)
	{
		rb2D.velocity = CalculateVelocity(transform.right, stats.GetDynamicStat(Stat.MoveSpeed));
		LookAt(targetPosition);
	}

	protected void LookAt(Vector2 targetPosition)
	{
		Vector2 targetDirection = (targetPosition - rb2D.position).normalized;
		float turnAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;

		rb2D.rotation = Mathf.MoveTowardsAngle(rb2D.rotation, turnAngle, turnSpeed * Time.deltaTime * TimeManager.LocalTimeScale);
	}

	/// <summary>
	/// Calculate the final velocity after applying repel force against other enemies nearby.
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	protected Vector2 CalculateVelocity(Vector2 direction, float speed)
	{
		// Enemies will try to avoid each other.
		Vector2 repelForce = Vector2.zero;

		foreach (Rigidbody2D enemy in _alertedEnemies)
		{
			if (enemy == rb2D)
				continue;

			if (Vector2.Distance(enemy.position, rb2D.position) <= repelRange)
			{
				Vector2 repelDirection = (rb2D.position - enemy.position).normalized;
				repelForce += repelDirection;
			}
		}

		Vector2 velocity = direction * speed * TimeManager.LocalTimeScale;
		velocity += repelForce.normalized * repelAmplitude;

		return velocity;
	}

	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(rb2D.position, repelRange);
	}
}
