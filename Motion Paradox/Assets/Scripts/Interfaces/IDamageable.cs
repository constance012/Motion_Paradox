using UnityEngine;

public interface IDamageable
{
	Vector2 Position { get; }
	void TakeDamage(Stats attackerStats, Vector3 attackerPos, float scaleFactor = 1f);
}