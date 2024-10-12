using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using DG.Tweening;
using CSTGames.Utility;

public class PlayerAiming : MonoBehaviour
{
	[Header("Weapon References"), Space]
	[SerializeField] private HandheldWeapon weapon;

	[Header("Aiming Settings"), Space]
	[SerializeField] private CinemachineVirtualCamera aimCamera;
	[SerializeField, Range(1f, 5f)] private float aimSmoothSpeed;
	[SerializeField] private bool overrideUserSettings;

	// Private fields.
	private TweenPool _tweenPool;
	private CinemachineFramingTransposer _aimTransposer;
	private Vector2 _smoothVel = Vector2.zero;
	private float _aimSmoothTime;

	private void Awake()
	{
		_tweenPool = new TweenPool();
		_aimTransposer = aimCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
	}

	private void Start()
	{
		CursorManager.Instance.SwitchCursorTexture(CursorTextureType.Crosshair);
		InputManager.Instance.OnAimModeToggleAction += (sender, phase) => ManageAimMode(phase);
		
		_aimSmoothTime = NumberManipulator.RangeConvert(overrideUserSettings ? aimSmoothSpeed : UserSettings.AimSpeed, 1f, 5f, 1f, .1f);
	}

	private void FixedUpdate()
	{
		if (TimeManager.GloballyPaused)
			return;
			
		HandleAiming();
	}

	private void ManageAimMode(InputActionPhase phase)
	{
		weapon.ToggleAiming(phase == InputActionPhase.Started);

		if (phase == InputActionPhase.Started)
		{
			_tweenPool.KillActiveTweens(false);
		}
		else
		{
			_tweenPool.Add(EndingAimMode(_aimTransposer.m_ScreenX, (v) => _aimTransposer.m_ScreenX = v));
			_tweenPool.Add(EndingAimMode(_aimTransposer.m_ScreenY, (v) => _aimTransposer.m_ScreenY = v));
		}
	}

	private void HandleAiming()
	{
		if (InputManager.Instance.IsPress(KeybindingActions.ToggleAimMode))
		{
			Vector2 mousePos = InputManager.Instance.MousePosition;

			mousePos.x /= Screen.width;
			mousePos.y /= Screen.height;
			
			float targetX = 1f - mousePos.x;
			float targetY = mousePos.y;

			_aimTransposer.m_ScreenX = Mathf.SmoothDamp(_aimTransposer.m_ScreenX, targetX, ref _smoothVel.x, _aimSmoothTime);
			_aimTransposer.m_ScreenY = Mathf.SmoothDamp(_aimTransposer.m_ScreenY, targetY, ref _smoothVel.y, _aimSmoothTime);
		}
	}

	private Tween EndingAimMode(float startValue, TweenCallback<float> updateCallback)
	{
		return DOVirtual.Float(startValue, .5f, 1f, updateCallback).SetEase(Ease.InSine);
	}
}