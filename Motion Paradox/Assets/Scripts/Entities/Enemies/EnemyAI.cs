using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

	[Header("References"), Space]
	[SerializeField] private Rigidbody2D rb2D;

	[Header("Stats"), Space]
	[SerializeField] private Stats stats;

	[Header("Repel Settings"), Space]
	[SerializeField] private float repelRange;
	[SerializeField] private float repelAmplitude;

	// Private fields.
	private static HashSet<Rigidbody2D> _alertedEnemies;
	private bool _facingRight = true;

	private void Start()
	{
		_alertedEnemies ??= new HashSet<Rigidbody2D>();
		_alertedEnemies.Add(rb2D);
	}

	private void OnDestroy()
	{
		_alertedEnemies.Remove(rb2D);
	}

	private void FixedUpdate()
	{
		if (PlayerStats.IsDeath)
			return;

		CheckFlip();
	}

	private void CheckFlip()
	{
		float sign = Mathf.Sign(PlayerController.Position.x -  rb2D.position.x);
		bool mustFlip = (_facingRight && sign < 0f) || (!_facingRight && sign > 0f);

		if (mustFlip)
		{
			transform.Rotate(Vector3.up * 180f);
			_facingRight = !_facingRight;
		}
	}

	/// <summary>
	/// Calculate the final velocity after applying repel force against other enemies nearby.
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	private Vector2 CalculateVelocity(Vector2 direction)
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

		Vector2 velocity = direction * stats.GetDynamicStat(Stat.MoveSpeed);
		velocity += repelForce.normalized * repelAmplitude;

		return velocity;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, repelRange);
	}
}
