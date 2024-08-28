using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public sealed class DirectionalSelector : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private Button previousButton;
	[SerializeField] private Button nextButton;
	[SerializeField] private TextMeshProUGUI selectedText;

	[Header("Options"), Space]
	[SerializeField] private string[] options;
	[SerializeField] private int defaultOptionIndex;
	
	[Header("On Value Changed Event"), Space]
	public UnityEvent<int> onIndexChanged;
	public UnityEvent<string> onValueChanged;

	public int Index
	{
		get { return _currentIndex; }
		set { SetIndex(value); }
	}

	public string Value
	{
		get { return _selected; }
		set { SetValue(value); }
	}

	// Private fields.
	private string _selected;
	private int _currentIndex;
	private TweenPool _tweenPool = new TweenPool();

	private void Start()
	{
		previousButton.onClick.AddListener(PreviousOption);
		nextButton.onClick.AddListener(NextOption);
	}

	public void PreviousOption()
	{
		int len = options.Length;
		_currentIndex = (--_currentIndex % len + len) % len;
		ReloadUI();	
	}

	public void NextOption()
	{
		_currentIndex = ++_currentIndex % options.Length;
		ReloadUI();
	}

	private void ReloadUI()
	{
		_tweenPool.KillActiveTweens(false);

		_selected = options[_currentIndex];
		selectedText.text = _selected;

		_tweenPool.Add(selectedText.transform.DOScale(1f, .2f)
							  				 .From(1.2f)
							  				 .SetEase(Ease.OutCubic));

		onIndexChanged?.Invoke(_currentIndex);
		onValueChanged?.Invoke(_selected);
	}

	private void SetIndex(int index)
	{
		try
		{
			_currentIndex = index;
			ReloadUI();
		}
		catch (IndexOutOfRangeException)
		{
			_currentIndex = defaultOptionIndex;
			ReloadUI();
		}
	}

	private void SetValue(string value)
	{
		int index = Array.IndexOf(options, value);
		
		if (index != -1)
		{
			_currentIndex = index;
			ReloadUI();
		}
		else
		{
			_currentIndex = defaultOptionIndex;
			ReloadUI();
		}
	}
}