using UnityEngine;

public sealed class MetalMissile : ScalableProjectile
{
	public override void ProcessCollision(Collider2D other)
	{
		flySpeed = 0f;

		DamageTarget(other, damageScaleFactor);

		EffectInstantiator.Instance.Instantiate<ParticleSystem>(EffectType.Explosion, transform.position, Quaternion.identity);
		AudioManager.Instance.Play("Explosion");
		CameraShaker.Instance.ShakeCamera(5f, .3f);

		Destroy(gameObject);
	}
}