using UnityEngine;
using DG.Tweening;

public abstract class RangedEnemyAction : EnemyAction
{
	[Header("Projectile Settings"), Space]
	[SerializeField] private GameObject projectilePrefab;
	[SerializeField] protected Transform firePoint;
	[SerializeField] private float verticalSpread;

	[Header("Recoil Settings"), Space]
	[SerializeField] private Transform recoilTransform;
	[SerializeField] private float recoilAmount;
	[SerializeField] private float recoilDuration;

	// Protected fields.
	protected static Transform _player;
	protected RangedEnemyAI _AI;
	
	// Private fields.
	private TweenPool _tweenPool;

	protected virtual void Awake()
	{
		_tweenPool = new TweenPool();
		_AI = movementScript as RangedEnemyAI;
		
		if (_player == null)
			_player = GameObject.FindWithTag("Player").transform;
	}

	private void OnDestroy()
	{
		_tweenPool.KillActiveTweens(true);
	}

	protected void FireProjectile(Vector3 direction, bool generateRecoil = true)
	{
		GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
		projectile.transform.right = direction + new Vector3(0f, Random.Range(-verticalSpread, verticalSpread));
		projectile.GetComponent<ProjectileBase>().Initialize(transform, stats, _player);

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