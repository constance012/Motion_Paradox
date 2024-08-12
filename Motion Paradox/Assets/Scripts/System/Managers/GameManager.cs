using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
	public bool GameDone { get; private set; }

	private void Start()
	{
		InputManager.Instance.onBackToMenuAction += (sender, e) => ReturnToMenu();
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

	public void GameOver()
	{
		GameDone = true;
	}
}