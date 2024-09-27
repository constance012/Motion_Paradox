using UnityEngine;

[CreateAssetMenu(menuName = "Items/Healing Item", fileName = "New Healing Item")]
public sealed class HealingItem : Item
{
	[Header("Healing amount"), Space]
	public int healingAmount;

	public override bool Use(Transform target, bool forced = false)
	{
		if (quantity > 0 && canBeUsed)
		{
			IHealable healable = target.GetComponent<IHealable>();

			if (healable != null && (healable.CanBeHealed || forced))
			{
				healable.Heal(healingAmount);
				return true;
			}
		}

		return false;
	}
}
