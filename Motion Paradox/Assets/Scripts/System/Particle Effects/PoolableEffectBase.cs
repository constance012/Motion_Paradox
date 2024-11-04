using UnityEngine;

public abstract class PoolableEffectBase : MonoBehaviour, IPoolable
{
	[Header("Settings"), Space]
	[SerializeField] protected bool playOnAllocate;

	public void Allocate()
	{
		gameObject.SetActive(true);
		AfterAllocate();
	}

	public void Deallocate()
	{
		BeforeDeallocate();
		gameObject.SetActive(false);
	}

	public abstract void Play();
	public abstract void Stop();
	public abstract void Clear();

	protected virtual void BeforeDeallocate()
	{
		Stop();
		Clear();
	}

	protected virtual void AfterAllocate()
	{
		if (playOnAllocate)
			Play();
	}
}