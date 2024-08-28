using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using DG.Tweening;
using UnityEngine;

public class HandheldWeapon : MonoBehaviour
{
	[Header("Weapon SO"), Space]
	[SerializeField] private Gun weaponSO;

	[Header("References"), Space]
	[SerializeField] private Stats stats;
	[SerializeField] private Transform firePoint;
	
	[Header("UI References"), Space]
	[SerializeField] private AmmoCountHUD ammoHUD;
	[SerializeField] private DynamicCrosshair crosshair;

	// Properties.
	public float FireInterval => 1f / _gun.fireRate;

	// Private fields.
	private Gun _gun;
	private Queue<bool> _firedShots;
	private BetterCoroutine _reloadCoroutine = new();
	private TweenPool _tweenPool;
	private float _baseSpread;

	private void Start()
	{
		_gun = Instantiate(weaponSO);
		_gun.name = weaponSO.name;
		_gun.Initialize();

		_baseSpread = _gun.verticleSpread;
		ammoHUD.Initialize(_gun.maxAmmo, _gun.piercingShotFrequency);
		crosshair.ChangeBaseExpansion(_baseSpread);
		
		_firedShots = new Queue<bool>();
		_tweenPool = new TweenPool();
	}

	public void ToggleAiming(bool isAiming)
	{
		_gun.verticleSpread = isAiming ? 0f : _baseSpread;
		crosshair.ChangeBaseExpansion(isAiming ? -.15f : _baseSpread);
	}

	public bool CanBeUsed()
	{
		return _gun.CanShoot();
	}

	public void UseWeapon()
	{
		_reloadCoroutine.StopCurrent(this);
		
		_gun.FireBullet(firePoint, stats, out bool isPiercingShot);
		GenerateRecoil(isPiercingShot);
		
		ammoHUD.RemoveTop();
		crosshair.Expand(FireInterval);
		
		_firedShots.Enqueue(isPiercingShot);
	}

	public void TryReload()
	{
		if (_gun.CanReload && !GameManager.Instance.GameDone)
			_reloadCoroutine.StartNew(this, PerformReload());
	}

	private IEnumerator PerformReload()
	{
		while (_gun.SingleCartridgeReload())
		{
			ammoHUD.AddBottom(_firedShots.Dequeue());

			yield return new WaitUntil(() => TimeManager.LocalTimeScale == 1);
			yield return new WaitForSeconds(_gun.reloadInterval);
		}
	}

	private void GenerateRecoil(bool isPiercingShot)
	{
		_tweenPool.KillActiveTweens(false);

		float strength = _gun.recoilStrength * Mathf.Sign(transform.localScale.y);
		if (isPiercingShot)
			strength *= 1.5f;

		_tweenPool.Add(transform.DOLocalRotate(Vector3.forward * strength, _gun.recoilDuration)
				 				.SetEase(Ease.OutQuad)
				 				.SetLoops(2, LoopType.Yoyo));
	}
}