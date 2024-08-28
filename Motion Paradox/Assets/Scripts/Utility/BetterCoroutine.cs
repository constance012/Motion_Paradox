using System.Collections;
using UnityEngine;

public class BetterCoroutine
{
	private Coroutine _coroutine;

	/// <summary>
	/// Stop this coroutine if it's not null.
	/// </summary>
	/// <param name="mono"></param>
	public void StopCurrent(MonoBehaviour mono)
	{
		if (_coroutine != null)
			mono.StopCoroutine(_coroutine);
	}

	public void StartNew(MonoBehaviour mono, IEnumerator coroutine, bool stopPrevious = true)
	{
		if (stopPrevious)
			StopCurrent(mono);
		
		_coroutine = mono.StartCoroutine(coroutine);
	}
}