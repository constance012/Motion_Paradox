using UnityEngine;
using TMPro;
using Ink.Runtime;

public sealed class TimePortal : Interactable, IHasDialogue
{
	[Header("References"), Space]
	[SerializeField] private Animator animator;
	[SerializeField] private TextMeshProUGUI countdownText;
	[SerializeField] private DialogueTrigger dialogueTrigger;

	[Header("Activation Timer (Seconds)"), Space]
	[SerializeField] private float activationTimer;

	// Properties.
	public bool Activated { get; private set; }

	private void Start()
	{
		TimeManager.InitializePortalTimer(activationTimer);
		dialogueTrigger.BindExternalFunction("EnterPortal", EnterPortal);
	}

	protected override void CheckForInteraction(float mouseDistance, float playerDistance)
	{
		base.CheckForInteraction(mouseDistance, playerDistance);

		Countdown();
	}

	protected override void CreatePopupLabel()
	{
		base.CreatePopupLabel();
		_popupLabel.SetLabelName("Time portal");
	}

	public override void Interact()
	{
		TriggerDialogue();
	}

	public void TriggerDialogue()
	{
		dialogueTrigger.Trigger();
	}

	private void EnterPortal()
	{
		if (Activated)
		{
			GameManager.Instance.ShowVictoryScreen();
		}
	}

	private void Countdown()
	{
		if (!Activated)
		{
			TimeManager.CountdownPortalTimer(Time.deltaTime);

			string timeLeft = $"{TimeManager.PortalTimerLeft:mm\\:ss}";
			countdownText.text = timeLeft;
			InkVariableHandler.Instance.SetVariable("portal_timer", new StringValue(timeLeft));

			if (TimeManager.IsPortalTimerUp)
			{
				Activated = true;
				animator.Play("Activate");
				countdownText.text = "00:00";

				AudioManager.Instance.Play("Portal Notify");
				InkVariableHandler.Instance.SetVariable("portal_activated", new BoolValue(true));
				TriggerDialogue();
			}
		}
	}
}