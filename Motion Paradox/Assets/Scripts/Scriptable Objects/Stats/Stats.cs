using UnityEngine;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using CSTGames.Utility;

[CreateAssetMenu(menuName = "Unit Stats/Stats", fileName = "New Blank Stats")]
public class Stats : ScriptableObject
{
	[Header("Stats"), Space]
	[Tooltip("Dynamic stats are GLOBAL stats shared between objects, which CAN be modified by upgrades.")]
	public SerializedDictionary<Stat, float> dynamicStats = new SerializedDictionary<Stat, float>();

	[Tooltip("Static stats are GLOBAL stats shared between objects, which CAN NOT be modified by upgrades.")]
	public SerializedDictionary<Stat, float> staticStats = new SerializedDictionary<Stat, float>();

	// Private fields.
	private readonly HashSet<StatsUpgrade> _appliedUpgrades = new HashSet<StatsUpgrade>();
	private readonly HashSet<Stat> _toStringIgnoreStats = new HashSet<Stat>()
	{
		Stat.InvincibilityTime,
		Stat.ProjectileSpeed,
		Stat.ProjectileLifeTime,
		Stat.ProjectileTrackingRigidity,
	};

	public void AddUpgrade(StatsUpgrade upgrade)
	{
		if (!_appliedUpgrades.Contains(upgrade))
			_appliedUpgrades.Add(upgrade);
	}

	public void RemoveUpgrade(StatsUpgrade upgrade)
	{
		_appliedUpgrades.Remove(upgrade);
	}

	public void ClearUpgrades()
	{
		_appliedUpgrades.Clear();
	}

	public float GetStaticStat(Stat statName)
	{
		if (staticStats.TryGetValue(statName, out float value))
			return value;
		else
		{
			string statString = statName.ToString().AddWhitespaceBeforeCapital();
			Debug.LogWarning($"No STATIC stat value found for \"{statString}\" on {this.name}");
			return -1f;
		}
	}
	
	public float GetDynamicStat(Stat statName)
	{
		if (dynamicStats.TryGetValue(statName, out float baseValue))
			return GetUpgradedValue(statName, baseValue);
		else
		{
			string statString = statName.ToString().AddWhitespaceBeforeCapital();
			Debug.LogWarning($"No DYNAMIC stat value found for \"{statString}\" on {this.name}");
			return -1f;
		}
	}

	public void ModifyStat(Stat statName, float delta)
	{
		if (dynamicStats.TryGetValue(statName, out float _))
		{
			dynamicStats[statName] += delta;
		}
		else
		{
			Debug.LogError($"No DYNAMIC stat value found for {statName} on {this.name}");
		}
	}

	private float GetUpgradedValue(Stat stat, float baseValue)
	{
		foreach (StatsUpgrade upgrade in _appliedUpgrades)
		{
			if (!upgrade.affectedStats.TryGetValue(stat, out float upgradeValue))
				continue;
			
			if (upgrade.type == UpgradeValueType.Percentage)
				baseValue *= 1f + upgradeValue;
			else
				baseValue += upgradeValue;
		}

		return baseValue;
	}

    public override string ToString()
    {
        string result = "";
		foreach (KeyValuePair<Stat, float> stat in dynamicStats)
		{
			if (!_toStringIgnoreStats.Contains(stat.Key))
				result += $"{stat.Key.ToString().AddWhitespaceBeforeCapital()}: {stat.Value}\n";
		}
		result += "\n";
		foreach (KeyValuePair<Stat, float> stat in staticStats)
		{
			if (!_toStringIgnoreStats.Contains(stat.Key))
				result += $"{stat.Key.ToString().AddWhitespaceBeforeCapital()}: {stat.Value}\n";
		}
		return result.TrimEnd('\r', '\n');
    }
}

public enum Stat
{
	// Dynamic.
	MaxHealth,
	Damage,
	AttackSpeed,
	MoveSpeed,
	InvincibilityTime,

	// Static.
	KnockBackStrength,
	KnockBackRes,
	ProjectileSpeed,
	ProjectileTrackingRigidity,
	ProjectileLifeTime
}