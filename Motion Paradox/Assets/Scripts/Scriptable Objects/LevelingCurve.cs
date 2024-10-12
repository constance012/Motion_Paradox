using UnityEngine;

[CreateAssetMenu(fileName = "New Leveling Curve", menuName = "Leveling Curve")]
public sealed class LevelingCurve : ScriptableObject
{
	[Header("Curve Settings"), Space]
	[SerializeField] private float baseXP;
	[SerializeField, Min(1f)] private float exponent;
	[SerializeField, Min(1f)] private int maxLevel;
	[SerializeField, ReadOnly] private int[] _xpRequirement;

	[Header("Visualizing Curve"), Space]
	[SerializeField] private AnimationCurve visualizingCurve;

	public int MaxLevel => maxLevel;
	public bool CurveIsNullOrEmpty => _xpRequirement == null || _xpRequirement.Length == 0;

	// Private fields.

	[ContextMenu("Calculate Curve")]
	public void CalculateCurve()
	{
		visualizingCurve.ClearKeys();
		_xpRequirement = new int[maxLevel];

		for (int i = 0; i < maxLevel; i++)
		{
			_xpRequirement[i] = Mathf.FloorToInt(baseXP * Mathf.Pow(i + 1, exponent));
			visualizingCurve.AddKey(i, _xpRequirement[i]);
		}
	}

	public int GetRequirementForLevel(int level)
	{
		if (CurveIsNullOrEmpty)
			CalculateCurve();

		if (level < MaxLevel)
		{
			return _xpRequirement[level - 1];
		}
		
		Debug.LogWarning("Max level reached, returns requirement of the last level.");
		return _xpRequirement[MaxLevel - 2];
	}
}