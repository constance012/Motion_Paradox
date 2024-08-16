using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class HealingItem : Item
{
	[Header("Healing amount"), Space]
	public int healingAmount;

	public override bool Use(Transform player, bool forced = false)
	{
		if (quantity > 0 && canBeUsed)
		{
			PlayerStats playerStats = player.GetComponent<PlayerStats>();

			if (playerStats.CanBeHealed || forced)
			{
				playerStats.Heal(healingAmount);
				return true;
			}
		}

		return false;
	}

	public override string ToString()
	{
		return base.ToString() + "\n" +
				$"<b> +{healingAmount} HP. </b>\n" +
				$"<b> Right Click to use. </b>";
	}
}
