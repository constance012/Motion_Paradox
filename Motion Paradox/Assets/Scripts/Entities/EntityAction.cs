using System.Collections;
using UnityEngine;

public abstract class EntityAction : MonoBehaviour
{
	[Header("Stats"), Space]
	[SerializeField] protected MonoBehaviour movementScript;
	[SerializeField] protected Rigidbody2D rb2d;
	[SerializeField] protected Stats stats;

	// Properties.
	protected float BaseAttackInterval => 1f / stats.GetDynamicStat(Stat.AttackSpeed);

	// Protected fields.
	protected Coroutine _attackCoroutine;
	protected float _attackInterval;

	protected virtual void Update()
	{
		_attackInterval -= Time.deltaTime;
	}

	protected abstract void TryAttack();
	protected abstract IEnumerator DoAttack();

	protected void StopPreviousAttack()
	{
		if (this.TryStopCoroutine(_attackCoroutine))
		{
			movementScript.enabled = true;
		}
	}
}