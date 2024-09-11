using UnityEngine;
using UnityEngine.Events;

public sealed class PlayerStats : EntityStats, IHealable
{
	[Header("Events"), Space]
	public UnityEvent onPlayerDies;

	public static bool IsDeath { get; set; }
	public bool CanBeHealed => _currentHealth < stats.GetDynamicStat(Stat.MaxHealth);

	// Private fields.
	private float _invincibilityTime;

	private void Awake()
	{
		_mat = this.GetComponentInChildren<SpriteRenderer>("Graphic/Player Sprite").material;
		IsDeath = false;
	}

	protected override void Start()
	{
		base.Start();
		_invincibilityTime = stats.GetStaticStat(Stat.InvincibilityTime);
	}

	private void Update()
	{
		if (_invincibilityTime > 0f)
			_invincibilityTime -= Time.deltaTime;
	}

	protected override void TakeDamage(Stats attackerStats, Vector3 attackerPos, float scaleFactor)
	{
		if (_currentHealth > 0 && _invincibilityTime <= 0f)
		{
			base.TakeDamage(attackerStats, attackerPos, scaleFactor);

			CameraShaker.Instance.ShakeCamera(2.5f, .3f);
			AudioManager.Instance.PlayWithRandomPitch("Taking Damage", .7f, 1.2f);
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
