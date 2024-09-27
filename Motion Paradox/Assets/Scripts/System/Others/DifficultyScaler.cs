using System;
using UnityEngine;

public sealed class DifficultyScaler : Singleton<DifficultyScaler>
{
	[Header("Settings"), Space]
	[SerializeField, Tooltip("A length in seconds for each difficulty level.")]
	private float difficultyLength;
	[SerializeField] private AnimationCurve difficultyCurve;

	public int DifficultyLevel => _currentDifficultyLevel;
	
	public event EventHandler<DifficultyReachedEventArgs> OnNextDifficultyReached;

	// Private fields.
	private float _elapsedTime;
	private float _totalTime;
	private int _currentDifficultyLevel = 1;
	private float _nextDifficultyMark;

	private void Start()
	{
		_totalTime = TimeManager.PortalTimerDuration - difficultyLength;
		_nextDifficultyMark = difficultyLength;
	}

	private void Update()
	{
		if (_elapsedTime >= _totalTime)
			return;

		_elapsedTime += Time.deltaTime * TimeManager.LocalTimeScale;

		if (_elapsedTime >= _nextDifficultyMark)
		{
			_currentDifficultyLevel++;
			_nextDifficultyMark += difficultyLength;

			Debug.Log($"Next difficulty mark: {_nextDifficultyMark}");
			OnNextDifficultyReached?.Invoke(this, new DifficultyReachedEventArgs(_elapsedTime, _totalTime, difficultyCurve));
		}
	}
}

public sealed class DifficultyReachedEventArgs : EventArgs
{
	public float elapsedTime;
	public float elapsedTimeNormalized;
	public float curveValue;

	public DifficultyReachedEventArgs(float elapsedTime, float totalTime, AnimationCurve curve)
	{
		this.elapsedTime = elapsedTime;
		elapsedTimeNormalized = Mathf.Clamp01(this.elapsedTime / totalTime);
		curveValue = curve.Evaluate(elapsedTimeNormalized);
	}
}