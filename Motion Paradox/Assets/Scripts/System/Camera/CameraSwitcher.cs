using UnityEngine;
using AYellowpaper.SerializedCollections;
using Cinemachine;
using UnityEngine.InputSystem;

public sealed class CameraSwitcher : Singleton<CameraSwitcher>
{
	[Header("Subscribed Cameras"), Space]
	[SerializeField] private SerializedDictionary<CameraType, CinemachineVirtualCameraBase> cameras;

	public CinemachineVirtualCameraBase Active => _activeCamera;

	// Private fields.
	private CinemachineVirtualCameraBase _activeCamera;

	private void Start()
	{
		InputManager.Instance.OnAimModeToggleAction += (sender, phase) => ManageAimMode(phase);
	}

	public void Switch(CameraType type)
	{
		if (_activeCamera != cameras[type])
		{
			foreach (var camera in cameras)
			{
				if (camera.Key == type)
				{
					camera.Value.Priority = 20;
					_activeCamera = camera.Value;
				}
				else
					camera.Value.Priority = 0;
			}
		}
	}

	private void ManageAimMode(InputActionPhase phase)
	{
		if (phase == InputActionPhase.Started)
			Switch(CameraType.AimCamera);
		else
			Switch(CameraType.FollowCamera);
			
	}
}

public enum CameraType
{
	FollowCamera,
	AimCamera
}