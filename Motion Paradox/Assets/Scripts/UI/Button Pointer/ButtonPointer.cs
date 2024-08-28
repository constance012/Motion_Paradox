using UnityEngine;
using UnityEngine.UI;

public sealed class ButtonPointer : Singleton<ButtonPointer>
{
	[Header("References"), Space]
	[SerializeField] private LayoutElement separator;

	// Private fields.
	private RectTransform _rectTransform;

	protected override void Awake()
	{
		base.Awake();
		_rectTransform = transform as RectTransform;
	}

	private void Start()
	{
		HidePointer();
	}

	public void ShowPointer(RectTransform caller)
	{
		separator.preferredWidth = caller.sizeDelta.x;

		_rectTransform.SetParent(caller.parent);
		_rectTransform.anchorMin = caller.anchorMin;
		_rectTransform.anchorMax = caller.anchorMax;
		_rectTransform.anchoredPosition = caller.anchoredPosition;
		
		gameObject.SetActive(true);

		AudioManager.Instance.PlayWithRandomPitch("Button Selected", .7f, 1f);
	}

	public void HidePointer()
	{
		separator.preferredWidth = separator.minWidth;
		gameObject.SetActive(false);
	}
}