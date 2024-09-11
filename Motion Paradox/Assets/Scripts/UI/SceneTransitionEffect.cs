using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public sealed class SceneTransitionEffect : MonoBehaviour
{
	[Header("Fade Image"), Space]
	[SerializeField] private Image fadeImage;
	[SerializeField] private float fadeDuration;

	[Header("Colors"), Space]
	[SerializeField] private Color introColor;
	[SerializeField] private Color normalColor;

	// Private fields.
	private TweenPool _tweenPool;

	private void Awake()
	{
		_tweenPool = new TweenPool();
	}

	public void IntroFade(float alpha, float delay, TweenCallback completeCallback)
	{
		fadeImage.color = introColor.ExtractRGB(1f - alpha);
		BeginFading(alpha, delay, completeCallback);
	}

	public void NormalFade(float alpha, float delay, TweenCallback completeCallback)
	{
		fadeImage.color = normalColor.ExtractRGB(1 - alpha);
		BeginFading(alpha, delay, completeCallback);
	}

	private void BeginFading(float alpha, float delay, TweenCallback completeCallback)
	{
		_tweenPool.KillActiveTweens(false);
		_tweenPool.Add(fadeImage.DOFade(alpha, fadeDuration)
								.SetDelay(delay)
				 				.SetEase(Ease.OutSine)
								.SetUpdate(true)
								.OnComplete(completeCallback));
	}
}