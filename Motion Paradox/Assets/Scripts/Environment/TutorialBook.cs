using UnityEngine;

public sealed class TutorialBook : Interactable, IHasDialogue
{
	[Header("Dialogue Trigger"), Space]
	[SerializeField] private DialogueTrigger dialogueTrigger;

	private void Start()
	{
		dialogueTrigger.BindExternalFunction("GetFirstControlSection", GetFirstControlSection);
		dialogueTrigger.BindExternalFunction("GetSecondControlSection", GetSecondControlSection);
	}

	protected override void CreatePopupLabel()
	{
		base.CreatePopupLabel();
		_popupLabel.SetLabelName("How to play");
	}

	public override void Interact()
	{
		TriggerDialogue();
	}

	public void TriggerDialogue()
	{
		dialogueTrigger.Trigger();
	}
	
	private string GetFirstControlSection()
	{
		return $"- '{InputManager.Instance.GetDisplayString(KeybindingActions.Movement, 1)}', " +
			   $"'{InputManager.Instance.GetDisplayString(KeybindingActions.Movement, 2)}', " +
			   $"'{InputManager.Instance.GetDisplayString(KeybindingActions.Movement, 3)}', " +
			   $"'{InputManager.Instance.GetDisplayString(KeybindingActions.Movement, 4)}': Move around<br>" +
			   $"- '{InputManager.Instance.GetDisplayString(KeybindingActions.Attack)}': Shoot<br>" +
			   $"- '{InputManager.Instance.GetDisplayString(KeybindingActions.ToggleAimMode)}': Aim<br>" +
			   $"- '{InputManager.Instance.GetDisplayString(KeybindingActions.Reload)}': Reload";
	}
	
	private string GetSecondControlSection()
	{
		return $"- '{InputManager.Instance.GetDisplayString(KeybindingActions.Interact)}': Interact<br>" +
			   $"- '{InputManager.Instance.GetDisplayString(KeybindingActions.ContinueDialogue, 1)}', " +
			   $"'{InputManager.Instance.GetDisplayString(KeybindingActions.ContinueDialogue, 2)}', " +
			   $"'{InputManager.Instance.GetDisplayString(KeybindingActions.ContinueDialogue, 3)}': Proceed dialogue<br>" +
			   $"- '{InputManager.Instance.GetDisplayString(KeybindingActions.BackToMenu)}': Back to menu";
	}
}