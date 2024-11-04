using UnityEngine;

[AddComponentMenu("Object Pools/Projectile Pool")]
public sealed class ProjectilePool : MultiplePrefabsPool<ProjectilePool, ProjectileType, ProjectileBase>
{
	
}

public enum ProjectileType
{
	NormalBullet,
	PiercingBullet,
	MetalPellet,
	MetalShard,
	MetalMissile
}