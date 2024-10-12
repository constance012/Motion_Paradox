using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityRandom = UnityEngine.Random;
using TMPro;

public sealed class UpgradeSystem : Singleton<UpgradeSystem>
{
	[Header("Upgrade Slots"), Space]
	[SerializeField] private List<UpgradeSlot> upgradeSlots = new List<UpgradeSlot>();

	[Header("Upgrades in Stock"), Space]
	[SerializeField] private List<UpgradeBase> stocks = new List<UpgradeBase>();
	[SerializeField, Min(1)] private int rerollLimit;

	[Header("UI References"), Space]
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private TextMeshProUGUI storedScrapText;
	[SerializeField] private Button rerollButton;
	[SerializeField] private Button skipButton;

	// Private fields.
	private HashSet<int> _rerollIndices = new HashSet<int>();
	private HashSet<IUpgradeApplicationReceiver> _receivers;
	private int _currentLevel;

	protected override void Awake()
	{
		base.Awake();
		stocks.ForEach(upgrade => upgrade.RemoveUpgrade());
		SceneLoader.Instance.OnSceneLoaded += SceneLoader_Loaded;
	}

	public void PlayerLeveling_LeveledUp(int currentLevel)
	{
		_currentLevel = currentLevel;
		titleText.text = $"<color=#BC712E>Level {_currentLevel} reached!</color>\nChoose an overdrive";
		storedScrapText.text = ScrapCollector.Instance.Amount.ToString();
		
		ToggleState(true);
	}

	public void ToggleState(bool state)
	{
		canvasGroup.ToggleAnimated(state, .3f);
		TimeManager.GlobalTimeScale = 1 - Convert.ToInt32(state);
		CursorManager.Instance.SwitchCursorTexture(state ? CursorTextureType.Default : CursorTextureType.Crosshair);

		if (state)
			Reroll(false);
	}

	// Callback method for the reroll button.
	public async void Reroll(bool consumeAttempt)
	{
		if (rerollLimit > 0 || !consumeAttempt)
		{
			TextMeshProUGUI rerollText = rerollButton.GetComponentInChildren<TextMeshProUGUI>();
			_rerollIndices.Clear();

			upgradeSlots.ForEach(slot => slot.PrepareForReroll());
			
			rerollText.text = "Rerolling...";
			rerollButton.interactable = false;
			skipButton.interactable = false;

			for (int i = 0; i < upgradeSlots.Count; i++)
			{
				UpgradeSlot slot = upgradeSlots[i];

				// Lock the slot if the remaining stocks is not enough to be added in.
				if (i >= stocks.Count)
				{
					slot.gameObject.SetActive(false);
					continue;
				}

				// Make sure not to reroll to the same upgrade twice.
				int index = UnityRandom.Range(0, stocks.Count);
				while (_rerollIndices.Contains(index))
				{
					index = UnityRandom.Range(0, stocks.Count);
				}

				_rerollIndices.Add(index);
				UpgradeBase upgrade = stocks[index];

				await slot.AddStock(upgrade, upgrade.GetCostAtLevel(_currentLevel));
			}

			skipButton.interactable = true;
			rerollLimit -= Convert.ToInt32(consumeAttempt);
			
			if (rerollLimit > 0)
			{
				rerollText.text = "Reroll";
				rerollButton.interactable = true;
			}
			else
			{
				rerollText.text = "Limit reached";
				rerollButton.interactable = false;
			}
		}
	}

	public void ApplyChosenUpgrade(UpgradeBase upgrade)
	{
		if (_receivers == null)
			SceneLoader_Loaded(this, null);

		upgrade.DoUpgrade();

		foreach (var receiver in _receivers)
		{
			receiver.OnUpgradeApplied(upgrade.GetType(), upgrade);
		}

		ToggleState(false);
	}

	private void SceneLoader_Loaded(object sender, SceneLoadEventArgs e)
	{
		Debug.Log("Fetching receivers...");
		IEnumerable<IUpgradeApplicationReceiver> receivers = FindObjectsOfType<MonoBehaviour>(true).OfType<IUpgradeApplicationReceiver>();
		_receivers = new HashSet<IUpgradeApplicationReceiver>(receivers);
	}
}