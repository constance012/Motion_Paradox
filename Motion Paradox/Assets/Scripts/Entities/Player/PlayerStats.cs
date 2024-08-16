using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : Entity
{
	[Header("Events"), Space]
	public UnityEvent onPlayerDies;

	public static bool IsDeath { get; set; }
	public bool CanBeHealed => _currentHealth < stats.GetDynamicStat(Stat.MaxHealth);

	// Private fields.
	private float _invincibilityTime;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void ResetStatic()
	{
		IsDeath = false;
	}

	private void Awake()
	{
		_mat = this.GetComponentInChildren<SpriteRenderer>("Graphic/Player Sprite").material;
	}

	protected override void Start()
	{
		base.Start();

		_invincibilityTime = stats.GetStaticStat(Stat.InvincibilityTime);
		IsDeath = false;
	}

	private void Update()
	{
		if (_invincibilityTime > 0f)
			_invincibilityTime -= Time.deltaTime;
	}

	public override void TakeDamage(float amount, bool weakpointHit, Vector3 attackerPos = default, float knockBackStrength = 0f)
	{
		if (_currentHealth > 0 && _invincibilityTime <= 0f)
		{
			base.TakeDamage(amount, weakpointHit, attackerPos, knockBackStrength);

			CameraShaker.Instance.ShakeCamera(2.5f, .3f);
			EffectInstantiator.Instance.Instantiate<ParticleSystem>(EffectType.CreatureImpact, transform.position, Random.insideUnitCircle.normalized);

			_invincibilityTime = stats.GetStaticStat(Stat.InvincibilityTime);
		}
	}

	public void Heal(int amount)
	{
		if (_currentHealth > 0)
		{
			_currentHealth += amount;
			_currentHealth = Mathf.Min(_currentHealth, stats.GetDynamicStat(Stat.MaxHealth));

			healthBar.SetCurrentHealth(_currentHealth);

			DamageText.Generate(dmgTextPrefab, dmgTextLoc.position, DamageText.HealingColor, DamageTextStyle.Normal, amount.ToString());
		}
	}

	public override void Die()
	{
		EffectInstantiator.Instance.Instantiate<ParticleSystem>(EffectType.CreatureDeath, transform.position, Quaternion.identity);

		IsDeath = true;
		onPlayerDies?.Invoke();
		
		GameManager.Instance.ShowGameOverScreen();
		gameObject.SetActive(false);
	}
}
