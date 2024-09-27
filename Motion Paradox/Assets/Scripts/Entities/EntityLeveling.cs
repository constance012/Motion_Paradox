using UnityEngine;

public abstract class EntityLeveling : MonoBehaviour, ILevelable
{
	[Header("Leveling Curve"), Space]
	[SerializeField] protected LevelingCurve levelingCurve;

	public int CurrentLevel { get; protected set; } = 1;
	public int CurrentExperience { get; protected set; } = 0;

	// Protected fields.
	private int _maxLevel;
	protected int _currentLevelRequirement;

	protected virtual void Start()
	{
		_maxLevel = levelingCurve.MaxLevel;
		_currentLevelRequirement = levelingCurve.GetRequirementForLevel(CurrentLevel);
	}
	
	public void GainExperience(int amount)
	{
		if (amount <= 0 || CurrentLevel == _maxLevel)
			return;

		CurrentExperience += amount;
		UpdateUI();
		
		if (CurrentExperience >= _currentLevelRequirement)
		{
			LevelUp();
		}
	}

	public virtual void LevelUp()
	{
		Debug.Log($"{gameObject.name} leveled up!");
		_currentLevelRequirement = levelingCurve.GetRequirementForLevel(++CurrentLevel);

		if (CurrentExperience > _currentLevelRequirement)
			CurrentExperience = _currentLevelRequirement;
	}

	protected abstract void UpdateUI();
}