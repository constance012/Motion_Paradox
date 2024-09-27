using UnityEngine;
using CSTGames.Utility;

[CreateAssetMenu(menuName = "Items/Base Item", fileName = "New Item")]
public class Item : IdentifiableSO
{
	[Header("Category"), Space]
	[ReadOnly] public int slotIndex = -1;
	public ItemCategory category;

	[Header("Quantity and Stack"), Space]
	public bool stackable;
	public int maxPerStack = 1;
	public int quantity = 1;

	[Header("Specials"), Space]
	public bool canBeUsed;
	public bool autoUse;

	public virtual bool Use(Transform target, bool forced = false)
	{
		Debug.Log($"Using {displayName}...");
		return canBeUsed;
	}

	public override string ToString()
	{
		return $"Rarity: <b><color=#{ColorUtility.ToHtmlStringRGB(rarity.color)}> {rarity.title} </color></b>\n" +
				$"Category: <b> {category.ToString().AddWhitespaceBeforeCapital()} </b>\n" +
				$"{description}";
	}
}

public enum ItemCategory
{
	Null = 0,
	Consumable = 2,
	Material = 3,
	Coin = 4,
	KeyItem = 1
}
