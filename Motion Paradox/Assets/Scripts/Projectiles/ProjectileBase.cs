using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
	[Header("References")]
	[Space]
	[SerializeField] protected Rigidbody2D rb2D;
	[SerializeField, Tooltip("The target to track if this projectile is homing.")]
	protected Transform targetToTrack;

	[Header("General Properties")]
	[Space]
	[SerializeField] protected float maxLifeTime;
	[SerializeField] protected bool isHoming;
	
	[Header("Movement Properties")]
	[Space]
	[SerializeField] protected float flySpeed;
	[SerializeField, Tooltip("How sharp does the projectile turn to reach its target? Measures in deg/s.")]
	protected float trackingRigidity;

	// Protected fields.
	protected float _aliveTime;
	protected Transform _wearer;
	protected Stats _wearerStats;

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
	
	public virtual void Initialize(Transform wearer, Stats wearerStats, Transform trackTarget)
	{
		_wearer = wearer;
		_wearerStats = wearerStats;
		
		SetTarget(trackTarget);

		flySpeed = _wearerStats.GetStaticStat(Stat.ProjectileSpeed);
		trackingRigidity = _wearerStats.GetStaticStat(Stat.ProjectileTrackingRigidity);
		maxLifeTime = _wearerStats.GetStaticStat(Stat.ProjectileLifeTime);

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

			rb2D.rotation = Mathf.MoveTowardsAngle(rb2D.rotation, angle, trackingRigidity * Time.deltaTime);
		}
	}
}
