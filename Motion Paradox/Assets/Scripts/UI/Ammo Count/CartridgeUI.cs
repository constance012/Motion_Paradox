using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public sealed class CartridgeUI : MonoBehaviour, IPoolable
{
	[Header("References"), Space]
	[SerializeField] private TweenableUIElement loadTween;
	[SerializeField] private Image graphic;

	// Private fields.
	private TweenPool _tweenPool;
	private RectTransform _rectTransform;

	private void Awake()
	{
		_rectTransform = transform as RectTransform;
		_tweenPool = new TweenPool();
	}

	public void Allocate()
	{
		gameObject.SetActive(true);
	}

	public void Deallocate()
	{
		_tweenPool.KillActiveTweens(true);
		gameObject.SetActive(false);
	}

	/// <summary>
	/// Moves the transform's local position a certain amount along the Y axis, direction depends on the sign of delta.
	/// </summary>
	/// <param name="delta"> The direction and amount to move. </param>
	public void Initialize(float spacing)
	{
		Allocate();
		loadTween.SetStartValues();
		graphic.DOFade(1f, 0f);

		_rectTransform.anchoredPosition += new Vector2(0f, spacing);
	}

	public void ChamberRound()
	{
		_tweenPool.Add(_rectTransform.DOAnchorPosX(-20, .2f, true)
			 		  				 .SetRelative(true)
			 		  				 .SetEase(Ease.OutSine));
	}

	public Tween FireRound()
	{
		Sequence sequence = DOTween.Sequence();
		
		sequence.Append(_rectTransform.DOAnchorPosX(-70f, .2f, true).SetRelative(true))
				.Join(graphic.DOFade(0f, .2f))
				.SetEase(Ease.OutSine)
		   		.OnComplete(() => Deallocate());

		_tweenPool.Add(sequence);
		return sequence;
	}

	public async void LoadRound(bool chamber = false)
	{
		await loadTween.AsyncStartTweening(true);
		
		if (chamber)
			ChamberRound();
	}

	public async void PushUpRound(float spacing, bool chamber)
	{
		Tween tween = _rectTransform.DOAnchorPosY(spacing, .2f, true)
									.SetRelative(true)
									.SetEase(Ease.OutCubic);
		
		_tweenPool.Add(tween);
		await tween.AsyncWaitForCompletion();

		if (chamber)
			ChamberRound();
	}
}