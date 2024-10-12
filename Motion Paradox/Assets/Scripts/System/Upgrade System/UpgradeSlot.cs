using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public sealed class UpgradeSlot : MonoBehaviour
{
	[Header("Upgrade in Stock"), Space]
	[SerializeField, ReadOnly] private UpgradeBase currentUpgrade;

	[Header("Tweening Effects"), Space]
	[SerializeField] private TweenableUIMaster rerollEffect;

	[Header("UI References"), Space]
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private Image icon;
	[SerializeField] private TextMeshProUGUI descriptionText;
	[SerializeField] private ScrapCostText costText;

	public UpgradeBase CurrentUpgrade => currentUpgrade;

	// Private fields.
	private int _currentCost;

	public void PrepareForReroll()
	{
		rerollEffect.SetStartValues();
		canvasGroup.Toggle(false);
	}

	public async Task AddStock(UpgradeBase upgrade, int cost)
	{
		if (currentUpgrade != upgrade || currentUpgrade == null)
		{
			currentUpgrade = upgrade;
			_currentCost = cost;
			
			icon.sprite = currentUpgrade.icon;
			descriptionText.text = currentUpgrade.description;
			costText.SetPriceTag(cost);
		}
		
		await rerollEffect.AsyncStartTweening(true);

		SetInteractable();
	}

	// Callback method for the button.
	public void ChooseUpgrade()
	{
		if (!currentUpgrade.IsApplied && costText.IsSufficient)
		{
			Debug.Log($"You've chosen {currentUpgrade.displayName}.");
			UpgradeSystem.Instance.ApplyChosenUpgrade(currentUpgrade);
			ScrapCollector.Instance.UpdateAmount(-_currentCost);
		}
	}

	public void SetInteractable()
	{
		canvasGroup.Toggle(true);
		
		if (currentUpgrade.IsApplied)
		{
			costText.MarkAsOwned();
			canvasGroup.DOFade(.7f, .2f).SetUpdate(true);
		}
	}
}