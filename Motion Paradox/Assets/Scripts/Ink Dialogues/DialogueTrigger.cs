using System;
using UnityEngine;
using Ink.Runtime;
using System.Collections;

public class DialogueTrigger : MonoBehaviour
{
	[Header("Attached Ink Json"), Space]
	[SerializeField] private TextAsset inkJson;

	[Header("Options"), Space]
	[SerializeField] private bool triggerOnStartup;
	[SerializeField, Tooltip("Controls whether this story always plays from the beginning upon triggering.")]
	private bool playFromBeginning;

	[Header("Destroy After Trigger"), Space]
	[SerializeField] private bool destroyAfterTrigger;
	[SerializeField, Min(0f)] private float delay;

	// Private fields.
	private Story _story;

	private void Awake()
	{
		_story = new Story(inkJson.text);
	}

	private IEnumerator Start()
	{
		if (triggerOnStartup)
		{
			yield return new WaitForSecondsRealtime(.1f);
			Trigger();
		}
	}

	#region External Functions Binding Methods.
	public void BindExternalFunction(string name, Action action)
	{
		_story.BindExternalFunction(name, action);
	}

	public void BindExternalFunction<T1>(string name, Action<T1> action)
	{
		_story.BindExternalFunction(name, action);
	}

	public void BindExternalFunction<T1, T2>(string name, Action<T1, T2> action)
	{
		_story.BindExternalFunction(name, action);
	}

	public void BindExternalFunction<T1, T2, T3>(string name, Action<T1, T2, T3> action)
	{
		_story.BindExternalFunction(name, action);
	}

	public void BindExternalFunction<T1, T2, T3, T4>(string name, Action<T1, T2, T3, T4> action)
	{
		_story.BindExternalFunction(name, action);
	}

	public void BindExternalFunction(string name, Func<object> func)
	{
		_story.BindExternalFunction(name, func);
	}

	public void BindExternalFunction<T1>(string name, Func<T1, object> func)
	{
		_story.BindExternalFunction(name, func);
	}

	public void BindExternalFunction<T1, T2>(string name, Func<T1, T2, object> func)
	{
		_story.BindExternalFunction(name, func);
	}

	public void BindExternalFunction<T1, T2, T3>(string name, Func<T1, T2, T3, object> func)
	{
		_story.BindExternalFunction(name, func);
	}

	public void BindExternalFunction<T1, T2, T3, T4>(string name, Func<T1, T2, T3, T4, object> func)
	{
		_story.BindExternalFunction(name, func);
	}

	public void UnbindExternalFunction(string name)
	{
		_story.UnbindExternalFunction(name);
	}
	#endregion

	public void Trigger()
	{
		if (playFromBeginning)
			_story.ResetState();
		
		if (!DialogueSystem.IsPlaying)
		{
			Debug.Log($"Trigger dialogue of {gameObject.name}");
			DialogueSystem.Instance.PlayDialogue(_story);
		}

		if (destroyAfterTrigger)
			Destroy(gameObject, delay);
	}
}