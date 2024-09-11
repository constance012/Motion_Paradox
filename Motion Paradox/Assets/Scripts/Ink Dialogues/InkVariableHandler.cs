using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using InkObject = Ink.Runtime.Object;
using InkValue = Ink.Runtime.Value;

public sealed class InkVariableHandler : Singleton<InkVariableHandler>
{
	[Header("Variable Loader Json"), Space]
	[SerializeField] private TextAsset globalLoaderJson;

	// Private fields.
	private Dictionary<string, InkObject> _variables;
	private Story _globals;

	protected override void Awake()
	{
		base.Awake();

		_variables ??= new Dictionary<string, InkObject>();
		_globals = new Story(globalLoaderJson.text);

		foreach (string variableName in _globals.variablesState)
		{
			InkObject value = _globals.variablesState.GetVariableWithName(variableName);
			_variables.Add(variableName, value);
			Debug.Log($"Initialized variable: {variableName} = {value}.");
		}
	}

	#region Variable Listening Methods.
	public void StartListening(Story story)
	{
		DistributeVariablesToStory(story);
		story.variablesState.variableChangedEvent += Story_OnVariableChanged;
	}
	
	public void StopListening(Story story)
	{
		story.variablesState.variableChangedEvent -= Story_OnVariableChanged;
	}

	private void Story_OnVariableChanged(string name, InkObject value)
	{
		Debug.Log($"Variable changed: {name} = {value}");

		if (_variables.ContainsKey(name))
		{
			_variables[name] = value;
		}
	}

	private void DistributeVariablesToStory(Story story)
	{
		foreach (var variable in _variables)
		{
			story.variablesState.SetGlobal(variable.Key, variable.Value);
		}
	}
	#endregion
	
	#region Variable Getter and Setter.
	public TValue GetVariable<TValue>(string name) where TValue : InkValue
	{
		if (_variables.TryGetValue(name, out InkObject value))
		{
			return value as TValue;
		}
		else
		{
			Debug.LogWarning($"Variable {name} does not exist, returning null.");
			return null;
		}
	}

	public void SetVariable(string name, InkValue value)
	{
		if (_variables.ContainsKey(name))
		{
			_variables[name] = value;
		}
	}
	#endregion
}