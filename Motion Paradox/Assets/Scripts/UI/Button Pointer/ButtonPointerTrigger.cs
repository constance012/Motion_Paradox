using UnityEngine;
using UnityEngine.EventSystems;

public sealed class ButtonPointerTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private RectTransform _rectTransform;

	private void Awake()
	{
		_rectTransform = transform as RectTransform;
	}

	public void OnPointerEnter(PointerEventData e)
	{
		ButtonPointer.Instance.ShowPointer(_rectTransform);
	}

	public void OnPointerExit(PointerEventData e)
	{
		ButtonPointer.Instance.HidePointer();
	}
}