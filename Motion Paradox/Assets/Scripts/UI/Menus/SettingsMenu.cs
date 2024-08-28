using System;
using UnityEngine;
using UnityEngine.Audio;

public sealed class SettingsMenu : MonoBehaviour
{
	[Header("Menu References"), Space]
	[SerializeField] private TweenableUIElement settingsMenu;
	[SerializeField] private TweenableUIElement mainMenu;

	[Header("Audio Mixer"), Space]
	[SerializeField] private AudioMixer mixer;

	[Header("Slider Groups"), Space]
	[SerializeField] private SliderGroup musicSlider;
	[SerializeField] private SliderGroup soundSlider;
	[SerializeField] private SliderGroup ambienceSlider;
	[SerializeField] private SliderGroup aimSpeedSlider;

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

	public void SetMusicVolume(float amount)
	{
		mixer.SetFloat("musicVol", UserSettings.ToMixerDecibel(amount));

		musicSlider.DisplayText = ConvertDecibelToText(amount);
		UserSettings.MusicVolume = amount;
	}

	public void SetSoundVolume(float amount)
	{
		mixer.SetFloat("soundVol", UserSettings.ToMixerDecibel(amount));

		soundSlider.DisplayText = ConvertDecibelToText(amount);
		UserSettings.SoundVolume = amount;
	}

	public void SetAmbienceVolume(float amount)
	{
		mixer.SetFloat("ambienceVol", UserSettings.ToMixerDecibel(amount));

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
		float musicVol = UserSettings.MusicVolume;
		float soundVol = UserSettings.SoundVolume;
		float ambienceVol = UserSettings.AmbienceVolume;
		float aimSpeed = UserSettings.AimSpeed;

		musicSlider.Value = musicVol;
		soundSlider.Value = soundVol;
		ambienceSlider.Value = ambienceVol;
		aimSpeedSlider.Value = aimSpeed * 10f;

		musicSlider.DisplayText = ConvertDecibelToText(musicVol);
		soundSlider.DisplayText = ConvertDecibelToText(soundVol);
		ambienceSlider.DisplayText = ConvertDecibelToText(ambienceVol);
		aimSpeedSlider.DisplayText = aimSpeed.ToString("0.0");

		qualitySelector.Index = UserSettings.QualityLevel;
		framerateSelector.Value = UserSettings.TargetFramerate.ToString();
		vsyncSelector.Index = UserSettings.UseVsync;
	}
}
