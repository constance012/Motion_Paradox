using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public sealed class Prologue : MonoBehaviour
{
	[Header("Director"), Space]
	[SerializeField] private PlayableDirector director;

	[Header("Skip Text"), Space]
	[SerializeField] private TextMeshProUGUI skipText;
	[SerializeField] private float textDestroyDelay;
	[SerializeField] private float sceneActivationDelay;

	// Private fields.
	private bool _donePlayback;

	private void Start()
	{
		skipText.text = $"Press '{InputManager.Instance.GetDisplayString(KeybindingActions.SkipPlayable)}' to skip";
		Destroy(skipText.transform.parent.gameObject, textDestroyDelay);

		InputManager.Instance.onSkipPlayableAction += (sender, e) => SkipPlayable();
	}

	private void Update()
	{
		if (_donePlayback)
			return;

		if (director.time == 0d && director.state == PlayState.Paused)
		{
			SkipPlayable();
		}
	}

	private void SkipPlayable()
	{
		director.Stop();
		_donePlayback = true;
		SceneLoader.Instance.LoadSceneAsync("Scenes/Game", sceneActivationDelay);
	}
}