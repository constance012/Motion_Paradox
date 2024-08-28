using System.Collections;
using UnityEngine;

public abstract class EntityAction : MonoBehaviour
{
	[Header("Stats"), Space]
	[SerializeField] protected MonoBehaviour movementScript;
	[SerializeField] protected Rigidbody2D rb2d;
	[SerializeField] protected Stats stats;

	// Properties.
	protected virtual float BaseAttackInterval => 1f / stats.GetDynamicStat(Stat.AttackSpeed);

	// Protected fields.
	protected BetterCoroutine _attackCoroutine = new();
	protected float _attackInterval;

	protected virtual void Update()
	{
		_attackInterval -= Time.deltaTime;
	}

	protected abstract void TryAttack();
	protected abstract IEnumerator DoAttack();

	protected void StopPreviousAttack()
	{
		_attackCoroutine.StopCurrent(this);
		movementScript.enabled = true;
	}
}