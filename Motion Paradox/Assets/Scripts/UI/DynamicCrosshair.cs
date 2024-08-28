using UnityEngine;
using DG.Tweening;

public sealed class DynamicCrosshair : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private RectTransform[] surroundings;

	[Header("Base Expansion Settings"), Space]
	[SerializeField] private float baseExpandMultipler;

	[Header("Dynamic Expansion Settings"), Space]
	[SerializeField, Min(0f)] private float expandAmount;
	[SerializeField, Range(0f, 1f)] private float expandDurationMultiplier;

	// Private fields.
	private readonly Vector2[] _expandDirections = new Vector2[]
	{
		new Vector2(-1f, 1f),
		new Vector2(1f, 1f),
		new Vector2(-1f, -1f),
		new Vector2(1f, -1f),
	};
	private RectTransform _rectTransform;
	private TweenPool _tweenPool;
	private float _previousExpandAmount;

	private void Awake()
	{
		_rectTransform = transform as RectTransform;
		_tweenPool = new TweenPool();
	}

	private void FixedUpdate()
	{
		_rectTransform.anchoredPosition = InputManager.Instance.MousePosition;
	}

	public async void ChangeBaseExpansion(float expandDelta)
	{
		_tweenPool.KillActiveTweens(true);

		await TweenSurroundings(-_previousExpandAmount, 0f, Ease.OutCubic, 0)
			  .AsyncWaitForCompletion();

		_previousExpandAmount = expandDelta * baseExpandMultipler;

		_tweenPool.Add(TweenSurroundings(_previousExpandAmount, .2f, Ease.OutCubic, 0));
	}

	public void Expand(float duration)
	{
		_tweenPool.KillActiveTweens(true);
		_tweenPool.Add(TweenSurroundings(expandAmount, duration * expandDurationMultiplier, Ease.OutQuad, 2));
	}

	private Tween TweenSurroundings(float amount, float duration, Ease easeType, int loopCount)
	{
		Sequence sequence = DOTween.Sequence();

		for (int i = 0; i < 4; i++)
		{
			sequence.Join(surroundings[i].DOAnchorPos(_expandDirections[i] * amount, duration, true)
										 .SetRelative(true)
										 .SetEase(easeType)
										 .SetLoops(loopCount, LoopType.Yoyo));
		}

		return sequence;
	}
}