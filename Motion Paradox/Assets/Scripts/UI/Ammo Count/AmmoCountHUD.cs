using System.Collections;
using System.Collections.Generic;
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
	private Queue<CartridgeUI> _bullets;

	private void Awake()
	{
		_bullets = new Queue<CartridgeUI>();
		transform.DestroyAllChildren(Destroy);
	}

	public void Initialize(int maxAmmo, int piercingShotFrequency)
	{
		float spacing = 0f;

		for (int i = 0; i < maxAmmo; i++)
		{
			bool isPiercingShot = (i + 1) % piercingShotFrequency == 0;
			
			CartridgeUI bulletUI = InstantiateBullet(isPiercingShot);

			bulletUI.MoveLocalY(-spacing);
			bulletUI.LoadRound(chamber: i == 0);

			spacing += bulletSpacing;
		}
	}

	public void AddBottom(bool isPiercingShot)
	{
		CartridgeUI bulletUI = InstantiateBullet(isPiercingShot);

		bulletUI.MoveLocalY((_bullets.Count - 1) * -bulletSpacing);
		bulletUI.LoadRound();

	}

	public void RemoveTop()
	{
		StartCoroutine(RemoveFiredBullet());
	}

	private IEnumerator RemoveFiredBullet()
	{
		Tween fireTween = _bullets.Dequeue().FireRound();

		yield return fireTween.WaitForCompletion();

		PushRoundsUp();
	}

	private CartridgeUI InstantiateBullet(bool isPiercingShot)
	{
		GameObject prefab = isPiercingShot ? piercingBulletUIPrefab : normalBulletUIPrefab;
		
		CartridgeUI bullet = Instantiate(prefab, transform).GetComponent<CartridgeUI>();
		_bullets.Enqueue(bullet);

		return bullet;
	}

	private void PushRoundsUp()
	{
		int index = 0;
		foreach (CartridgeUI bullet in _bullets)
		{
			bullet.PushUpRound(bulletSpacing, chamber: index == 0);
			index++;
		}
	}
}