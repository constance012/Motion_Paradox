using System;

public static class TimeManager
{
	public static float LocalTimeScale { get; set; } = 1f;
	public static TimeSpan PortalTimerLeft => _portalTimer;
	public static bool IsPortalTimerUp => _portalTimer <= TimeSpan.Zero;
	
	// Private fields.
	private static TimeSpan _portalTimer;

	public static void InitializePortalTimer(float startTime)
	{
		_portalTimer = TimeSpan.FromSeconds(startTime);
	}

	public static void CountdownPortalTimer(float amountSeconds)
	{
		_portalTimer -= TimeSpan.FromSeconds(amountSeconds);
	}
}