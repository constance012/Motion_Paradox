using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class Consumable : Item
{
	public enum HealingType
	{
		Health,
		Mana
	}

	[Header("Healing amount"), Space]
	public HealingType healingType;
	public int healingAmount;

	public override bool Use(bool forced = false)
	{
		throw new NotImplementedException();
	}

	public override string ToString()
	{
		return base.ToString() + "\n" +
				$"<b> +{healingAmount} HP. </b>\n" +
				$"<b> Right Click to use. </b>";
	}
}
