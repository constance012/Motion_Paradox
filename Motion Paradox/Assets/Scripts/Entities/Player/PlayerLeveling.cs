using UnityEngine;
using UnityEngine.Events;

public sealed class PlayerLeveling : EntityLeveling
{
	[Header("UI References"), Space]
	[SerializeField] private LevelSlider slider;

	[Header("Events"), Space]
	public UnityEvent<int> onPlayerLeveledUp;

	protected override void Start()
	{		
		base.Start();
		slider.SetLevelRange(CurrentLevel, CurrentExperience, 0, _currentLevelRequirement);
	}

	public override void LevelUp()
	{
		base.LevelUp();
		onPlayerLeveledUp?.Invoke(CurrentLevel);

		slider.SetLevelRange(CurrentLevel, CurrentExperience, levelingCurve.GetRequirementForLevel(CurrentLevel - 1), _currentLevelRequirement);
	}

	protected override void UpdateUI()
	{
		slider.UpdateExperience(CurrentExperience);
	}
}