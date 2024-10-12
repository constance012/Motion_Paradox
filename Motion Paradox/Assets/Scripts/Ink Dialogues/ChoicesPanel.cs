using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;
using TMPro;

public sealed class ChoicesPanel : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private CanvasGroup canvasGroup;

	[Header("Choice Objects"), Space]
	[SerializeField] private GameObject[] choiceObjects;

	// Private fields.
	private TextMeshProUGUI[] _choiceTexts;
	private int _choiceCount;

	private void Awake()
	{
		_choiceCount = choiceObjects.Length;
		_choiceTexts = new TextMeshProUGUI[_choiceCount];

		for (int i = 0; i < _choiceCount; i++)
		{
			_choiceTexts[i] = choiceObjects[i].GetComponentInChildren<TextMeshProUGUI>();
		}
	}

	public bool TryDisplayChoices(IList<Choice> choiceList)
	{
		int choiceGiven = choiceList.Count;
		if (choiceGiven == 0)
		{
			Debug.LogWarning("There are no more choices left to display, the story might have reached the end.");
			return false;
		}

		if (choiceGiven > _choiceCount)
		{
			Debug.LogWarning($"More choices were given than the amount the UI can support. Given: {choiceGiven}, Support: {_choiceCount}");
			return false;
		}

		for (int i = 0; i < _choiceCount; i++)
		{
			if (i >= choiceGiven)
			{
				choiceObjects[i].SetActive(false);
				continue;
			}

			choiceObjects[i].SetActive(true);
			_choiceTexts[i].text = choiceList[i].text;			
		}

		canvasGroup.ToggleAnimated(true, .3f);
		return true;
	}

	public void Hide()
	{
		canvasGroup.ToggleAnimated(false, .3f);
	}
}