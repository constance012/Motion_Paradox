using System;
using UnityEngine;
using DG.Tweening;

public static class CanvasGroupExtensions
{
	public static void Toggle(this CanvasGroup canvasGroup, bool state)
	{
		canvasGroup.alpha = state ? 1f : 0f;
		canvasGroup.interactable = state;
		canvasGroup.blocksRaycasts = state;
	}

	public static void ToggleAnimated(this CanvasGroup canvasGroup, bool state, float duration)
	{
		canvasGroup.DOFade(Convert.ToInt32(state), duration)
				   .SetUpdate(true)
				   .OnComplete(() => canvasGroup.Toggle(state));
	}
}