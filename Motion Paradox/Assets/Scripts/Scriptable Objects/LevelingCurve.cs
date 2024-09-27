using UnityEngine;

[CreateAssetMenu(fileName = "New Leveling Curve", menuName = "Leveling Curve")]
public sealed class LevelingCurve : ScriptableObject
{
	[Header("Experience Requirements"), Space]
	public int[] xpRequirement;

	public int MaxLevel => xpRequirement.Length + 1;

	public int GetRequirementForLevel(int level)
	{
		if (level < MaxLevel)
		{
			return xpRequirement[level - 1];
		}
		
		Debug.LogWarning("Max level reached, returns requirement of the last level.");
		return xpRequirement[MaxLevel - 2];
	}
}