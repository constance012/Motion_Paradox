using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public abstract class DirectionalSelector<TObject> : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] protected Button previousButton;
	[SerializeField] protected Button nextButton;
	[SerializeField] protected TextMeshProUGUI selectedText;

	[Header("Options"), Space]
	[SerializeField] protected TObject[] options;
	[SerializeField] protected int defaultOptionIndex;
	
	[Header("On Value Changed Event"), Space]
	public UnityEvent<int> onIndexChanged;
	public UnityEvent<TObject> onValueChanged;

	public int Index
	{
		get { return _currentIndex; }
		set { SetIndex(value); }
	}

	public TObject Value
	{
		get { return _selected; }
		set { SetValue(value); }
	}

	// Protected fields.
	protected TObject _selected;
	protected int _currentIndex;

	// Private fields.
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
		SetDisplayText();

		_tweenPool.Add(selectedText.transform.DOScale(1f, .2f)
							  				 .From(1.2f)
							  				 .SetEase(Ease.OutCubic));

		onIndexChanged?.Invoke(_currentIndex);
		onValueChanged?.Invoke(_selected);
	}

	protected abstract void SetDisplayText();

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

	private void SetValue(TObject value)
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