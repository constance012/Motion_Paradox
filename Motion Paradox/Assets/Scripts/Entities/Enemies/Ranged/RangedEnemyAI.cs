using UnityEngine;
using CSTGames.Utility;

public sealed class RangedEnemyAI : EnemyAI
{
	[Header("Strafing"), Space]
	[SerializeField] private float strafeSpeed;

	[Header("Retreating"), Space]
	[SerializeField] private float retreatThreshold;
	[SerializeField] private float retreatMagnitude;
	[SerializeField] private float retreatDuration;

	public bool IsRetreating => _isRetreating;
	private float RetreatThresholdSquared => retreatThreshold * retreatThreshold;

	// Private fields.
	private float _strafeDirection;
	private bool _isRetreating;
	private float _retreatTimer;
	private Vector2 _fallbackLocation;

	protected override void Start()
	{
		base.Start();
		_strafeDirection = RandomUtils.RandomSign;
	}

	protected override void FixedUpdate()
	{
		if (PlayerStats.IsDeath)
			return;
		
		if (!_isRetreating)
		{
			if (_rawTargetDirection.sqrMagnitude > action.AttackRadiusSquared + .04f)
			{
				ChaseTarget(PlayerController.Position);
			}
			else if (_rawTargetDirection.sqrMagnitude > RetreatThresholdSquared)
			{
				StrafeAroundTarget();
			}
			else
			{
				BeginRetreating();
			}
		}
		else
		{
			Retreat();
		}
	}

	public void BeginRetreating()
	{
		Debug.Log("Retreating...");
		action.ResetAttackInterval();
				
		_isRetreating = true;
		_retreatTimer = retreatDuration;
		_fallbackLocation = rb2D.position - (Vector2)transform.right * retreatMagnitude;

		_strafeDirection = RandomUtils.RandomSign;
	}

	private void Retreat()
	{
		_retreatTimer -= Time.deltaTime;

		if (_retreatTimer <= 0f)
			_isRetreating = false;
		else
			ChaseTarget(_fallbackLocation);
	}

	private void StrafeAroundTarget()
	{
		rb2D.velocity = CalculateVelocity(transform.up * _strafeDirection, strafeSpeed);
		LookAt(PlayerController.Position);
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(rb2D.position, retreatThreshold);
	}
}