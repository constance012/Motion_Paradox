public interface ILevelable
{
	int CurrentLevel { get; }
	int CurrentExperience { get; }

	void GainExperience(int amount);
	void LevelUp();
}