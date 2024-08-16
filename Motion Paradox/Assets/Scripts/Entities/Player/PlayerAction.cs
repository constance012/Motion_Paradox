using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : EntityAction
{
	[Header("References"), Space]
	[SerializeField] private AmmoCountUI ammoUI;
	[SerializeField] private Transform firePoint;

	[Header("Projectile Prefabs"), Space]
	[SerializeField] private GameObject normalBulletPrefab;
	[SerializeField] private GameObject piercingBulletPrefab;

	[Header("Ammon and Reload"), Space]
	[SerializeField, Min(4)] private int maxAmmo;
	[SerializeField, Min(2)] private int piercingShotFrequency;
	[SerializeField] private float reloadInterval;

	public bool OutOfAmmo => _currentAmmo == 0;

	// Private fields.
	private Queue<bool> _firedShots = new Queue<bool>();
	private int _currentAmmo;
	private int _shotIndex = 0;
	private Coroutine _reloadCoroutine;
	private bool _isReloading;

	private void OnDisable()
	{
		CursorManager.Instance.SwitchCursorTexture(CursorTextureType.Default);
	}

	private void Start()
	{
		ammoUI.Initialize(maxAmmo, piercingShotFrequency);
		_currentAmmo = maxAmmo;

		InputManager.Instance.onAttackAction += (sender, e) => TryAttack();
		InputManager.Instance.onReloadAction += (sender, e) => TryReload();
	}

	protected override void TryAttack()
	{
		if (GameManager.Instance.GameDone)
			return;

		if (OutOfAmmo)
		{
			AudioManager.Instance.Play("Dry Fire");
		}
		else if (_attackInterval <= 0f)
		{
			StopPreviousCoroutine();
			_attackCoroutine = StartCoroutine(DoAttack());
		}
	}

	protected override IEnumerator DoAttack()
	{
		if (_isReloading && _reloadCoroutine != null)
		{
			StopCoroutine(_reloadCoroutine);
			_isReloading = false;
		}

		rb2d.velocity = Vector2.zero;
		movementScript.enabled = false;

		_shotIndex = (_shotIndex + 1) % piercingShotFrequency;
		bool isPiercingShot = _shotIndex == 0;
		
		InstantiateBullet(isPiercingShot);

		_currentAmmo--;
		ammoUI.RemoveTop();
		_firedShots.Enqueue(isPiercingShot);

		_attackInterval = BaseAttackInterval;
		
		yield return new WaitForSeconds(.2f);

		movementScript.enabled = true;
	}

	private void TryReload()
	{	
		if (!_isReloading && !GameManager.Instance.GameDone)
			_reloadCoroutine = StartCoroutine(PerformReload());
	}

	private IEnumerator PerformReload()
	{
		_isReloading = true;

		for (int i = maxAmmo - _currentAmmo; i > 0; i--)
		{
			AudioManager.Instance.PlayWithRandomPitch("Reloading", .7f, 1.2f);
			
			_currentAmmo++;
			ammoUI.AddBullet(_firedShots.Dequeue());

			yield return new WaitUntil(() => TimeManager.LocalTimeScale == 1);
			yield return new WaitForSeconds(reloadInterval);
		}

		_isReloading = false;
	}

	private void InstantiateBullet(bool isPiercingShot)
	{
		GameObject bulletPrefab = isPiercingShot ? piercingBulletPrefab : normalBulletPrefab;
		GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
		bullet.transform.right = firePoint.right;
		bullet.GetComponent<ProjectileBase>().Initialize(transform, stats, null);

		EffectInstantiator.Instance.Instantiate<ParticleSystem>(EffectType.MuzzleFlash, firePoint.position, firePoint.rotation);
		CameraShaker.Instance.ShakeCamera(isPiercingShot ? 4f : 2f, .3f);

		AudioManager.Instance.SetVolume("Gunshot", .5f, isPiercingShot);
		AudioManager.Instance.PlayWithRandomPitch("Gunshot", .7f, .9f);
	}
}