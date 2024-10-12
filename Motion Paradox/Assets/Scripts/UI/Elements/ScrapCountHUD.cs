using UnityEngine;
using TMPro;

public sealed class ScrapCountHUD : MonoBehaviour
{
	[Header("UI References"), Space]
	[SerializeField] private TextMeshProUGUI countText;

	[Header("Tweenable"), Space]
	[SerializeField] private TweenableUIMaster showTweenable;
	[SerializeField] private float onScreenTime;

	// Private fields.
	private float _onScreenTime;

	private void Start()
	{
		countText.text = "0";
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

	public void UpdateAmount(int amount)
	{
		if (_onScreenTime <= 0f)
			showTweenable.StartTweening(true);

		_onScreenTime = onScreenTime;
		countText.text = amount.ToString();
	}
}