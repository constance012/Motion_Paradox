using UnityEngine;
using TMPro;

public sealed class TweenableText : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private TextMeshProUGUI textMesh;
	[SerializeField] private TweenableUIMaster tweenable;

	public string Text
	{
		get { return textMesh.text; }
		set { SetText(value); }
	}

	public Color Color
	{
		get { return textMesh.color; }
		set { textMesh.color = value; }
	}

	public void SetTextWithoutTween(string text)
	{
		textMesh.text = text;
	}
	
	private void SetText(string text)
	{
		textMesh.text = text;
		tweenable.StartTweening(true);
	}
}