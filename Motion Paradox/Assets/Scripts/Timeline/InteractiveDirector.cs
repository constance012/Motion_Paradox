using UnityEngine;
using UnityEngine.Playables;

public sealed class NarrativeDirector : MonoBehaviour, IHasDialogue
{
	[Header("Playable Asset"), Space]
	[SerializeField] private PlayableDirector director;

	[Header("Dialogue Trigger"), Space]
	[SerializeField] private DialogueTrigger dialogueTrigger;

	[Header("Pause Time Marks (Seconds)"), Space]
	[SerializeField] private double[] pauseMarks;

	// Private fields.
	private double _totalDuration;
	private int _pauseMarkCount;
	private int _nextMarkIndex;

	private void Start()
	{
		_totalDuration = director.duration;
		_pauseMarkCount = pauseMarks.Length;
		_nextMarkIndex = 0;

		dialogueTrigger.BindExternalFunction("ResumePlayable", ResumePlayable);
	}

	private void Update()
	{
		if (_nextMarkIndex == _pauseMarkCount)
			return;

		if (director.time > pauseMarks[_nextMarkIndex])
		{
			PausePlayable(pauseMarks[_nextMarkIndex]);
		}
	}

	public void TriggerDialogue()
	{
		_nextMarkIndex = Mathf.Clamp(++_nextMarkIndex, 0, _pauseMarkCount);  // Integer clamping is inclusive.
		director.Pause();
		dialogueTrigger.Trigger();
	}

	private void PausePlayable(double timeMark)
	{
		if (timeMark < _totalDuration && timeMark >= 0d)
		{
			TriggerDialogue();
		}
		else
		{
			Debug.LogWarning($"Failed to pause the playable, the time mark at {timeMark:0.000}s is invalid.");
		}
	}

	private void ResumePlayable()
	{
		director.Resume();
	}
}
