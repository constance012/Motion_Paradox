using UnityEngine;

[CreateAssetMenu(menuName = "Items/Healing Item", fileName = "New Healing Item")]
public class HealingItem : Item
{
	[Header("Healing amount"), Space]
	public int healingAmount;

	public override bool Use(Transform target, bool forced = false)
	{
		if (quantity > 0 && canBeUsed)
		{
			IHealable healable = target.GetComponent<IHealable>();

			if (healable.CanBeHealed || forced)
			{
				healable.Heal(healingAmount);
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
