using System;
using UnityEngine;

public class EnemyStats : Entity
{
	[Header("Drop Items"), Space]
	[SerializeField] private GameObject heart;

	public string ID { get; private set; }

	private void Awake()
	{
		_mat = this.GetComponentInChildren<SpriteRenderer>("Graphic/Enemy Sprite").material;
		ID = Guid.NewGuid().ToString();
	}

	protected override void Start()
	{
		base.Start();
		healthBar.name = $"{gameObject.name} Health Bar";
	}

	public override void Die()
	{
		EffectInstantiator.Instance.Instantiate<ParticleSystem>(EffectType.Explosion, transform.position, Quaternion.identity);
		Instantiate(heart, rb2D.position, Quaternion.identity);

		AudioManager.Instance.Play("Explosion");
		CameraShaker.Instance.ShakeCamera(4f, .2f);

		Destroy(healthBar.gameObject);
		Destroy(gameObject);
	}
}
