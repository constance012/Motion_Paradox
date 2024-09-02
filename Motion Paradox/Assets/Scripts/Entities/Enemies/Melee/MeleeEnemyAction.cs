using UnityEngine;

public abstract class MeleeEnemyAction : EnemyAction
{
	[Header("Charge Attack Settings"), Space]
	[SerializeField] private float chargeForce;

	// Private fields.
	private Vector2 _chargeVelocity;

	private void FixedUpdate()
	{
		if (!movementScript.enabled)
		{
			_chargeVelocity = Vector2.Lerp(_chargeVelocity, Vector2.zero, Time.deltaTime);
			
			if (TimeManager.LocalTimeScale > 0f)
			{
				rb2d.velocity = _chargeVelocity * TimeManager.LocalTimeScale;
			}
			else
			{
				rb2d.velocity = Vector2.zero;
			}
		}
	}

	protected void Charge()
	{
		Vector2 chargeDirection = (PlayerController.Position- rb2d.position).normalized;
		rb2d.AddForce(chargeDirection * chargeForce, ForceMode2D.Impulse);
		_chargeVelocity = rb2d.velocity;
	}
}