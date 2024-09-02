using UnityEngine;

public sealed class PiercingProjectile : ScalableProjectile
{
	public override void ProcessCollision(Collider2D other)
	{
		DamageTarget(other, damageScaleFactor);
	}
}