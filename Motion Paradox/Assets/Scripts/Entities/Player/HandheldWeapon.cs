using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandheldWeapon : MonoBehaviour
{
	[Header("Weapon SO"), Space]
	[SerializeField] private Gun weaponSO;

	[Header("References"), Space]
	[SerializeField] private Stats stats;
	[SerializeField] private AmmoCountUI ammoUI;
	[SerializeField] private Transform firePoint;

	// Private fields.
	private Queue<bool> _firedShots;
	private Gun _gun;
	private Coroutine _reloadCoroutine;

	private void Start()
	{
		_gun = Instantiate(weaponSO);
		_gun.name = weaponSO.name;
		_gun.Initialize();

		ammoUI.Initialize(_gun.maxAmmo, _gun.piercingShotFrequency);
		_firedShots ??= new Queue<bool>();
	}

	public bool CanBeUsed()
	{
		return _gun.CanShoot();
	}

	public void UseWeapon()
	{
		this.TryStopCoroutine(_reloadCoroutine);
		
		_gun.FireBullet(firePoint, stats, out bool isPiercingShot);
		ammoUI.RemoveTop();
		
		_firedShots.Enqueue(isPiercingShot);
	}

	public void TryReload()
	{
		if (_gun.CanReload && !GameManager.Instance.GameDone)
			_reloadCoroutine = StartCoroutine(PerformReload());
	}

	private IEnumerator PerformReload()
	{
		while (_gun.SingleCartridgeReload())
		{
			ammoUI.AddBullet(_firedShots.Dequeue());

			yield return new WaitUntil(() => TimeManager.LocalTimeScale == 1);
			yield return new WaitForSeconds(_gun.reloadInterval);
		}
	}
}