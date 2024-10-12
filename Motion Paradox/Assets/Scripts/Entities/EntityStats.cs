using System.Collections;
using UnityEngine;

public abstract class EntityStats : MonoBehaviour, IDamageable
{
	[Header("References"), Space]
	[SerializeField] protected Rigidbody2D rb2D;
	[SerializeField] protected MonoBehaviour movementScript;

	[Header("Damage Text"), Space]
	[SerializeField] protected GameObject dmgTextPrefab;
	[SerializeField] protected Transform dmgTextLoc;

	[Header("Stats"), Space]
	[SerializeField] protected Stats stats;
	[SerializeField] protected float damageFlashTime;

	[Header("Health Bar"), Space]
	[SerializeField] protected HealthBar healthBar;

	public Vector2 Position => rb2D.position;

	// Protected fields.
	protected Material _mat;
	protected float _currentHealth;

	protected virtual void Start()
	{
		stats.ClearUpgrades();
		_currentHealth = stats.GetDynamicStat(Stat.MaxHealth);
		healthBar.SetMaxHealth(_currentHealth);
	}

	public virtual void TakeDamage(Stats attackerStats, Vector3 attackerPos, float scaleFactor = 1f)
	{
		float damage = attackerStats.GetDynamicStat(Stat.Damage) * scaleFactor;
		float knockBackStrength = attackerStats.GetStaticStat(Stat.KnockBackStrength) * scaleFactor;

		_currentHealth -= damage;
		_currentHealth = Mathf.Max(0f, _currentHealth);
		healthBar.SetCurrentHealth(_currentHealth);

		DamageText.Generate(dmgTextPrefab, dmgTextLoc.position, DamageTextStyle.Normal, damage.ToString());

		StartCoroutine(TriggerDamageFlash());
		StartCoroutine(BeingKnockedBack(attackerPos, knockBackStrength));

		if (_currentHealth <= 0)
			Die();
	}

	public abstract void Die();

	protected IEnumerator TriggerDamageFlash()
	{
		float flashIntensity;
		float elapsedTime = 0f;

		while (elapsedTime < damageFlashTime)
		{
			elapsedTime += Time.deltaTime;

			flashIntensity = Mathf.Lerp(1f, 0f, elapsedTime / damageFlashTime);
			_mat.SetFloat("_FlashIntensity", flashIntensity);

			yield return null;
		}
	}

	protected IEnumerator BeingKnockedBack(Vector3 attackerPos, float strength)
	{
		if (attackerPos == default)
			yield break;

		rb2D.velocity = Vector3.zero;
		movementScript.enabled = false;

		Vector2 direction = transform.position - attackerPos;
		float knockBackStrength = strength * (1f - stats.GetStaticStat(Stat.KnockBackRes));

		Vector2 force = direction.normalized * knockBackStrength;

		rb2D.AddForce(force, ForceMode2D.Impulse);

		yield return new WaitForSeconds(.25f);

		movementScript.enabled = true;
	}
}
