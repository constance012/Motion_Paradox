using System;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class UITweener
{
	[Header("Tweener Settings"), Space]
	[Header("General"), Space]
	public string name;
	public UITweeningType tweenType;
	
	[Tooltip("The end value of the tween, USE ONLY the X component for a single floating point.")]
	public Vector3 endValue;
	public float duration;

	[Header("On Game Starts"), Space]
	[Tooltip("The override value, USE ONLY the X component for a single floating point.")]
	public Vector3 startValue;
	public bool useCurrentValueAsStart;
	
	[Tooltip("Override the starting value of the game object after the game starts.")]
	public bool overrideStartValue;
	
	[Header("Moving and Rotating"), Space]
	public RotateMode rotateMode;
	public bool snapToInteger;
	public bool isSpeedBased;

	[Header("Looping"), Space]
	public LoopType loopType;
	public int loopCount;

	[Header("Easing and Delay"), Space]
	public Ease easeType = Ease.Linear;
	public float delay;

	[Header("Back, Elastic and Flash Eases Only"), Space]
	public float overshoot = 1.70158f;
	[Range(-1f, 1f)] public float period = 0f;

	[Header("Custom Update Method"), Space]
	public UpdateType updateType;
	public bool ignoreTimeScale;

	[Header("Others"), Space]
	public bool tweenInRelativeSpace;
	public bool playAlongPreviousTweener;

	public TweenableUIElement TweenableUI { get; set; }

	private TweenPool _tweenPool;

	public void ValidateDefaultValues(int index)
	{
		name = $"Unnamed Tweener #{index + 1}";
		easeType = Ease.Linear;
		overshoot = 1.70158f;
	}

	public Tween CreateTween(bool forwards, bool standalone = true)
	{
		_tweenPool ??= new TweenPool();
		_tweenPool.KillActiveTweens(true);

		Vector3 targetValue = forwards ? endValue : startValue;
		Tween tween = null;

		switch (tweenType)
		{
			case UITweeningType.Scale:
				tween = TweenableUI._rectTransform.DOScale(targetValue, duration);
				break;

			case UITweeningType.SizeDelta:
				tween = TweenableUI._rectTransform.DOSizeDelta(targetValue, duration, snapToInteger);
				break;

			case UITweeningType.Move:
				tween = TweenableUI._rectTransform.DOAnchorPos(targetValue, duration, snapToInteger);
				break;

			case UITweeningType.Rotate:
				tween = TweenableUI._rectTransform.DORotate(targetValue, duration, rotateMode);
				break;

			case UITweeningType.FadeCanvasGroup:
				tween = TweenableUI._canvasGroup.DOFade(targetValue.x, duration);
				break;
			
			case UITweeningType.FadeGraphic:
				tween = TweenableUI._graphic.DOFade(targetValue.x, duration);
				break;

			case UITweeningType.Color:
				tween = TweenableUI._graphic.DOColor(TweenableUI.Vector3ToColor(targetValue), duration);
				break;
		}

		tween.SetRelative(tweenInRelativeSpace)
			 .SetLoops(loopCount, loopType)
			 .SetEase(easeType, overshoot, period)
			 .SetSpeedBased(isSpeedBased)
			 .SetUpdate(updateType, ignoreTimeScale);

		if (standalone)
			tween.SetDelay(delay);

		_tweenPool.Add(tween);

		return tween;
	}
}

public enum UITweeningType
{
	Scale = 0,
	SizeDelta = 6,
	Move = 1,
	Rotate = 2,
	FadeCanvasGroup = 3,
	FadeGraphic = 4,
	Color = 5
}

public enum TweenCallbackPeriod
{
	AfterForwardTween,
	AfterBackwardTween
}