using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public sealed class SliderGroup : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private Slider slider;
	[SerializeField] private TextMeshProUGUI displayText;

	[Header("On Value Changed Event"), Space]
	public UnityEvent<float> onValueChanged;

	public float Value
	{
		get { return slider.value; }
		set { slider.value = value; }
	}

	public string DisplayText
	{
		get { return displayText.text; }
		set { displayText.text = value; }
	}

	private void Start()
	{
		slider.onValueChanged.AddListener(Slider_OnValueChanged);
	}

	private void Slider_OnValueChanged(float value)
	{
		onValueChanged?.Invoke(value);
	}
}