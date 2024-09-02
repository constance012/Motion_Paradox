using System.Collections;
using UnityEngine;

public abstract class EntityAction : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] protected MonoBehaviour movementScript;
	[SerializeField] protected Rigidbody2D rb2d;
	[SerializeField] protected Stats stats;

	// Properties.
	protected abstract float BaseAttackInterval { get; }

	// Protected fields.
	protected BetterCoroutine _attackCoroutine = new();
	protected float _attackInterval;

	protected abstract void Update();
	protected abstract void TryAttack();
	protected abstract IEnumerator DoAttack();

	public void ResetAttackInterval()
	{
		_attackInterval = BaseAttackInterval;
	}

	protected void StopPreviousAttack()
	{
		_attackCoroutine.StopCurrent(this);
		movementScript.enabled = true;
	}
}