using System;
using UnityEngine;
using UnityEngine.Audio;

public sealed class SettingsMenu : MonoBehaviour
{
	[Header("Menu References"), Space]
	[SerializeField] private TweenableUIMaster settingsMenu;
	[SerializeField] private TweenableUIMaster mainMenu;

	[Header("Audio Mixer"), Space]
	[SerializeField] private AudioMixer mixer;

	[Header("Slider Groups"), Space]
	[SerializeField] private AudioSlider masterSlider;
	[SerializeField] private AudioSlider musicSlider;
	[SerializeField] private AudioSlider soundSlider;
	[SerializeField] private AudioSlider ambienceSlider;
	[SerializeField] private AudioSlider aimSpeedSlider;
	[SerializeField] private AudioSlider dialogueSpeedSlider;

	[Header("Directional Selectors"), Space]
	[SerializeField] private DirectionalSelector qualitySelector;
	[SerializeField] private DirectionalSelector framerateSelector;
	[SerializeField] private DirectionalSelector vsyncSelector;

	private void OnEnable()
	{
		ReloadUI();
	}

	#region Callback Methods for UI.
	public async void OpenMainMenu()
	{
		await settingsMenu.SetActive(false);
		await mainMenu.SetActive(true);
	}

	public void SetMasterVolume(float amount)
	{
		mixer.SetFloat("masterVol", masterSlider.ValueAsMixerDecibel);

		masterSlider.DisplayText = ConvertDecibelToText(amount);
		UserSettings.MasterVolume = amount;
	}
	
	public void SetMusicVolume(float amount)
	{
		mixer.SetFloat("musicVol", musicSlider.ValueAsMixerDecibel);

		musicSlider.DisplayText = ConvertDecibelToText(amount);
		UserSettings.MusicVolume = amount;
	}

	public void SetSoundVolume(float amount)
	{
		mixer.SetFloat("soundVol", soundSlider.ValueAsMixerDecibel);

		soundSlider.DisplayText = ConvertDecibelToText(amount);
		UserSettings.SoundVolume = amount;
	}

	public void SetAmbienceVolume(float amount)
	{
		mixer.SetFloat("ambienceVol", ambienceSlider.ValueAsMixerDecibel);

		ambienceSlider.DisplayText = ConvertDecibelToText(amount);
		UserSettings.AmbienceVolume = amount;
	}

	public void SetQualityLevel(int index)
	{
		QualitySettings.SetQualityLevel(index);
		UserSettings.QualityLevel = index;
	}

	public void SetTargetFramerate(string value)
	{
		try
		{
			int fps = Convert.ToInt32(value);
			Application.targetFrameRate = Convert.ToInt32(fps);
			UserSettings.TargetFramerate = fps;
		}
		catch (FormatException)
		{
			Application.targetFrameRate = 60;
			UserSettings.TargetFramerate = 60;
		}
	}

	public void SetVsync(int useVsync)
	{
		Debug.Log($"Use Vsync: {Convert.ToBoolean(useVsync)}.");
		QualitySettings.vSyncCount = useVsync;
		UserSettings.UseVsync = useVsync;
	}

	public void SetAimSpeed(float amount)
	{
		float value = amount / 10f;
		aimSpeedSlider.DisplayText = value.ToString("0.0");
		UserSettings.AimSpeed = value;
	}
	
	public void SetDialogueSpeed(float amount)
	{
		dialogueSpeedSlider.DisplayText = amount.ToString();
		UserSettings.DialogueSpeed = (int)amount;
	}

	public void ResetToDefault()
	{
		UserSettings.ResetToDefault(UserSettings.SettingSection.All);
		ReloadUI();
	}
	#endregion

	#region Utility Functions.
	private string ConvertDecibelToText(float amount)
	{
		return (amount * 100f).ToString("0");
	}
	#endregion

	private void ReloadUI()
	{
		float masterVol = UserSettings.MasterVolume;
		float musicVol = UserSettings.MusicVolume;
		float soundVol = UserSettings.SoundVolume;
		float ambienceVol = UserSettings.AmbienceVolume;
		float aimSpeed = UserSettings.AimSpeed;
		int dialogueSpeed = UserSettings.DialogueSpeed;

		masterSlider.Value = masterVol;
		musicSlider.Value = musicVol;
		soundSlider.Value = soundVol;
		ambienceSlider.Value = ambienceVol;
		aimSpeedSlider.Value = aimSpeed * 10f;
		dialogueSpeedSlider.Value = dialogueSpeed;

		masterSlider.DisplayText = ConvertDecibelToText(masterVol);
		musicSlider.DisplayText = ConvertDecibelToText(musicVol);
		soundSlider.DisplayText = ConvertDecibelToText(soundVol);
		ambienceSlider.DisplayText = ConvertDecibelToText(ambienceVol);
		aimSpeedSlider.DisplayText = aimSpeed.ToString("0.0");
		dialogueSpeedSlider.DisplayText = dialogueSpeed.ToString();

		qualitySelector.Index = UserSettings.QualityLevel;
		framerateSelector.Value = UserSettings.TargetFramerate.ToString();
		vsyncSelector.Index = UserSettings.UseVsync;
	}
}
