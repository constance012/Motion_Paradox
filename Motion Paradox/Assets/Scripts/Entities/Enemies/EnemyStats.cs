﻿using System;
using UnityEngine;

public sealed class EnemyStats : EntityStats
{
	[Header("Dropped Loots"), Space]
	[SerializeField] private EnemyLootTrigger lootTrigger;

	public string ID { get; private set; }

	private void Awake()
	{
		_mat = this.GetComponentInChildren<SpriteRenderer>("Graphic/Sprite").material;
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
		AudioManager.Instance.Play("Explosion");
		CameraShaker.Instance.ShakeCamera(4f, .2f);

		lootTrigger.DispenseLoots();

		DestroyGameObject();
	}

	public void DestroyGameObject()
	{
		Destroy(healthBar.gameObject);
		Destroy(gameObject);
	}
}
