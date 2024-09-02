using UnityEngine;

public interface IDamageable
{
	Vector2 Position { get; }
	void Damage(Stats attackerStats, Vector3 attackerPos, float scaleFactor = 1f);
}