using UnityEngine;

public sealed class NormalProjectile : ProjectileBase
{
	public override void ProcessCollision(Collider2D other)
	{
		flySpeed = 0f;
		DamageTarget(other, 1f);
		Destroy(gameObject);
	}
}
