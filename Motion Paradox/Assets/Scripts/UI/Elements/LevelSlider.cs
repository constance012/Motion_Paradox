using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public sealed class LevelSlider : MonoBehaviour
{
	[Header("UI References"), Space]
	[SerializeField] private TweenableText levelText;
	[SerializeField] private TextMeshProUGUI xpText;
	[SerializeField] private Slider slider;
	[SerializeField] private Image fillImage;

	[Header("Tweenable"), Space]
	[SerializeField] private TweenableUIMaster showTweenable;
	[SerializeField] private float onScreenTime;

	[Header("Fill Colors"), Space]
	[SerializeField] private Color defaultFillColor;
	[SerializeField] private Color xpChangedColor;

	// Private fields.
	private TweenPool _tweenPool = new();
	private float _onScreenTime;

	private void Start()
	{
		fillImage.color = defaultFillColor;
		slider.onValueChanged.AddListener((value) => xpText.text = $"{value}/{slider.maxValue}");
	}

	private void Update()
	{
		if (_onScreenTime > 0f)
		{
			_onScreenTime -= Time.unscaledDeltaTime;

			if (_onScreenTime <= 0f)
			{
				showTweenable.StartTweening(false);
			}
		}
	}

	public void UpdateExperience(int xp)
	{
		if (_onScreenTime <= 0f)
			showTweenable.StartTweening(true);

		_onScreenTime = onScreenTime;

		TweenSliderValue(xp);
	}

	public void SetLevelRange(int level, int xp, int min, int max)
	{
		levelText.Text = $"Level <size=20>{level}</size>";

		slider.minValue = min;
		slider.maxValue = max;
		
		if (slider.minValue == slider.maxValue)
			slider.minValue--;
		
		TweenSliderValue(xp);
	}

	public void TweenSliderValue(int xp)
	{
		_tweenPool.KillActiveTweens(true);
		Sequence sequence = DOTween.Sequence();

		sequence.Append(slider.DOValue(xp, .3f).SetEase(Ease.InSine))
				.Join(fillImage.DOColor(defaultFillColor, .6f).From(xpChangedColor).SetEase(Ease.InCubic))
				.SetUpdate(true);

		_tweenPool.Add(sequence);
	}
}