using System.Collections;
using UnityEngine;

public sealed class DestroyerGearAction : RangedEnemyAction
{
	[Header("Attack Pattern")]
	[Header("Shot Volleys"), Space]
	[SerializeField] private Vector2Int shotsPerVolleyRange;
	[SerializeField] private float angleBetweenShots;

	[Header("Missiles"), Space]
	[SerializeField] private GameObject missilePrefab;
	[SerializeField] private Transform[] launchPods;
	[SerializeField, Tooltip("Launch a missile every nth shot.")] private float missileFrequency;
	[SerializeField] private float missileTrackingRigidity;

	// Private fields.
	private int _shotCount = 0;

	protected override IEnumerator DoAttack()
	{
		if (DistanceToPlayer <= attackRadius && !_AI.IsRetreating)
		{
			rb2d.velocity = Vector2.zero;
			movementScript.enabled = false;

			if (++_shotCount % missileFrequency == 0)
				LaunchMissile();
			
			FireVolley();
			ResetAttackInterval();

			yield return new WaitForSeconds(recoverTime);
			
			movementScript.enabled = true;
		}
	}

	private void FireVolley()
	{
		int shotsPerVolley = shotsPerVolleyRange.RandomBetweenEnds();
		float offsetAngle = angleBetweenShots * (shotsPerVolley - 1) / 2f;
		float angle = 0;
		
		for (int i = 0; i < shotsPerVolley; i++)
		{
			Vector3 direction = Quaternion.Euler(0f, 0f, angle - offsetAngle) * firePoint.right;
			FireProjectile(direction, generateRecoil: false);
			angle += angleBetweenShots;
		}

		GenerateRecoil();
	}

	private void LaunchMissile()
	{
		int index = Random.Range(0, launchPods.Length);
		Vector3 launchPos = launchPods[index].position;
		Vector3 launchDir = launchPods[index].right;

		GameObject missile = Instantiate(missilePrefab, launchPos, Quaternion.identity);
		missile.transform.right = launchDir;
		
		ProjectileBase projectile = missile.GetComponent<ProjectileBase>();
		projectile.Initialize(transform, stats, _player);
		projectile.trackingRigidity = missileTrackingRigidity;
	}
}