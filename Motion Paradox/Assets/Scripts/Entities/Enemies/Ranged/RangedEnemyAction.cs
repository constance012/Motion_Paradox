using UnityEngine;
using DG.Tweening;

public abstract class RangedEnemyAction : EnemyAction
{
	[Header("Projectile Settings"), Space]
	[SerializeField] private GameObject projectilePrefab;
	[SerializeField, Min(3)] private int allocateAmount = 3;
	[SerializeField] protected Transform firePoint;
	[SerializeField] private float verticalSpread;

	[Header("Recoil Settings"), Space]
	[SerializeField] private Transform recoilTransform;
	[SerializeField] private float recoilAmount;
	[SerializeField] private float recoilDuration;

	[Header("Fire Effect"), Space]
	[SerializeField] private ParticleSystem muzzleEffect;

	// Protected fields.
	protected static Transform _player;
	protected RangedEnemyAI _AI;
	
	// Private fields.
	private TweenPool _tweenPool;
	private Transform _projectileParent;
	private ObjectPool<ProjectileBase> _projectilePool;

	protected virtual void Awake()
	{
		_projectileParent = new GameObject(gameObject.name).transform;
		_projectileParent.SetParent(GameObject.FindWithTag("ProjectileContainer").transform);
		_projectilePool = new ObjectPool<ProjectileBase>(projectilePrefab, allocateAmount, _projectileParent);
		
		_tweenPool = new TweenPool();
		_AI = movementScript as RangedEnemyAI;
		
		if (_player == null)
			_player = GameObject.FindWithTag("Player").transform;
	}

	public void EnemyStats_Died()
	{
		_tweenPool.KillActiveTweens(true);
		Destroy(_projectileParent.gameObject);
	}

	protected void FireProjectile(Vector3 direction, bool generateRecoil = true)
	{
		muzzleEffect.Play();
		ProjectileBase projectile = _projectilePool.Spawn(firePoint.position, Quaternion.identity);
		projectile.transform.right = direction + new Vector3(0f, Random.Range(-verticalSpread, verticalSpread));
		projectile.Initialize(transform, stats, _player);

		if (generateRecoil)
			GenerateRecoil();
	}

	protected void GenerateRecoil()
	{
		// Generate recoil on the Graphic game object.
		_tweenPool.KillActiveTweens(true);
		_tweenPool.Add(recoilTransform.DOLocalMoveX(-recoilAmount, recoilDuration)
									  .SetEase(Ease.OutQuad)
									  .SetLoops(2, LoopType.Yoyo));
	}
}