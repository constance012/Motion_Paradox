using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using WebGLScreenshot = WebGLScreenshotTool;
using System;

[AddComponentMenu("Singletons/Game Manager")]
public sealed class GameManager : Singleton<GameManager>
{
	[Header("References"), Space]
	[SerializeField] private CanvasGroup gameOverScreen;
	[SerializeField] private CanvasGroup victoryScreen;
	
	public EventHandler onGameOver;
	public EventHandler onGameVictory;

	public bool GameDone { get; private set; }

	private void Start()
	{
		InputManager.Instance.onBackToMenuAction += (sender, e) => ReturnToMenu();
		InputManager.Instance.onTakeScreenshotAction += (sender, e) => WebGLScreenshot.WebGLScreenshotTool.instance.TakeScreenshot();

		AudioManager.Instance.Play("Ambience");

		gameOverScreen.Toggle(false);
		victoryScreen.Toggle(false);
	}

	/// <summary>
	/// Callback method for the retry button.
	/// </summary>
	public void RestartGame()
	{
		GameDone = false;

		SceneManager.LoadSceneAsync("Scenes/Game");
	}

	/// <summary>
	/// Callback method for the return to menu button.
	/// </summary>
	public void ReturnToMenu()
	{
		SceneManager.LoadSceneAsync("Scenes/Menu");
	}

	public void ShowGameOverScreen()
	{
		GameDone = true;
		TimeManager.LocalTimeScale = 1f;

		CursorManager.Instance.SwitchCursorTexture(CursorTextureType.Default);

		onGameOver?.Invoke(this, EventArgs.Empty);

		gameOverScreen.DOFade(1f, .75f)
					  .OnComplete(() => gameOverScreen.Toggle(true));
	}

	public void ShowVictoryScreen()
	{
		GameDone = true;
		TimeManager.LocalTimeScale = 1f;

		CursorManager.Instance.SwitchCursorTexture(CursorTextureType.Default);

		onGameVictory?.Invoke(this, EventArgs.Empty);

		victoryScreen.DOFade(1f, .75f)
					  .OnComplete(() => victoryScreen.Toggle(true));
	}
}