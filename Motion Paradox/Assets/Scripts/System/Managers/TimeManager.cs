using System;
using UnityEngine;

public sealed class TimeManager : Singleton<TimeManager>
{
	[Header("Portal Activation Timer (Seconds)"), Space]
	[SerializeField] private float portalActivationTimer;

	public static float LocalTimeScale
	{
		get { return Instance._localTimeScale; }
		set { Instance._localTimeScale = value; }
	}

	public static float GlobalTimeScale
	{
		get { return Time.timeScale; }
		set
		{
			LocalTimeScale = value;
			Time.timeScale = value;
		}
	}
	
	public static float PortalTimerDuration => Instance.portalActivationTimer;
	public static TimeSpan PortalTimerLeft => Instance._portalTimer;
	public static bool IsPortalTimerUp => Instance._portalTimer <= TimeSpan.Zero;
	
	// Private fields.
	private TimeSpan _portalTimer;
	private float _localTimeScale = 1f;

	private void Start()
	{
		_portalTimer = TimeSpan.FromSeconds(portalActivationTimer);
	}

	private void Update()
	{
		_portalTimer -= TimeSpan.FromSeconds(Time.deltaTime) * LocalTimeScale;
	}
}