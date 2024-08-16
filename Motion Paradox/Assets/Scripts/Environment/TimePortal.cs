using System;
using TMPro;
using UnityEngine;

public class TimePortal : Interactable
{
	[Header("References"), Space]
	[SerializeField] private Animator animator;
	[SerializeField] private TextMeshProUGUI countdownText;

	[Header("Activation Timer (Seconds)"), Space]
	[SerializeField] private float activationTimer;

	// Properties.
	public bool Activated { get; private set; }

	private void Start()
	{
		TimeManager.InitializePortalTimer(activationTimer);
	}

	protected override void CheckForInteraction(float mouseDistance, float playerDistance)
	{
		base.CheckForInteraction(mouseDistance, playerDistance);

		Countdown();
	}

	protected override void TriggerInteraction(float playerDistance)
	{
		base.TriggerInteraction(playerDistance);

		if (InputManager.Instance.WasPressedThisFrame(KeybindingActions.Interact))
			TryEnterPortal();
	}

	protected override void CreatePopupLabel()
	{
		base.CreatePopupLabel();
		
		_popupLabel.SetLabelName(Activated ? "Enter portal?" : "Not ready yet");
	}

	private void TryEnterPortal()
	{
		if (Activated)
		{
			Debug.Log("Entering Portal...");
			GameManager.Instance.ShowVictoryScreen();
		}
	}

	private void Countdown()
	{
		if (!Activated)
		{
			TimeManager.CountdownPortalTimer(Time.deltaTime);

			countdownText.text = $"{TimeManager.PortalTimerLeft:mm\\:ss}";

			if (TimeManager.IsPortalTimerUp)
			{
				Activated = true;
				animator.Play("Activate");
				countdownText.text = "00:00";
			}
		}
	}
}