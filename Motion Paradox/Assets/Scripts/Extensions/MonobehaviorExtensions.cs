using UnityEngine;

public static class MonoBehaviourExtensions
{
	public static bool TryStopCoroutine(this MonoBehaviour mono, Coroutine coroutine)
	{
		if (coroutine != null)
			mono.StopCoroutine(coroutine);

		return coroutine != null;
	}
}