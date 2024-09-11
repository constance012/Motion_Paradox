using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using UnityDebug = UnityEngine.Debug;

/// <summary>
/// Manages inputs from various devices and sources, using the NEW input system.
/// </summary>
[AddComponentMenu("Singletons/Input Manager")]
public sealed class InputManager : Singleton<InputManager>
{
	public EventHandler onAttackAction;
	public EventHandler<InputActionPhase> onAimModeToggleAction;
	public EventHandler onContinueDialogueAction;
	public EventHandler onReloadAction;
	public EventHandler onBackToMenuAction;
	public EventHandler onSkipPlayableAction;

	// Private fields.
	private PlayerInputActions _playerInputActions;
	private Dictionary<KeybindingActions, InputAction> _inputActions;

	protected override void Awake()
	{
		base.Awake();
		Initialize();
	}

	private void OnDestroy()
	{
		Dispose();
	}

	#region Event methods.
	private void Attack_performed(InputAction.CallbackContext context)
	{
		onAttackAction?.Invoke(this, EventArgs.Empty);
	}

	private void AimMode_toggled(InputAction.CallbackContext context)
	{
		onAimModeToggleAction?.Invoke(this, context.phase);
	}

	private void ContinueDialogue_performed(InputAction.CallbackContext context)
	{
		onContinueDialogueAction?.Invoke(this, EventArgs.Empty);
	}
	
	private void Reload_performed(InputAction.CallbackContext context)
	{
		onReloadAction?.Invoke(this, EventArgs.Empty);
	}
	
	private void BackToMenu_performed(InputAction.CallbackContext context)
	{
		onBackToMenuAction?.Invoke(this, EventArgs.Empty);
	}
	
	private void SkipPlayable_performed(InputAction.CallbackContext context)
	{
		onSkipPlayableAction?.Invoke(this, EventArgs.Empty);
	}
	#endregion

	#region Get data and value methods.
	public TValue ReadValue<TValue>(KeybindingActions action) where TValue : struct
	{
		return _inputActions[action].ReadValue<TValue>();
	}

	public string GetDisplayString(KeybindingActions action, int index = 0)
	{
		ReadOnlyArray<InputBinding> bindings = _inputActions[action].bindings;
		index = Mathf.Clamp(index, 0, bindings.Count - 1);

		return bindings[index].ToDisplayString();
	}
	#endregion

	#region Get keys and mouse buttons methods.
	public Vector2 ScrollDelta => Mouse.current.scroll.ReadValue().normalized;
	public Vector2 MousePosition => Mouse.current.position.ReadValue();

	public bool GetMouseButtonDown(MouseButton button)
	{
		return GetMouseButtonControl(button).wasPressedThisFrame;
	}

	public bool GetMouseButtonHeld(MouseButton button)
	{
		return GetMouseButtonControl(button).isPressed;
	}

	public bool GetMouseButtonUp(MouseButton button)
	{
		return GetMouseButtonControl(button).wasReleasedThisFrame;
	}
	
	public bool GetKeyDown(Key key)
	{
		return Keyboard.current[key].wasPressedThisFrame;
	}

	public bool GetKeyHeld(Key key)
	{
		return Keyboard.current[key].isPressed;
	}

	public bool GetKeyUp(Key key)
	{
		return Keyboard.current[key].wasReleasedThisFrame;
	}

	public bool WasPressedThisFrame(KeybindingActions action)
	{
		return _inputActions[action].WasPressedThisFrame();
	}

	public bool IsPress(KeybindingActions action)
	{
		return _inputActions[action].IsPressed();
	}

	public bool WasReleasedThisFrame(KeybindingActions action)
	{
		return _inputActions[action].WasReleasedThisFrame();
	}
	#endregion

	#region Initialization and Clean up.
	private void Initialize()
	{
		_playerInputActions = new PlayerInputActions();

		_playerInputActions.Player.Enable();

		_inputActions ??= new Dictionary<KeybindingActions, InputAction>()
		{
			[KeybindingActions.Attack] = _playerInputActions.Player.Attack,
			[KeybindingActions.ToggleAimMode] = _playerInputActions.Player.ToggleAimMode,
			[KeybindingActions.Aiming] = _playerInputActions.Player.Aiming,

			[KeybindingActions.Movement] = _playerInputActions.Player.Movement,
			[KeybindingActions.ContinueDialogue] = _playerInputActions.Player.ContinueDialogue,
			[KeybindingActions.Interact] = _playerInputActions.Player.Interact,
			[KeybindingActions.Reload] = _playerInputActions.Player.Reload,
			[KeybindingActions.BackToMenu] = _playerInputActions.Player.BackToMenu,
			[KeybindingActions.SkipPlayable] = _playerInputActions.Player.SkipPlayable,
		};

		Subscribe(KeybindingActions.Attack, ActionEventType.Performed, Attack_performed);

		Subscribe(KeybindingActions.ToggleAimMode, ActionEventType.Started, AimMode_toggled);
		Subscribe(KeybindingActions.ToggleAimMode, ActionEventType.Canceled, AimMode_toggled);

		Subscribe(KeybindingActions.ContinueDialogue, ActionEventType.Performed, ContinueDialogue_performed);
		Subscribe(KeybindingActions.Reload, ActionEventType.Performed, Reload_performed);
		Subscribe(KeybindingActions.BackToMenu, ActionEventType.Performed, BackToMenu_performed);
		Subscribe(KeybindingActions.SkipPlayable, ActionEventType.Performed, SkipPlayable_performed);
	}

	private void Dispose()
	{
		Unsubscribe(KeybindingActions.Attack, ActionEventType.Performed, Attack_performed);

		Unsubscribe(KeybindingActions.ToggleAimMode, ActionEventType.Started, AimMode_toggled);
		Unsubscribe(KeybindingActions.ToggleAimMode, ActionEventType.Canceled, AimMode_toggled);

		Unsubscribe(KeybindingActions.ContinueDialogue, ActionEventType.Performed, ContinueDialogue_performed);
		Unsubscribe(KeybindingActions.Reload, ActionEventType.Performed, Reload_performed);
		Unsubscribe(KeybindingActions.BackToMenu, ActionEventType.Performed, BackToMenu_performed);
		Unsubscribe(KeybindingActions.SkipPlayable, ActionEventType.Performed, SkipPlayable_performed);
		
		_playerInputActions.Dispose();
	}
	#endregion

	#region Event subscription management.
	private void Subscribe(KeybindingActions action, ActionEventType eventType, Action<InputAction.CallbackContext> method)
	{
		switch (eventType)
		{
			case ActionEventType.Started:
				_inputActions[action].started += method;
				break;
			case ActionEventType.Performed:
				_inputActions[action].performed += method;
				break;
			case ActionEventType.Canceled:
				_inputActions[action].canceled += method;
				break;
		}
	}

	private void Unsubscribe(KeybindingActions action, ActionEventType eventType, Action<InputAction.CallbackContext> method)
	{
		switch (eventType)
		{
			case ActionEventType.Started:
				_inputActions[action].started -= method;
				break;
			case ActionEventType.Performed:
				_inputActions[action].performed -= method;
				break;
			case ActionEventType.Canceled:
				_inputActions[action].canceled -= method;
				break;
		}
	}
	#endregion

	private ButtonControl GetMouseButtonControl(MouseButton button)
	{
		return button switch
		{
			MouseButton.Left => Mouse.current.leftButton,
			MouseButton.Right => Mouse.current.rightButton,
			MouseButton.Middle => Mouse.current.middleButton,
			MouseButton.Forward => Mouse.current.forwardButton,
			MouseButton.Backward => Mouse.current.backButton,
			_ => Mouse.current.leftButton,  // Default will be the left mouse button.
		};
	}

	enum ActionEventType
	{
		Started,
		Performed,
		Canceled
	}
}

/// <summary>
/// Represents different actions in the game associated with different control keys.
/// </summary>
public enum KeybindingActions
{
	Attack,
	ToggleAimMode,
	Aiming,
	Movement,
	ContinueDialogue,
	Interact,
	Reload,
	BackToMenu,
	SkipPlayable
}

public enum MouseButton
{
	Left,
	Right,
	Middle,
	Forward,
	Backward
}