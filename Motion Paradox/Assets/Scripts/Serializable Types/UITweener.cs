using System;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

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
	public FromTweenType fromTweenType;
	public bool tweenInRelativeSpace;
	public bool playAlongPreviousTweener;

	public TweenableUIMaster TweenableUI { get; set; }

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
		Tweener tweener = null;

		switch (tweenType)
		{
			case UITweeningType.Scale:
				tweener = TweenableUI._rectTransform.DOScale(targetValue, duration);
				TryConvertToFromTween<Vector3, Vector3, VectorOptions>(tweener);
				break;

			case UITweeningType.SizeDelta:
				tweener = TweenableUI._rectTransform.DOSizeDelta(targetValue, duration, snapToInteger);
				TryConvertToFromTween<Vector2, Vector2, VectorOptions>(tweener);
				break;

			case UITweeningType.Move:
				tweener = TweenableUI._rectTransform.DOAnchorPos(targetValue, duration, snapToInteger);
				TryConvertToFromTween<Vector2, Vector2, VectorOptions>(tweener);
				break;

			case UITweeningType.Rotate:
				tweener = TweenableUI._rectTransform.DORotate(targetValue, duration, rotateMode);
				TryConvertToFromTween<Quaternion, Vector3, QuaternionOptions>(tweener);
				break;

			case UITweeningType.FadeCanvasGroup:
				tweener = TweenableUI._canvasGroup.DOFade(targetValue.x, duration);
				TryConvertToFromTween<float, float, FloatOptions>(tweener);
				break;
			
			case UITweeningType.FadeGraphic:
				tweener = TweenableUI._graphic.DOFade(targetValue.x, duration);
				TryConvertToFromTween<Color, Color, ColorOptions>(tweener);
				break;

			case UITweeningType.Color:
				tweener = TweenableUI._graphic.DOColor(TweenableUI.Vector3ToColor(targetValue), duration);
				TryConvertToFromTween<Color, Color, ColorOptions>(tweener);
				break;
		}

		tweener.SetRelative(tweenInRelativeSpace)
			   .SetLoops(loopCount, loopType)
			   .SetEase(easeType, overshoot, period)
			   .SetSpeedBased(isSpeedBased)
			   .SetUpdate(updateType, ignoreTimeScale);

		if (standalone)
			tweener.SetDelay(delay);

		_tweenPool.Add(tweener);

		return tweener;
	}

	private void TryConvertToFromTween<T1, T2, TPlugOptions>(Tweener tweener) where TPlugOptions : struct, IPlugOptions
	{
		switch (fromTweenType)
		{
			case FromTweenType.FromStartValue:
				if (typeof(T2) == typeof(Vector2) || typeof(T2) == typeof(Vector3))
					(tweener as TweenerCore<T1, Vector3, TPlugOptions>).From(startValue);
				
				else if (typeof(T2) == typeof(float))
					(tweener as TweenerCore<T1, float, TPlugOptions>).From(startValue.x);
					
				break;
			
			case FromTweenType.FromEndValue:
				if (typeof(T2) == typeof(Vector2) || typeof(T2) == typeof(Vector3))
					(tweener as TweenerCore<T1, Vector3, TPlugOptions>).From(endValue);
				
				else if (typeof(T2) == typeof(float))
					(tweener as TweenerCore<T1, float, TPlugOptions>).From(endValue.x);
					
				break;
		}
	}
}

public enum FromTweenType
{
	None,
	FromStartValue,
	FromEndValue
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