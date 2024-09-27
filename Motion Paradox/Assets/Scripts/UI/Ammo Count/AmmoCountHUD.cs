using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public sealed class AmmoCountHUD : MonoBehaviour
{
	[Header("UI Prefabs"), Space]
	[SerializeField] private GameObject normalBulletUIPrefab;
	[SerializeField] private GameObject piercingBulletUIPrefab;

	[Header("Spacing Settings"), Space]
	[SerializeField] private float bulletSpacing;

	// Private fields.
	private const string PIERCING_BULLET_NAME = "Piercing Bullet UI";
	private const string NORMAL_BULLET_NAME = "Normal Bullet UI";
	private ObjectPool<CartridgeUI> _bulletUIPool;
	private Queue<CartridgeUI> _magazine;

	private void Awake()
	{
		_bulletUIPool ??= new ObjectPool<CartridgeUI>(parent: transform);
		_magazine ??= new Queue<CartridgeUI>();
	}

	public void Initialize(int maxAmmo, int piercingShotFrequency)
	{
		transform.DestroyAllChildren(Destroy);

		float spacing = 0f;
		for (int i = 0; i < maxAmmo; i++)
		{
			bool isPiercingShot = (i + 1) % piercingShotFrequency == 0;
			
			CartridgeUI bulletUI = AllocateBullet(isPiercingShot);

			bulletUI.Initialize(-spacing);
			bulletUI.LoadRound(chamber: i == 0);

			spacing += bulletSpacing;
		}
	}

	public void AddBottom(bool isPiercingShot)
	{
		string bulletName = isPiercingShot ? PIERCING_BULLET_NAME : NORMAL_BULLET_NAME;
		CartridgeUI bulletUI = _bulletUIPool.Spawn(bullet => bullet.name.Equals(bulletName) && !bullet.gameObject.activeInHierarchy);

		bulletUI.Initialize(_magazine.Count * -bulletSpacing);
		bulletUI.LoadRound();

		_magazine.Enqueue(bulletUI);
	}

	public void RemoveTop()
	{
		StartCoroutine(RemoveFiredBullet());
	}

	private IEnumerator RemoveFiredBullet()
	{
		Tween fireTween = _magazine.Dequeue().FireRound();

		yield return fireTween.WaitForCompletion();

		PushRoundsUp();
	}

	private void PushRoundsUp()
	{
		int index = 0;
		foreach (CartridgeUI bullet in _magazine)
		{
			bullet.PushUpRound(bulletSpacing, chamber: index == 0);
			index++;
		}
	}

	private CartridgeUI AllocateBullet(bool isPiercingShot)
	{
		GameObject prefab = isPiercingShot ? piercingBulletUIPrefab : normalBulletUIPrefab;
		
		CartridgeUI bullet = _bulletUIPool.Prefill(prefab);
		bullet.name = isPiercingShot ? PIERCING_BULLET_NAME : NORMAL_BULLET_NAME;
		
		_magazine.Enqueue(bullet);

		return bullet;
	}
}