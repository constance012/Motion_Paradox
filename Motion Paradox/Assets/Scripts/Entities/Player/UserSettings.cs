using System;
using UnityEngine;

/// <summary>
/// A static wrapper class for easily manipulating PlayerPref keys.
/// </summary>
public static class UserSettings
{
	#region Audio Settings
	public static float MasterVolume
	{
		get { return PlayerPrefs.GetFloat("MasterVolume", 1f); }
		set { PlayerPrefs.SetFloat("MasterVolume", value); }
	}

	public static float MusicVolume
	{
		get { return PlayerPrefs.GetFloat("MusicVolume", 1f); }
		set { PlayerPrefs.SetFloat("MusicVolume", value); }
	}

	public static float SoundVolume
	{
		get { return PlayerPrefs.GetFloat("SoundVolume", 1f); }
		set { PlayerPrefs.SetFloat("SoundVolume", value); }
	}

	public static float AmbienceVolume
	{
		get { return PlayerPrefs.GetFloat("AmbienceVolume", 1f); }
		set { PlayerPrefs.SetFloat("AmbienceVolume", value); }
	}

	public static float ToMixerDecibel(float amount) => Mathf.Log10(amount) * 20f;
	#endregion


	#region Graphics Settings
	public static int QualityLevel
	{
		get { return PlayerPrefs.GetInt("QualityLevel", 1); }
		set { PlayerPrefs.SetInt("QualityLevel", value); }
	}

	public static int TargetFramerate
	{
		get { return PlayerPrefs.GetInt("TargetFramerate", 60); }
		set { PlayerPrefs.SetInt("TargetFramerate", value); }
	}

	public static int UseVsync
	{
		get { return PlayerPrefs.GetInt("UseVsync", 0); }
		set { PlayerPrefs.SetInt("UseVsync", value); }
	}
	#endregion

	#region Gameplay Settings
	public static float AimSpeed
	{
		get { return PlayerPrefs.GetFloat("AimSpeed", 3f); }
		set { PlayerPrefs.SetFloat("AimSpeed", value); }
	}

	public static int DialogueSpeed
	{
		get { return PlayerPrefs.GetInt("DialogueSpeed", 50); }
		set { PlayerPrefs.SetInt("DialogueSpeed", value); }
	}
	#endregion

	/// <summary>
	/// Resets all the settings in the specified section to their default value.
	/// </summary>
	/// <param name="section"></param>
	public static void ResetToDefault(SettingSection section)
	{
		switch (section)
		{
			case SettingSection.Audio:
				MasterVolume = 1f;
				MusicVolume = 1f;
				SoundVolume = 1f;
				AmbienceVolume = 1f;
				break;

			case SettingSection.Graphics:
				QualityLevel = 1;
				TargetFramerate = 60;
				UseVsync = 0;
				break;

			case SettingSection.Gameplay:
				AimSpeed = 3f;
				DialogueSpeed = 50;
				break;

			case SettingSection.All:
				MasterVolume = 1f;
				MusicVolume = 1f;
				SoundVolume = 1f;
				AmbienceVolume = 1f;

				QualityLevel = 3;
				TargetFramerate = 60;
				UseVsync = 0;

				AimSpeed = 3f;
				DialogueSpeed = 50;
				break;
		}
	}

	/** <summary>
		Deletes the specified key by name, or you can optionally specified whether to delete all keys at once.
		<para />
		<c>WARNING: The method causes irreversible changes, use with your own risk.</c>
		</summary>
		<param name="keyName">The name of the key to be deleted.</param>
		<param name="deleteAll">Optional, set this to true in order to delete all keys at once.</param>
	**/
	public static void DeleteKey(string keyName, bool deleteAll = false)
	{
		if (deleteAll)
			PlayerPrefs.DeleteAll();
		else
			PlayerPrefs.DeleteKey(keyName);
	}

	public enum SettingSection { Audio, Graphics, Gameplay, All }
}
