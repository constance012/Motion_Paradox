using System;
using System.Linq;
using System.Collections;
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
	private BetterCoroutine _toggleCoroutine = new();
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

		// Reward the player with 1 reroll chance every 2 levels.
		rerollLimit += _currentLevel % 2 != 0 ? 1 : 0;

		titleText.text = $"<color=#BC712E>Level {_currentLevel} reached!</color>\nChoose an overdrive";

		if (!GameManager.GameDone)
		{
			_toggleCoroutine.StartNew(this, ToggleStateDelayed(true, .5f), true);
		}
	}

	public void Close()
	{
		_toggleCoroutine.StartNew(this, ToggleStateDelayed(false, .2f), true);
	}

	public IEnumerator ToggleStateDelayed(bool isActive, float delay)
	{
		yield return new WaitForSecondsRealtime(delay);

		canvasGroup.ToggleAnimated(isActive, .3f);
		storedScrapText.text = ScrapCollector.Instance.Amount.ToString();

		TimeManager.GlobalTimeScale = 1 - Convert.ToInt32(isActive);
		CursorManager.Instance.SwitchCursorTexture(isActive ? CursorTextureType.Default : CursorTextureType.Crosshair);

		if (isActive)
		{
			Reroll(consumeAttempt: false);
		}
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

		Close();
	}

	private void SceneLoader_Loaded(object sender, SceneLoadEventArgs e)
	{
		IEnumerable<IUpgradeApplicationReceiver> receivers = FindObjectsOfType<MonoBehaviour>(true).OfType<IUpgradeApplicationReceiver>();
		_receivers = new HashSet<IUpgradeApplicationReceiver>(receivers);
	}
}