using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Gun", fileName = "New Gun")]
public class Gun : IdentifiableSO
{
	[Header("Bullet Prefab"), Space]
	public GameObject normalBulletPrefab;
	public GameObject piercingBulletPrefab;

	[Header("Spreading and Recoil"), Space]
	public float verticalSpread;
	public float muzzleClimbAngle;
	public float recoilStrength;
	public float recoilDuration;
	public Ease recoilEase;

	[Header("Ammunition"), Space]
	[Min(2)] public int maxAmmo;
	[Min(0)] public int piercingShotFrequency;
	public float fireRate;
	public float reloadInterval;
	[HideInInspector] public bool _isReloading;

	public bool CanReload => !_isReloading && _currentAmmo < maxAmmo;
	public bool OutOfAmmo => _currentAmmo == 0;
	public int CurrentAmmo => _currentAmmo;

	// Private fields.
	private int _currentAmmo;
	private int _shotIndex = 0;

	public void Initialize()
	{
		if (piercingShotFrequency == 0)
			piercingShotFrequency = int.MaxValue;
		
		_currentAmmo = maxAmmo;
		_shotIndex = 0;
	}

	public bool CanShoot()
	{
		if (OutOfAmmo)
		{
			AudioManager.Instance.Play("Dry Fire");
			return false;
		}

		return true;
	}

	public void FireBullet(Transform firePoint, Stats stats, out bool isPiercingShot)
	{
		_isReloading = false;

		_shotIndex = (_shotIndex + 1) % piercingShotFrequency;
		isPiercingShot = _shotIndex == 0;
		
		SpawnBullet(firePoint, stats, isPiercingShot);

		_currentAmmo--;
	}

	public bool SingleCartridgeReload()
	{
		if (_currentAmmo < maxAmmo)
		{
			_isReloading = true;

			AudioManager.Instance.PlayWithRandomPitch("Reloading", .8f, 1f);
			_currentAmmo++;
		}
		else
			_isReloading = false;

		return _isReloading;
	}

	private void SpawnBullet(Transform firePoint, Stats stats, bool isPiercingShot)
	{
		ProjectileBase bullet = isPiercingShot ? ProjectilePool.Instance.Spawn(ProjectileType.PiercingBullet, firePoint.position, Quaternion.identity) :
												 ProjectilePool.Instance.Spawn(ProjectileType.NormalBullet, firePoint.position, Quaternion.identity);
		
		bullet.transform.right = CalculateBulletDirection(firePoint.right);
		bullet.Initialize(firePoint, stats, null);

		EffectInstantiator.Instance.Instantiate<ParticleSystem>(EffectType.MuzzleFlash, firePoint);
		CameraShaker.Instance.ShakeCamera(isPiercingShot ? 4f : 2f, .3f);

		AudioManager.Instance.SetVolume("Gunshot", .8f, isPiercingShot);
		AudioManager.Instance.PlayWithRandomPitch("Gunshot", .9f, 1.1f);
	}

	private Vector2 CalculateBulletDirection(Vector2 initalDirection)
	{
		Vector2 spreading = new Vector2(0f, Random.Range(-verticalSpread, verticalSpread));
		return initalDirection + spreading;
	}
}