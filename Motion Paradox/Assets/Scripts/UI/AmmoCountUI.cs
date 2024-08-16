using UnityEngine;

public class AmmoCountUI : MonoBehaviour
{
	[Header("UI Prefabs"), Space]
	[SerializeField] private GameObject normalBulletUIPrefab;
	[SerializeField] private GameObject piercingBulletUIPrefab;

	private void Awake()
	{
		transform.DestroyAllChildren(Destroy);
	}

	public void Initialize(int maxAmmo, int piercingShotFrequency)
	{
		for (int i = 0; i < maxAmmo; i++)
		{
			bool isPiercingShot = (i + 1) % piercingShotFrequency == 0;
			GameObject prefab = isPiercingShot ? piercingBulletUIPrefab : normalBulletUIPrefab;
			Instantiate(prefab, transform);
		}
	}

	public void AddBullet(bool isPiercingShot)
	{
		GameObject prefab = isPiercingShot ? piercingBulletUIPrefab : normalBulletUIPrefab;
		Instantiate(prefab, transform);
	}

	public void RemoveTop()
	{
		Destroy(transform.GetChild(0).gameObject);
	}
}