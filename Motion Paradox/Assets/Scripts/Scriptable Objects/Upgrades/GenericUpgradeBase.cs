using System.Collections.Generic;
using UnityEngine;

public abstract class GenericUpgradeBase<TUnit> : UpgradeBase where TUnit : ScriptableObject
{
	[Header("Units List"), Space]
	public List<TUnit> unitsToApply = new List<TUnit>();
}