using UnityEngine;
using DG.Tweening;
using System;

[AddComponentMenu("Singletons/Game Manager")]
public sealed class GameManager : Singleton<GameManager>
{
	[Header("References"), Space]
	[SerializeField] private CanvasGroup gameOverScreen;
	[SerializeField] private CanvasGroup victoryScreen;
	
	public EventHandler onGameOver;
	public EventHandler onGameVictory;

	public static bool GameDone { get; private set; }

	private void Start()
	{
		GameDone = false;
		gameOverScreen.Toggle(false);
		victoryScreen.Toggle(false);

		AudioManager.Instance.Play("Ambience");
		InputManager.Instance.onBackToMenuAction += (sender, e) => ReturnToMenu();
	}

	/// <summary>
	/// Callback method for the retry button.
	/// </summary>
	public void RestartGame()
	{
		GameDone = false;
		TimeManager.GlobalTimeScale = 1f;

		DOTween.KillAll();
		DOTween.ClearCachedTweens();
		CursorManager.Instance.SwitchCursorTexture(CursorTextureType.Crosshair);

		SceneLoader.Instance.LoadSceneAsync("Scenes/Game");
	}

	/// <summary>
	/// Callback method for the return to menu button.
	/// </summary>
	public void ReturnToMenu()
	{
		GameDone = false;
		TimeManager.GlobalTimeScale = 1f;

		DOTween.Clear();
		CursorManager.Instance.SwitchCursorTexture(CursorTextureType.Default);
		AudioManager.Instance.Stop("Ambience");
		
		SceneLoader.Instance.LoadSceneAsync("Scenes/Menu");
	}

	public void ShowGameOverScreen()
	{
		GameDone = true;
		TimeManager.GlobalTimeScale = 1f;

		CursorManager.Instance.SwitchCursorTexture(CursorTextureType.Default);

		onGameOver?.Invoke(this, EventArgs.Empty);

		gameOverScreen.DOFade(1f, .75f)
					  .OnComplete(() => gameOverScreen.Toggle(true));
	}

	public void ShowVictoryScreen()
	{
		GameDone = true;
		TimeManager.GlobalTimeScale = 1f;

		CursorManager.Instance.SwitchCursorTexture(CursorTextureType.Default);

		onGameVictory?.Invoke(this, EventArgs.Empty);

		victoryScreen.DOFade(1f, .75f)
					  .OnComplete(() => victoryScreen.Toggle(true));
	}
}