using UnityEngine;

public sealed class SimpleProjectile : ProjectileBase
{
	public override void ProcessCollision(Collider2D other)
	{
		flySpeed = 0f;

		
		Entity target = other.GetComponentInParent<Entity>();

		if (target != null && _wearer != null)
		{
			if (other.gameObject.layer == 6)
				EffectInstantiator.Instance.Instantiate<ParticleSystem>(EffectType.SolidImpact, target.transform.position, -transform.right);

			target.TakeDamage(_wearerStats.GetDynamicStat(Stat.Damage), false, _wearer.position, _wearerStats.GetStaticStat(Stat.KnockBackStrength));
		}

		Destroy(gameObject);
	}
}
