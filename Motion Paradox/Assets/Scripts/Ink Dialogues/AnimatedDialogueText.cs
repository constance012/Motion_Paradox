using System.Collections;
using UnityEngine;
using TMPro;

public sealed class AnimatedDialogueText : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private TextMeshProUGUI dialogueText;
	[SerializeField] private CanvasGroup continueCue;

	[Header("Animation"), Space]
	[SerializeField] private float dialogueSpeed;
	[SerializeField] private bool overrideUserSettings;

	[Header("Typing Sound"), Space]
	[SerializeField] private bool playTypingSound;
	[SerializeField] private Vector2 pitchRange;
	[SerializeField] private int typingSoundFrequency;

	public bool IsAnimating { get; private set; }

	// Private fields.
	private BetterCoroutine _animateCoroutine = new();
	private string _currentSentence;
	private float _delayTime;

	private void Start()
	{
		_delayTime = 1f / (overrideUserSettings ? dialogueSpeed : UserSettings.DialogueSpeed);
	}

	private void OnDisable()
	{
		_currentSentence = "";
		dialogueText.text = "";
	}

	public void StartAnimating(string sentence)
	{
		_currentSentence = sentence;
		_animateCoroutine.StartNew(this, AnimateText());
	}

	public void ForceComplete()
	{
		_animateCoroutine.StopCurrent(this);
		IsAnimating = false;
		dialogueText.maxVisibleCharacters = _currentSentence.Length;
		continueCue.alpha = 1f;
	}

	private IEnumerator AnimateText()
	{
		IsAnimating = true;

		dialogueText.text = _currentSentence;
		dialogueText.maxVisibleCharacters = 0;
		continueCue.alpha = 0f;

		bool withinRichTextTags = false;

		foreach (char c in _currentSentence)
		{
			if (c == '<' || withinRichTextTags)
			{
				withinRichTextTags = true;	
				if (c == '>')
					withinRichTextTags = false;
				
				continue;
			}

			PlayTypingSound(dialogueText.maxVisibleCharacters++);
			yield return new WaitForSecondsRealtime(_delayTime);
		}

		yield return new WaitForEndOfFrame();

		continueCue.alpha = 1f;
		IsAnimating = false;
	}

	private void PlayTypingSound(int charIndex)
	{
		if (!playTypingSound)
			return;

		if (charIndex % typingSoundFrequency == 0)
		{
			AudioManager.Instance.PlayWithRandomPitch("Dialogue Typing", pitchRange.x, pitchRange.y);
		}
	}
}