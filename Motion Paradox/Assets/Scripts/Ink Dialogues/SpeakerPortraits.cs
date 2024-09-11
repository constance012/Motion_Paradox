using UnityEngine;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;

public sealed class SpeakerPortraits : MonoBehaviour
{
	[Header("Portrait Map"), Space]
	[SerializeField, Tooltip("A map contains portraits corresponding to their tag keys, keys must be in LOWERCASE.")]
	private SerializedDictionary<string, Sprite> portraits;
	[SerializeField] private Sprite defaultPortrait;

	[Header("References"), Space]
	[SerializeField] private Image portraitImage;

	public void SetPortrait(string key)
	{
		if (!string.IsNullOrEmpty(key))
		{
			if (portraits.TryGetValue(key.ToLower(), out Sprite portrait))
			{
				portraitImage.sprite = portrait;
			}
			else
			{
				Debug.LogWarning($"No portrait image found for {key}, using the default image.");
				portraitImage.sprite = defaultPortrait;
			}
		}
	}
}