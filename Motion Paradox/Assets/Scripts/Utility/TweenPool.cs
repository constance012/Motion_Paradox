using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;

/// <summary>
/// An utility class for storing and manages tweens.
/// </summary>
public sealed class TweenPool
{
	public int Count => _activeTweens.Count;
	public HashSet<Tween> ActiveTweens => _activeTweens;

	private HashSet<Tween> _activeTweens;

	public TweenPool()
	{
		_activeTweens = new HashSet<Tween>();
	}

	public void Add(Tween tween)
	{
		_activeTweens.Add(tween);
	}

	public void KillActiveTweens(bool completed)
	{
		foreach (Tween tween in _activeTweens)
		{
			if (tween.IsActive())
				tween.Kill(completed);
		}

		_activeTweens.Clear();
	}

	public async Task RewindAndKillActiveTweens(bool completed)
	{
		foreach (Tween tween in _activeTweens)
		{
			if (tween.IsActive())
			{
				tween.Rewind();
				await tween.AsyncWaitForRewind();
				tween.Kill(completed);
			}
		}

		_activeTweens.Clear();
	}
}