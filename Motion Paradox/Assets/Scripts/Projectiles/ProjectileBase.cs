using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] protected Rigidbody2D rb2D;
	[SerializeField, ReadOnly, Tooltip("The target to track if this projectile is homing.")]
	protected Transform targetToTrack;

	[Header("General Properties"), Space]
	[SerializeField] protected float maxLifeTime;
	[SerializeField, ReadOnly] protected bool isHoming;
	
	[Header("Movement Properties"), Space]
	[SerializeField] protected float flySpeed;
	[Tooltip("How sharp does the projectile turn to reach its target? Measures in deg/s.")]
	public float trackingRigidity;

	// Protected fields.
	protected float _aliveTime;
	protected Transform _shooter;
	protected Stats _shooterStats;

	private void FixedUpdate()
	{
		TravelForwards();

		if (PlayerStats.IsDeath)
		{
			targetToTrack = null;
		 	return;
		}

		TrackingTarget();
	}

	private void LateUpdate()
	{
		if (TimeManager.LocalTimeScale > 0f)
			_aliveTime -= Time.deltaTime;

		if (_aliveTime <= 0f)
			Destroy(gameObject);
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		ProcessCollision(other.collider);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		ProcessCollision(other);
	}

	public void SetTarget(Transform target)
	{
		targetToTrack = target;
		isHoming = targetToTrack != null;
	}
	
	public virtual void Initialize(Transform shooter, Stats shooterStats, Transform trackTarget)
	{
		_shooter = shooter;
		_shooterStats = shooterStats;
		
		SetTarget(trackTarget);

		flySpeed = _shooterStats.GetStaticStat(Stat.ProjectileSpeed);
		trackingRigidity = _shooterStats.GetStaticStat(Stat.ProjectileTrackingRigidity);
		maxLifeTime = _shooterStats.GetStaticStat(Stat.ProjectileLifeTime);

		_aliveTime = maxLifeTime;
	}

	/// <summary>
	/// Determines what happens if this projectile collides with other objects.
	/// </summary>
	/// <param name="other"></param>
	public abstract void ProcessCollision(Collider2D other);

	protected void TravelForwards()
	{
		rb2D.velocity = transform.right * flySpeed * TimeManager.LocalTimeScale;
	}

	protected void TrackingTarget()
	{
		if (isHoming && targetToTrack != null)
		{
			Vector3 trackDirection = targetToTrack.position - transform.position;
			float angle = Mathf.Atan2(trackDirection.y, trackDirection.x) * Mathf.Rad2Deg;

			rb2D.rotation = Mathf.MoveTowardsAngle(rb2D.rotation, angle, trackingRigidity * Time.deltaTime * TimeManager.LocalTimeScale);
		}
	}

	protected void DamageTarget(Collider2D other, float damageScale)
	{
		IDamageable target = other.GetComponentInParent<IDamageable>();

		if (target != null && _shooter != null)
		{
			EffectInstantiator.Instance.Instantiate<ParticleSystem>(EffectType.SolidImpact, target.Position, -transform.right);
			target.Damage(_shooterStats, _shooter.position, scaleFactor: damageScale);
		}
	}
}
