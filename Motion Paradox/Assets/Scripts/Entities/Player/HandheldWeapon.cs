using System.Collections;
using System.Collections.Generic;
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

		_baseSpread = _gun.verticalSpread;
		ammoHUD.Initialize(_gun.maxAmmo, _gun.piercingShotFrequency);
		crosshair.ChangeBaseExpansion(_baseSpread);
		
		_firedShots = new Queue<bool>();
		_tweenPool = new TweenPool();
	}

	public void ToggleAiming(bool isAiming)
	{
		_gun.verticalSpread = isAiming ? 0f : _baseSpread;
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
		if (_gun.CanReload && !GameManager.GameDone)
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
		transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

		float muzzleClimbAngle = _gun.muzzleClimbAngle * Mathf.Sign(transform.localScale.y);
		float recoilStrength = -_gun.recoilStrength;

		if (isPiercingShot)
		{
			muzzleClimbAngle *= 1.5f;
			recoilStrength *= 1.5f;
		}

		Sequence sequence = DOTween.Sequence();
		sequence.Append(transform.DOLocalRotate(Vector3.forward * muzzleClimbAngle, _gun.recoilDuration))
				.Join(transform.DOLocalMoveX(recoilStrength, _gun.recoilDuration))
				.SetEase(_gun.recoilEase)
				.SetLoops(2, LoopType.Yoyo);
		
		_tweenPool.Add(sequence);
	}
}