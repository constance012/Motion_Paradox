using UnityEngine;

[CreateAssetMenu(menuName = "Items/Experience Container", fileName = "New Experience Container")]
public sealed class ExperienceContainer : Item
{
	public override bool Use(Transform target, bool forced = false)
	{
		ILevelable levelable = target.GetComponent<ILevelable>();
		levelable.GainExperience(quantity);
		return true;
	}
}