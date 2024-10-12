using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public sealed class TweenableUIMaster : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[Header("GameObject to Tween (Override Self)"), Space]
	[SerializeField] private GameObject gameObjectToTween;

	[Header("Sequence of Tweeners"), Space]
	[SerializeField] private List<UITweener> tweeners = new List<UITweener>();

	[Header("Sequence Settings (Apply to the entire Sequence)"), Space]
	[Header("On Game Starts"), Space]
	[SerializeField] private bool disableOnStart;
	[SerializeField] private bool dontSetStartValuesOnAwake;

	[Header("Looping"), Space]
	[SerializeField] private LoopType loopType;
	[SerializeField] private int loopCount;

	[Header("Easing and Delay"), Space]
	[SerializeField] private Ease easeType = Ease.Linear;
	[SerializeField] private float delay;

	[Header("Back, Elastic and Flash Eases Only"), Space]
	[SerializeField] private float overshoot = 1.70158f;
	[SerializeField, Range(-1f, 1f)] private float period = 0f;

	[Header("Custom Update Method"), Space]
	[SerializeField] private UpdateType updateType;
	[SerializeField] private bool ignoreTimeScale;

	[Header("Others"), Space]
	[SerializeField] private bool tweenInRelativeSpace;
	[SerializeField] private bool tweenOnEnable;
	[SerializeField] private bool tweenOnMouseHover;
	[SerializeField] private bool resetValueOnDestroyOrDisable;
	
	[Header("On Complete Callback (Tweeners and Sequences)"), Space]
	[SerializeField] private UnityEvent onCompleteCallback;
	[SerializeField] private TweenCallbackPeriod callbackPeriod;

	// Hidden public fields.
	[HideInInspector] public RectTransform _rectTransform;
	[HideInInspector] public Graphic _graphic;
	[HideInInspector] public CanvasGroup _canvasGroup;

	// Indexer.
	public UITweener this[int index] => tweeners[index];

	// Private fields.
	private TweenPool _tweenPool;
	private static bool _dotweenInitialized = false;
	private bool _setActiveInProgress;

	private void OnValidate()
	{
		for (int i = 0; i < tweeners.Count; i++)
		{
			if (string.IsNullOrEmpty(tweeners[i].name))
				tweeners[i].ValidateDefaultValues(i);
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void ResetStatic()
	{
		_dotweenInitialized = false;
	}
	
	private void Awake()
	{
		if (!_dotweenInitialized)
		{
			DOTween.Init(recycleAllByDefault: false, useSafeMode: true, logBehaviour: LogBehaviour.Verbose)
				   .SetCapacity(200, 50);
			_dotweenInitialized = true;
		}

		CheckForRequireComponents();

		if (!dontSetStartValuesOnAwake)
			SetStartValues();
	}

	public async void OnEnable()
	{
		if (tweenOnEnable && !_setActiveInProgress)
			await AsyncStartTweening(true);
	}

	private void OnDisable()
	{
		if (resetValueOnDestroyOrDisable)
			SetStartValues();
	}

	private void OnDestroy()
	{
		if (resetValueOnDestroyOrDisable)
			SetStartValues();
	}

	private void Start()
	{
		gameObjectToTween.SetActive(!disableOnStart);
	}

	public async void OnPointerEnter(PointerEventData e)
	{
		if (tweenOnMouseHover && !_setActiveInProgress)
			await AsyncStartTweening(true);
	}

	public async void OnPointerExit(PointerEventData e)
	{
		if (tweenOnMouseHover && !_setActiveInProgress)
			await AsyncStartTweening(false);
	}
	
	public async Task SetActive(bool active)
	{
		_setActiveInProgress = true;

		if (active)
		{
			gameObject.SetActive(true);
			await AsyncStartTweening(true);
		}
		else
			await AsyncStartTweening(false);

		_setActiveInProgress = false;
	}

	public async void StartTweening(bool forwards)
	{
		await AsyncStartTweening(forwards);
	}

	public async Task AsyncStartTweening(bool forwards)
	{
		_tweenPool.KillActiveTweens(true);

		bool applyCallback = (forwards && callbackPeriod == TweenCallbackPeriod.AfterForwardTween) ||
							 (!forwards && callbackPeriod == TweenCallbackPeriod.AfterBackwardTween);

		if (tweeners.Count == 0)
			return;
		else if (tweeners.Count == 1)
		{
			Tween tween = tweeners[0].CreateTween(forwards)
									 .OnComplete(applyCallback ? (() => onCompleteCallback?.Invoke()) : null);

			_tweenPool.Add(tween);
			await tween.AsyncWaitForCompletion();
		}
		else
		{
			Sequence sequence = DOTween.Sequence();

			foreach (UITweener tweener in tweeners)
			{
				Tween tween = tweener.CreateTween(forwards, standalone: false);
				
				if (tweener.playAlongPreviousTweener)
				{
					sequence.Join(tween);
				}
				else
				{
					sequence.AppendInterval(tweener.delay);
					sequence.Append(tween);
				}
			}

			sequence.SetRelative(tweenInRelativeSpace)
					.SetLoops(loopCount, loopType)
					.SetEase(easeType, overshoot, period)
					.PrependInterval(delay)
					.SetUpdate(updateType, ignoreTimeScale)
					.OnComplete(applyCallback ? (() => onCompleteCallback?.Invoke()) : null);
			
			_tweenPool.Add(sequence);
			await sequence.AsyncWaitForCompletion();
		}
	}

	public Color Vector3ToColor(Vector3 vector, float alpha = 1f)
	{
		Vector3 color01 = vector / 255f;
		return new Color(color01.x, color01.y, color01.z, alpha);
	}

	public Vector3 ColorToVector3(Color color)
	{
		return new Vector3(color.r, color.g, color.b) * 255f;
	}
	
	public void SetStartValues()
	{
		foreach (UITweener tweener in tweeners)
		{
			switch (tweener.tweenType)
			{
				case UITweeningType.Scale:
					if (tweener.useCurrentValueAsStart)
						tweener.startValue = _rectTransform.localScale;
					else if (tweener.overrideStartValue)
						_rectTransform.localScale = tweener.startValue;
					break;

				case UITweeningType.SizeDelta:
					if (tweener.useCurrentValueAsStart)
						tweener.startValue = _rectTransform.sizeDelta;
					else if (tweener.overrideStartValue)
						_rectTransform.sizeDelta = tweener.startValue;
					break;

				case UITweeningType.Move:
					if (tweener.useCurrentValueAsStart)
						tweener.startValue = _rectTransform.anchoredPosition;
					else if (tweener.overrideStartValue)
						_rectTransform.anchoredPosition = tweener.startValue;
					break;
				
				case UITweeningType.Rotate:
					if (tweener.useCurrentValueAsStart)
						tweener.startValue = _rectTransform.eulerAngles;
					else if (tweener.overrideStartValue)
						_rectTransform.eulerAngles = tweener.startValue;
					break;
				
				case UITweeningType.FadeCanvasGroup:
					if (tweener.useCurrentValueAsStart)
						tweener.startValue = new Vector3(_canvasGroup.alpha, 0f, 0f);
					else if (tweener.overrideStartValue)
						_canvasGroup.alpha = tweener.startValue.x;
					break;
				
				case UITweeningType.FadeGraphic:
					if (tweener.useCurrentValueAsStart)
						tweener.startValue = new Vector3(_graphic.color.a, 0f, 0f);
					else if (tweener.overrideStartValue)
						_graphic.DOFade(tweener.startValue.x, 0f);
					break;

				case UITweeningType.Color:
					if (tweener.useCurrentValueAsStart)
						tweener.startValue = ColorToVector3(_graphic.color);
					else if (tweener.overrideStartValue)
						_graphic.color = Vector3ToColor(tweener.startValue);
					break;
			}
		}
	}

	private void CheckForRequireComponents()
	{
		if (gameObjectToTween == null)
			gameObjectToTween = gameObject;

		_tweenPool = new TweenPool();
		_rectTransform = gameObjectToTween.GetComponent<RectTransform>();
		
		foreach (UITweener tweener in tweeners)
		{
			tweener.TweenableUI = this;

			switch (tweener.tweenType)
			{
				case UITweeningType.Color:
				case UITweeningType.FadeGraphic:
					if (!gameObjectToTween.TryGetComponent(out _graphic))
						_graphic = gameObjectToTween.AddComponent<Image>();
					break;

				case UITweeningType.FadeCanvasGroup:
					if (!gameObjectToTween.TryGetComponent(out _canvasGroup))
						_canvasGroup = gameObjectToTween.AddComponent<CanvasGroup>();
					break;
			}
		}
	}
}