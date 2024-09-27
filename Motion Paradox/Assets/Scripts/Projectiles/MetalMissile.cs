using UnityEngine;

public sealed class MetalMissile : ScalableProjectile
{
	public override void Initialize(Transform wearer, Stats wearerStats, Transform trackTarget)
	{
		base.Initialize(wearer, wearerStats, trackTarget);
		AudioManager.Instance.Play("Missile Fuel Burning");
	}

	public override void ProcessCollision(Collider2D other)
	{
		DamageTarget(other, damageScaleFactor);

		EffectInstantiator.Instance.Instantiate<ParticleSystem>(EffectType.Explosion, transform.position, Quaternion.identity);
		AudioManager.Instance.Stop("Missile Fuel Burning");
		AudioManager.Instance.Play("Explosion");
		CameraShaker.Instance.ShakeCamera(5f, .3f);

		Deallocate();
	}
}