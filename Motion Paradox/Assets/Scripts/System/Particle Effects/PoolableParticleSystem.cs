using UnityEngine;
using static UnityEngine.ParticleSystem;

public sealed class PoolableParticleSystem : PoolableEffectBase
{
	[Header("System References"), Space]
	[SerializeField] private ParticleSystem effect;

	public ParticleSystem Effect => effect;

	private void Awake()
	{
		MainModule main = effect.main;
		main.playOnAwake = false;
	}

	public override void Play() => effect.Play();
	public override void Stop() => effect.Stop();
	public override void Clear() => effect.Clear();

	public void SetCustomSimulationSpace(Transform space)
	{
		MainModule main = effect.main;
		main.customSimulationSpace = space;
	}

	protected override void BeforeDeallocate()
	{
		base.BeforeDeallocate();
		SetCustomSimulationSpace(null);
	}
}