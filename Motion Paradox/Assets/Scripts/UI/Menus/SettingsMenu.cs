using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
	[Header("Audio Mixer"), Space]
	[SerializeField] private AudioMixer mixer;

	[Header("UI References"), Space]
	[SerializeField] private Slider _masterSlider;
	[SerializeField] private Slider _musicSlider;
	[SerializeField] private Slider _soundSlider;

	// Private fields
	private TextMeshProUGUI _musicText;
	private TextMeshProUGUI _soundText;
	private TextMeshProUGUI _masterText;

	private void Awake()
	{
		_musicText = _musicSlider.GetComponentInChildren<TextMeshProUGUI>();
		_soundText = _soundSlider.GetComponentInChildren<TextMeshProUGUI>();
		_masterText = _masterSlider.GetComponentInChildren<TextMeshProUGUI>();
	}

	private void OnEnable()
	{
		ReloadUI();
	}

	#region Callback Method for UI.
	public void SetMasterVolume(float amount)
	{
		mixer.SetFloat("masterVol", GetVolume(amount));

		_masterText.text = $"Master: {ConvertDecibelToText(amount)}";
		UserSettings.MasterVolume = amount;
	}

	public void SetMusicVolume(float amount)
	{
		mixer.SetFloat("musicVol", GetVolume(amount));

		_musicText.text = $"Music: {ConvertDecibelToText(amount)}";
		UserSettings.MusicVolume = amount;
	}

	public void SetSoundVolume(float amount)
	{
		mixer.SetFloat("soundVol", GetVolume(amount));

		_soundText.text = $"Sound: {ConvertDecibelToText(amount)}";
		UserSettings.SoundVolume = amount;
	}

	public void SetQualityLevel(int index)
	{
		QualitySettings.SetQualityLevel(index);
		UserSettings.QualityLevel = index;
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

	private float GetVolume(float amount) => Mathf.Log10(amount) * 20f;
	#endregion

	public void ReloadUI()
	{
		float masterVol = UserSettings.MasterVolume;
		float musicVol = UserSettings.MusicVolume;
		float soundVol = UserSettings.SoundVolume;

		_masterSlider.value = masterVol;
		_musicSlider.value = musicVol;
		_soundSlider.value = soundVol;

		_masterText.text = $"Master: {ConvertDecibelToText(masterVol)}";
		_musicText.text = $"Music: {ConvertDecibelToText(musicVol)}";
		_soundText.text = $"Sound: {ConvertDecibelToText(soundVol)}";
	}
}
