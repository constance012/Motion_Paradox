using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public sealed class CartridgeUI : MonoBehaviour
{
	[Header("Tweening Settings"), Space]
	[SerializeField] private TweenableUIElement loadTween;

	// Private fields.
	private RectTransform _rectTransform;

	private void Awake()
	{
		_rectTransform = transform as RectTransform;
	}

	/// <summary>
	/// Moves the transform's local position a certain amount along the Y axis, direction depends on the sign of delta.
	/// </summary>
	/// <param name="delta"> The direction and amount to move. </param>
	public void MoveLocalY(float delta)
	{
		_rectTransform.anchoredPosition += new Vector2(0f, delta);
	}

	public void ChamberRound()
	{
		(transform as RectTransform).DOAnchorPosX(-20, .2f, true)
			 						.SetRelative(true)
			 						.SetEase(Ease.OutSine);
	}

	public Tween FireRound()
	{
		RectTransform round = transform as RectTransform;
		Sequence sequence = DOTween.Sequence();
		
		return sequence.Append(round.DOAnchorPosX(-70f, .2f, true).SetRelative(true))
					   .Join(round.GetComponent<Image>().DOFade(0f, .2f))
					   .SetEase(Ease.OutSine)
					   .OnComplete(() => Destroy(gameObject));
	}

	public async void LoadRound(bool chamber = false)
	{
		await loadTween.AsyncStartTweening(true);
		
		if (chamber)
			ChamberRound();
	}

	public async void PushUpRound(float spacing, bool chamber = false)
	{
		await _rectTransform.DOAnchorPosY(spacing, .2f, true)
							.SetRelative(true)
							.SetEase(Ease.OutCubic)
							.AsyncWaitForCompletion();

		if (chamber)
			ChamberRound();
	}
}