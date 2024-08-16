using System.Collections;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
	[Header("Component References"), Space]
	[SerializeField] protected Rigidbody2D rb2D;
	[SerializeField] protected MonoBehaviour movementBehaviour;

	[Header("Damage Text"), Space]
	[SerializeField] protected GameObject dmgTextPrefab;
	[SerializeField] protected Transform dmgTextLoc;

	[Header("Stats"), Space]
	[SerializeField] protected Stats stats;
	[SerializeField] protected float damageFlashTime;

	[Header("Health Bar"), Space]
	[SerializeField] protected HealthBar healthBar;

	// Protected fields.
	protected Material _mat;
	protected float _currentHealth;

	protected virtual void Start()
	{
		_currentHealth = stats.GetDynamicStat(Stat.MaxHealth);
		healthBar.SetMaxHealth(_currentHealth);
	}

	public virtual void TakeDamage(float amount, bool weakpointHit, Vector3 attackerPos = default, float knockBackStrength = 0f)
	{
		AudioManager.Instance.PlayWithRandomPitch("Taking Damage", .7f, 1.2f);
		
		_currentHealth -= amount;
		_currentHealth = Mathf.Max(0f, _currentHealth);
		healthBar.SetCurrentHealth(_currentHealth);

		DamageTextStyle style = weakpointHit ? DamageTextStyle.Critical : DamageTextStyle.Normal;
		DamageText.Generate(dmgTextPrefab, dmgTextLoc.position, style, amount.ToString());

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
		movementBehaviour.enabled = false;
		movementBehaviour.StopAllCoroutines();

		Vector2 direction = transform.position - attackerPos;
		float knockBackStrength = strength * (1f - stats.GetStaticStat(Stat.KnockBackRes));

		Vector2 force = direction.normalized * knockBackStrength;

		rb2D.AddForce(force, ForceMode2D.Impulse);

		yield return new WaitForSeconds(.3f);

		movementBehaviour.enabled = true;
	}
}
