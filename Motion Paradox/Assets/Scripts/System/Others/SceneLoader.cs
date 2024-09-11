using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneLoader : PersistentSingleton<SceneLoader>
{
	[Header("Transition Effect"), Space]
	[SerializeField] private SceneTransitionEffect transition;

	public EventHandler<SceneLoadEventArgs> onSceneLoaded;

	// Private fields.
	private AsyncOperation _preloadOperation;

	private void Start()
	{
		SceneManager.sceneLoaded += Scene_Loaded;
		transition.IntroFade(0f, .5f, null);
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= Scene_Loaded;
	}

	private void Scene_Loaded(Scene scene, LoadSceneMode mode)
	{
		Debug.Log($"Scene {scene.name} loaded with {mode} mode.");
		onSceneLoaded?.Invoke(this, new SceneLoadEventArgs(scene, mode));
		transition.NormalFade(0f, 1f, null);
	}

	#region Load Scene Overloads.
	public void LoadSceneAsync(string sceneName, float activateDelay = 0f)
	{
		if (IsPreviousOperationDone())
		{
			_preloadOperation = SceneManager.LoadSceneAsync(sceneName);
			ActivatePreloadedScene(activateDelay);
		}
	}

	public void LoadSceneAsync(string sceneName, LoadSceneMode mode, float activateDelay = 0f)
	{
		if (IsPreviousOperationDone())
		{
			_preloadOperation = SceneManager.LoadSceneAsync(sceneName, mode);

			if (mode == LoadSceneMode.Single)
				ActivatePreloadedScene(activateDelay);
		}
	}
	
	public void LoadSceneAsync(int buildIndex, float activateDelay = 0f)
	{
		if (IsPreviousOperationDone())
		{
			_preloadOperation = SceneManager.LoadSceneAsync(buildIndex);
			ActivatePreloadedScene(activateDelay);
		}
	}

	public void LoadSceneAsync(int buildIndex, LoadSceneMode mode, float activateDelay = 0f)
	{
		if (IsPreviousOperationDone())
		{
			_preloadOperation = SceneManager.LoadSceneAsync(buildIndex, mode);

			if (mode == LoadSceneMode.Single)
				ActivatePreloadedScene(activateDelay);
		}
	}
	#endregion

	public void ActivatePreloadedScene(float delay = 0f)
	{
		if (_preloadOperation != null)
		{
			Debug.Log("Prepare for activating scene...");
			_preloadOperation.allowSceneActivation = false;
			transition.NormalFade(1f, delay, () => _preloadOperation.allowSceneActivation = true);
		}
	}

	private bool IsPreviousOperationDone()
	{
		if (_preloadOperation != null && !_preloadOperation.isDone)
		{
			Debug.LogWarning("FAILED to load a new scene. There's a preloaded scene that hasn't been activated yet.");
			return false;
		}

		return true;
	}
}

public sealed class SceneLoadEventArgs : EventArgs
{
	public Scene scene;
	public LoadSceneMode mode;

	public SceneLoadEventArgs(Scene scene, LoadSceneMode mode)
	{
		this.scene = scene;
		this.mode = mode;
	}
}