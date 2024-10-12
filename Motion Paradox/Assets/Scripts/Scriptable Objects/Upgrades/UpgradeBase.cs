using UnityEngine;

public abstract class UpgradeBase : IdentifiableSO
{
	[Header("Type"), Space]
	public UpgradeValueType type;

	[Header("Currency Cost"), Space]
	public int baseCost;
	[Min(0f)] public float exponent;

	// Properties.
	public bool IsApplied { get; set; }
    
    public abstract void DoUpgrade();
	public abstract void RemoveUpgrade();

	public int GetCostAtLevel(int level)
	{
		return Mathf.FloorToInt(baseCost * Mathf.Pow(level, exponent));
	}
}

public enum UpgradeValueType
{
	Flat,
	Percentage
}