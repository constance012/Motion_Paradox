using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public sealed class MainMenu : MonoBehaviour
{
	[Header("Menu References"), Space]
	[SerializeField] private TweenableUIElement mainMenu;
	[SerializeField] private TweenableUIElement settingsMenu;

	[Header("Audio Mixer"), Space]
	[SerializeField] private AudioMixer mixer;

	// Private fields.
	private static bool _userSettingsLoaded;

	private void Start()
	{
		#if UNITY_EDITOR
			_userSettingsLoaded = false;
		#endif

		LoadUserSettings();
	}

	#region Callback Methods for UI.
	public async void OpenSettingsMenu()
	{
		await mainMenu.SetActive(false);
		await settingsMenu.SetActive(true);
	}

	public void StartGame()
	{
		DOTween.Clear();
		SceneManager.LoadSceneAsync("Scenes/Game");
	}
	#endregion

	private void LoadUserSettings()
	{
		if (!_userSettingsLoaded)
		{
			Debug.Log("Loading user settings...");

			CursorManager.Instance.SwitchCursorTexture(CursorTextureType.Default);

			mixer.SetFloat("musicVol", UserSettings.ToMixerDecibel(UserSettings.MusicVolume));
			mixer.SetFloat("soundVol", UserSettings.ToMixerDecibel(UserSettings.SoundVolume));
			mixer.SetFloat("ambienceVol", UserSettings.ToMixerDecibel(UserSettings.AmbienceVolume));

			QualitySettings.SetQualityLevel(UserSettings.QualityLevel);
			Application.targetFrameRate = UserSettings.TargetFramerate;
			QualitySettings.vSyncCount = Convert.ToInt32(UserSettings.UseVsync);

			_userSettingsLoaded = true;
		}
	}
}