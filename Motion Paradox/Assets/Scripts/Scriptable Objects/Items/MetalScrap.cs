using UnityEngine;

[CreateAssetMenu(menuName = "Items/Metal Scrap", fileName = "New Scrap")]
public sealed class MetalScrap : Item
{
	public override bool Use(Transform target, bool forced = false)
	{
		if (quantity > 0)
		{
			ScrapCollector.Instance.UpdateAmount(quantity);
			return true;
		}

		return false;
	}
}