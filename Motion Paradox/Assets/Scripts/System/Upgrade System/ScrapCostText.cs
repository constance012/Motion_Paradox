using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class ScrapCostText : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private TextMeshProUGUI costText;
	[SerializeField] private GameObject icon;
	[SerializeField] private LayoutElement layoutElement;

	[Header("Color"), Space]
	[SerializeField] private Color normalColor;
	[SerializeField] private Color insufficientColor;

	public bool IsSufficient => _isSufficient;

	// Private fields.
	private bool _isSufficient;

	public void SetPriceTag(int scrapCost)
	{
		_isSufficient = ScrapCollector.Instance.Amount >= scrapCost;

		icon.SetActive(true);
		costText.text = scrapCost.ToString();
		costText.color = _isSufficient ? normalColor : insufficientColor;

		layoutElement.enabled = costText.preferredWidth > layoutElement.preferredWidth;
	}

	public void MarkAsOwned()
	{
		icon.SetActive(false);
		layoutElement.enabled = false;

		costText.text = "owned";
		costText.color = normalColor;
	}
}