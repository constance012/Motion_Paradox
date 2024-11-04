using UnityEngine;

[AddComponentMenu("Object Pools/Effect Pool")]
public sealed class EffectPool : MultiplePrefabsPool<EffectPool, EffectType, PoolableEffectBase>
{
	#region Instantiate Method Overloads.
	public PoolableEffectBase Spawn(EffectType type, RaycastHit2D hitInfo)
	{
		PoolableEffectBase effect = _pools[type].Spawn();

		effect.transform.position = hitInfo.point;
		effect.transform.rotation = Quaternion.LookRotation(hitInfo.normal, Vector3.right);

		return effect;
	}
	
	public PoolableEffectBase Spawn(EffectType type, RaycastHit2D hitInfo, Vector3 rotateAxis)
	{
		PoolableEffectBase effect = _pools[type].Spawn();

		effect.transform.position = hitInfo.point;
		effect.transform.rotation = Quaternion.LookRotation(hitInfo.normal, rotateAxis);

		return effect;
	}

	public PoolableEffectBase Spawn(EffectType type, Vector3 position, Vector3 normal)
	{
		PoolableEffectBase effectObj = _pools[type].Spawn();

		effectObj.transform.position = position;
		effectObj.transform.rotation = Quaternion.LookRotation(normal, Vector3.right);

		return effectObj;
	}
	
	public PoolableEffectBase Spawn(EffectType type, Vector3 position, Vector3 normal, Vector3 rotateAxis)
	{
		PoolableEffectBase effectObj = _pools[type].Spawn();

		effectObj.transform.position = position;
		effectObj.transform.rotation = Quaternion.LookRotation(normal, rotateAxis);

		return effectObj;
	}
	#endregion
}


public enum EffectType
{
	SolidImpact,
	CreatureImpact,
	CreatureDeath,
	MuzzleFlash,
	Explosion
}