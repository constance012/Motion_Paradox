using UnityEngine;

public abstract class ScalableProjectile : ProjectileBase
{
	[Header("Stats Scale Factors"), Space]
	[SerializeField, Min(.5f)] protected float speedScaleFactor;
	[SerializeField, Min(1f)] protected float damageScaleFactor;
	[SerializeField, Min(1f)] protected float lifeTimeScaleFactor;

	public override void Initialize(Transform wearer, Stats wearerStats, Transform trackTarget)
	{
		base.Initialize(wearer, wearerStats, trackTarget);

		flySpeed *= speedScaleFactor;
		maxLifeTime *= lifeTimeScaleFactor;
	}
}