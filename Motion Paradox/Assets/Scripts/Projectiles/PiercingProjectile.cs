using UnityEngine;

public sealed class PiercingProjectile : ProjectileBase
{
	[Header("Stats Scale Factors"), Space]
	[SerializeField, Min(1.5f)] private float speedScaleFactor;
	[SerializeField, Min(2f)] private float damageScaleFactor;

	public override void Initialize(Transform wearer, Stats wearerStats, Transform trackTarget)
	{
		base.Initialize(wearer, wearerStats, trackTarget);
		flySpeed *= speedScaleFactor;
	}

	public override void ProcessCollision(Collider2D other)
	{
		Entity target = other.GetComponentInParent<Entity>();

		if (target != null && _wearer != null)
		{
			if (other.gameObject.layer == 6)
				EffectInstantiator.Instance.Instantiate<ParticleSystem>(EffectType.SolidImpact, target.transform.position, -transform.right);

			float damage = _wearerStats.GetDynamicStat(Stat.Damage) * damageScaleFactor;
			float knockBackStrength = _wearerStats.GetStaticStat(Stat.KnockBackStrength) * damageScaleFactor;
			
			target.TakeDamage(damage, false, _wearer.position, knockBackStrength);
		}
	}
}