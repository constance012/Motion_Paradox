using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(menuName = "Upgrades/Stats Upgrade", fileName = "New Stats Upgrade")]
public class StatsUpgrade : GenericUpgradeBase<Stats>
{
	[Header("Detail"), Space]
	public SerializedDictionary<Stat, float> affectedStats = new SerializedDictionary<Stat, float>();

	public override void DoUpgrade()
	{
		if (!IsApplied)
		{
			Debug.Log($"Applying \"{this.displayName}\" upgrade...");
			unitsToApply.ForEach(unit => unit.AddUpgrade(this));
			IsApplied = true;
		}
	}

	public override void RemoveUpgrade()
	{
		if (IsApplied)
		{
			Debug.Log($"Removing \"{this.displayName}\" upgrade...");
			unitsToApply.ForEach(unit => unit.RemoveUpgrade(this));
			IsApplied = false;
		}
	}
}