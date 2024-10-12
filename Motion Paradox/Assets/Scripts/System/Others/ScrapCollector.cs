using UnityEngine;
using TMPro;

public sealed class ScrapCollector : Singleton<ScrapCollector>
{
	[Header("References"), Space]
	[SerializeField] private ScrapCountHUD scrapCount;

	public int Amount => _scraps;

	// Private fields.
	private int _scraps;

	public void UpdateAmount(int delta)
	{
		_scraps += delta;
		scrapCount.UpdateAmount(_scraps);
	}
}