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
	public EventHandler onBackToMenuAction;
	public EventHandler onAttackAction;
	public EventHandler onReloadAction;
	public EventHandler onTakeScreenshotAction;

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

	#region Event Methods.
	private void BackToMenu_performed(InputAction.CallbackContext context)
	{
		onBackToMenuAction?.Invoke(this, EventArgs.Empty);
	}

	private void Attack_performed(InputAction.CallbackContext context)
	{
		onAttackAction?.Invoke(this, EventArgs.Empty);
	}

	private void Reload_performed(InputAction.CallbackContext context)
	{
		onReloadAction?.Invoke(this, EventArgs.Empty);
	}

	private void TakeScreenshot_performed(InputAction.CallbackContext context)
	{
		onTakeScreenshotAction?.Invoke(this, EventArgs.Empty);
	}
	#endregion

	#region Public Wrapper Method.
	public Vector2 Read2DVector(KeybindingActions action)
	{
		return _inputActions[action].ReadValue<Vector2>().normalized;
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

	public bool WasPerformedThisFrame(KeybindingActions action)
	{
		return _inputActions[action].WasPerformedThisFrame();
	}

	public bool WasReleasedThisFrame(KeybindingActions action)
	{
		return _inputActions[action].WasReleasedThisFrame();
	}
	#endregion

	private void Initialize()
	{
		_playerInputActions = new PlayerInputActions();

		_playerInputActions.Player.Enable();

		_playerInputActions.Player.BackToMenu.performed += BackToMenu_performed;
		_playerInputActions.Player.Attack.performed += Attack_performed;
		_playerInputActions.Player.Reload.performed += Reload_performed;
		_playerInputActions.Player.TakeScreenshot.performed += TakeScreenshot_performed;

		_inputActions ??= new Dictionary<KeybindingActions, InputAction>()
		{
			[KeybindingActions.Attack] = _playerInputActions.Player.Attack,

			[KeybindingActions.MoveLeft] = _playerInputActions.Player.Movement,
			[KeybindingActions.MoveRight] = _playerInputActions.Player.Movement,
			[KeybindingActions.MoveUp] = _playerInputActions.Player.Movement,
			[KeybindingActions.MoveDown] = _playerInputActions.Player.Movement,

			[KeybindingActions.Interact] = _playerInputActions.Player.Interact,
			[KeybindingActions.Reload] = _playerInputActions.Player.Reload,
			
			[KeybindingActions.BackToMenu] = _playerInputActions.Player.BackToMenu,
		};
	}

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

	private void Dispose()
	{
		_playerInputActions.Player.BackToMenu.performed -= BackToMenu_performed;
		
		_playerInputActions.Dispose();
	}
}

/// <summary>
/// Represents different actions in the game associated with different control keys.
/// </summary>
public enum KeybindingActions
{
	Attack,
	MoveLeft,
	MoveRight,
	MoveUp,
	MoveDown,
	Reload,
	Interact,
	Pause,
	BackToMenu,
}

public enum MouseButton
{
	Left,
	Right,
	Middle,
	Forward,
	Backward
}