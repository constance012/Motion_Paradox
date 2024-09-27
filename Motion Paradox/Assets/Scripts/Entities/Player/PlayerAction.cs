using System.Collections;
using UnityEngine;

public sealed class PlayerAction : EntityAction
{
	[Header("References"), Space]
	[SerializeField] private HandheldWeapon weapon;

	protected override float BaseAttackInterval => weapon.FireInterval;

	private void Start()
	{
		InputManager.Instance.OnAttackAction += (sender, e) => TryAttack();
		InputManager.Instance.OnReloadAction += (sender, e) => weapon.TryReload();
	}

	protected override void Update()
	{
		_attackInterval -= Time.deltaTime;
	}

	protected override void TryAttack()
	{
		if (GameManager.GameDone || DialogueManager.IsPlaying)
			return;

		if (_attackInterval <= 0f && weapon.CanBeUsed())
		{
			StopPreviousAttack();
			_attackCoroutine.StartNew(this, DoAttack());
		}
	}

	protected override IEnumerator DoAttack()
	{	
		rb2d.velocity = Vector2.zero;
		movementScript.enabled = false;

		weapon.UseWeapon();
		ResetAttackInterval();
		
		yield return new WaitForSeconds(.2f);

		movementScript.enabled = true;
	}
}