using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Ammunition Upgrade", fileName = "New Ammunition Upgrade")]
public class AmmunitionUpgrade : FirearmUpgradeBase
{
	[Header("Details"), Space]
	public int maxAmmo;
	public int piercingShotFrequency;
	public float fireRate;
	public float reloadInterval;

	public override void DoUpgrade()
	{
		if (!IsApplied)
		{
			Debug.Log($"Applying \"{this.displayName}\" upgrade...");
			ApplyChanges(1);
		}
	}

	public override void RemoveUpgrade()
	{
		if (IsApplied)
		{
			Debug.Log($"Removing \"{this.displayName}\" upgrade...");
			ApplyChanges(-1);
		}
	}

	private void ApplyChanges(int sign)
	{
		foreach (Gun gun in unitsToApply)
		{
			gun.maxAmmo += maxAmmo * sign;

			if (gun.piercingShotFrequency == int.MaxValue)
				gun.piercingShotFrequency = piercingShotFrequency;
			else
				gun.piercingShotFrequency += piercingShotFrequency * sign;

			if (type == UpgradeValueType.Percentage)
			{
				if (sign == 1)
				{
					gun.fireRate *= 1f + fireRate;
					gun.reloadInterval *= 1f + reloadInterval;
				}
				else
				{
					gun.fireRate /= 1f + fireRate;
					gun.reloadInterval /= 1f + reloadInterval;
				}
			}
			else
			{
				gun.fireRate += fireRate * sign;
				gun.reloadInterval += reloadInterval * sign;
			}
		}

		IsApplied = sign == 1;
	}
}