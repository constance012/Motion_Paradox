using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Ink.Runtime;

public sealed class DialogueSystem : Singleton<DialogueSystem>
{
	[Header("Canvas Group"), Space]
	[SerializeField] private CanvasGroup canvasGroup;

	[Header("UI References"), Space]
	[SerializeField] private Animator layoutAnimator;
	[SerializeField] private AnimatedDialogueText dialogueText;
	[SerializeField] private SpeakerPortraits portrait;
	[SerializeField] private TextMeshProUGUI speakerNameText;
	[SerializeField] private GameObject continueCue;

	[Header("Choices Panel"), Space]
	[SerializeField] private ChoicesPanel choicesPanel;

	[Header("Options"), Space]
	[SerializeField] private bool pauseGameWhilePlaying;

	[Header("Events"), Space]
	public UnityEvent onDialoguePlayed;
	public UnityEvent onDialogueEnded;

	// Properties.
	public static bool IsPlaying { get; private set; }
	
	// Private fields.
	private const string _BREAK_POINT_STRING = "--breakpoint--";
	private Story _currentStory;
	private InkTagParser _tagParser;
	
	private void Start()
	{
		_tagParser = new InkTagParser(':');

		IsPlaying = false;
		canvasGroup.Toggle(false);
		choicesPanel.Hide();

		InputManager.Instance.OnContinueDialogueAction += (sender, e) => TryContinueStory();
	}

	#region Dialogue States Control Methods.
	public void PlayDialogue(Story story)
	{
		_currentStory = story;
		IsPlaying = true;

		TimeManager.GlobalTimeScale = Convert.ToInt32(!pauseGameWhilePlaying);
		InkVariableHandler.Instance.StartListening(_currentStory);
		CursorManager.Instance.SwitchCursorTexture(CursorTextureType.Default);
		onDialoguePlayed?.Invoke();

		canvasGroup.ToggleAnimated(true, .3f);
		TryContinueStory();
	}

	public void ForceEndDialogue()
	{
		StartCoroutine(EndDialogue());
	}

	private IEnumerator EndDialogue()
	{
		TimeManager.GlobalTimeScale = 1f;
		
		// Prevent other inputs from being received before the dialogue ends.
		yield return new WaitForSeconds(.1f);

		if (!GameManager.GameDone)
			CursorManager.Instance.SwitchCursorTexture(CursorTextureType.Crosshair);
			
		InkVariableHandler.Instance.StopListening(_currentStory);
		onDialogueEnded?.Invoke();
		
		IsPlaying = false;
		_currentStory = null;

		canvasGroup.ToggleAnimated(false, .3f);
	}
	#endregion

	#region Story Flow Control Methods.
	private void TryContinueStory()
	{
		if (IsPlaying)
		{
			if (dialogueText.IsAnimating)
			{
				dialogueText.ForceComplete();
			}
			else if (_currentStory.canContinue)
			{
				Debug.Log("Continuing story...");

				string nextSentence = _currentStory.Continue().Trim('\n', '\r');
				if (string.IsNullOrEmpty(nextSentence))
				{
					TryContinueStory();
					return;
				}
				if (nextSentence.Equals(_BREAK_POINT_STRING))
				{
					ForceEndDialogue();
					return;
				}

				dialogueText.StartAnimating(nextSentence);
				HandleTags();
			}
			else
			{
				if (choicesPanel.TryDisplayChoices(_currentStory.currentChoices))
				{
					// Hide the continue cue if there are choices to be displayed.
					continueCue.SetActive(false);
				}
				else
				{
					// End the story as there are no choices left to be displayed.
					ForceEndDialogue();
				}
			}
		}
	}

	// Callback method for choice UI buttons.
	public void MakeChoice(int index)
	{
		_currentStory.ChooseChoiceIndex(index);
		choicesPanel.Hide();
		TryContinueStory();
	}

	private void HandleTags()
	{
		if (_tagParser.TryUpdateTags(_currentStory.currentTags))
		{
			// Only update when new tag values arrive.
			speakerNameText.text = _tagParser[InkTagKey.Speaker];
			portrait.SetPortrait(_tagParser[InkTagKey.Portrait]);
			layoutAnimator.Play(_tagParser[InkTagKey.Layout]);
		}
	}
	#endregion
}