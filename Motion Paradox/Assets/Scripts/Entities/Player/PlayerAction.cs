using System.Collections;
using UnityEngine;

public class PlayerAction : EntityAction
{
	[Header("References"), Space]
	[SerializeField] private HandheldWeapon weapon;

	private void Start()
	{
		InputManager.Instance.onAttackAction += (sender, e) => TryAttack();
		InputManager.Instance.onReloadAction += (sender, e) => weapon.TryReload();
	}

	protected override void TryAttack()
	{
		if (GameManager.Instance.GameDone)
			return;

		if (_attackInterval <= 0f && weapon.CanBeUsed())
		{
			StopPreviousAttack();
			_attackCoroutine = StartCoroutine(DoAttack());	
		}
	}

	protected override IEnumerator DoAttack()
	{	
		rb2d.velocity = Vector2.zero;
		movementScript.enabled = false;

		weapon.UseWeapon();

		_attackInterval = BaseAttackInterval;
		
		yield return new WaitForSeconds(.2f);

		movementScript.enabled = true;
	}
}