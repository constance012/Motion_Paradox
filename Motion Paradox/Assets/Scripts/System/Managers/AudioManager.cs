using System;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

[AddComponentMenu("Singletons/Audio Manager")]
public sealed class AudioManager : PersistentSingleton<AudioManager>
{
	[Header("Audio Array"), Space]
	public Audio[] audioArray;

	protected override void Awake()
	{
		base.Awake();

		foreach (var audio in audioArray)
		{
			GameObject audioSourceHolder = new GameObject(audio.name);
			audioSourceHolder.transform.SetParent(transform.Find(audio.audioType.ToString()));

			// Add the Audio Source to the Manager's holder for each clip.
			audio.source = audioSourceHolder.AddComponent<AudioSource>();
			audio.source.outputAudioMixerGroup = audio.mixerGroup;
			audio.source.volume = audio.volume;
			audio.source.pitch = audio.pitch;
			audio.source.loop = audio.loop;
			audio.source.playOnAwake = false;
		}
	}

	private void Start()
	{
		Play("Main Theme");
	}

	/// <summary>
	/// Play the audio with a random clip and default pitch.
	/// </summary>
	/// <param name="audioName"></param>
	public void Play(string audioName)
	{
		if (!TryGetAudio(audioName, out Audio chosenAudio))
		{
			Debug.LogWarning($"Audio Clip: {audioName} could not be found!!");
			return;
		}

		chosenAudio.source.clip = GetRandomClip(chosenAudio);

		chosenAudio.source.Play();
	}

	/// <summary>
	/// Play the audio with the specified clip and pitch.
	/// </summary>
	/// <param name="audioName"></param>
	/// <param name="clipIndex"></param>
	/// <param name="pitch"></param>
	public void Play(string audioName, int clipIndex, float pitch)
	{
		if (!TryGetAudio(audioName, out Audio chosenAudio))
		{
			Debug.LogWarning($"Audio Clip: {audioName} could not be found!!");
			return;
		}

		chosenAudio.source.clip = chosenAudio[clipIndex];
		chosenAudio.source.pitch = pitch;

		chosenAudio.source.Play();
	}

	/// <summary>
	/// Play the audio with random clip and pitch;
	/// </summary>
	/// <param name="audioName"></param>
	/// <param name="min"></param>
	/// <param name="max"></param>
	public void PlayWithRandomPitch(string audioName, float min, float max)
	{
		if (!TryGetAudio(audioName, out Audio chosenAudio))
		{
			Debug.LogWarning($"Audio Clip: {audioName} could not be found!!");
			return;
		}

		chosenAudio.source.clip = GetRandomClip(chosenAudio);
		chosenAudio.source.pitch = UnityRandom.Range(min, max);

		chosenAudio.source.Play();
	}

	public void Stop(string audioName)
	{
		if (!TryGetAudio(audioName, out Audio chosenAudio))
		{
			Debug.LogWarning($"Audio Clip: {audioName} could not be found!!");
			return;
		}

		chosenAudio.source.Stop();
	}

	public void SetVolume(string audioName, float newVolume, bool resetToDefault = false)
	{
		if (TryGetAudio(audioName, out Audio chosenAudio))
		{
			chosenAudio.source.volume = resetToDefault ? chosenAudio.volume : newVolume;
		}
	}

	public bool TryGetAudio(string audioName, out Audio chosenAudio)
	{
		chosenAudio = GetAudio(audioName);
		return chosenAudio != null;
	}

	public Audio GetAudio(string audioName)
	{
		audioName = audioName.ToLower().Trim();
		return Array.Find(audioArray, audio => audio.name.ToLower().Equals(audioName));
	}

	private AudioClip GetRandomClip(Audio target)
	{
		int index = UnityRandom.Range(0, target.clips.Length);
		return target[index];
	}
}
