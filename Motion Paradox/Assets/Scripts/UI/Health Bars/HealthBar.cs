using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	[Header("Health Segments"), Space]
	[SerializeField] private GameObject segmentPrefab;
	[SerializeField] private Sprite evenSegmentSprite;
	[SerializeField] private Sprite oddSegmentSprite;
	[SerializeField] private Sprite emptySegmentSprite;

	[Header("Segments Settings"), Space]
	[SerializeField] private Transform segmentParent;
	[SerializeField] private Color emptyingColor;
	[SerializeField] private Color fillingColor;

	[Header("Flashing Effect Settings"), Space]
	[SerializeField] private float duration;
	[SerializeField] private int overshoot;
	[SerializeField, Range(-1f, 1f)] private float period;

	// Private fields.
	private TweenPool _tweenPool;
	private Image[] _segments;
	private int _previousHealth;

	protected virtual void Awake()
	{
		_tweenPool = new TweenPool();
	}

	private void OnDestroy()
	{
		_tweenPool.KillActiveTweens(true);
	}

	public async void SetCurrentHealth(float current)
	{
		_tweenPool.KillActiveTweens(true);

		int currentInt = (int)current;
		Task[] modifiedSegments = new Task[Mathf.Abs(_previousHealth - currentInt)];

		if (currentInt < _previousHealth)
		{
			for (int i = currentInt; i < _previousHealth; i++)
			{
				modifiedSegments[i - currentInt] = EmptyingSegment(i);
			}
		}
		else
		{
			for (int i = _previousHealth; i < currentInt; i++)
			{
				modifiedSegments[i - _previousHealth] = FillingSegment(i);
			}
		}
		
		_previousHealth = currentInt;
		await Task.WhenAll(modifiedSegments);
	}

	public void SetMaxHealth(float max)
	{
		int maxInt = (int)max;
		_previousHealth = maxInt;

		if (_segments == null || _segments.Length != max)
		{
			segmentParent.DestroyAllChildren(Destroy);
			_segments = new Image[maxInt];

			for (int i = 0; i < _segments.Length; i++)
			{
				Image segment = Instantiate(segmentPrefab, segmentParent).GetComponent<Image>();
				segment.sprite = (i % 2 == 0) ? evenSegmentSprite : oddSegmentSprite;
				
				_segments[i] = segment;
			}
		}
		else
		{
			for (int i = 0; i < _segments.Length; i++)
			{
				_segments[i].sprite = (i % 2 == 0) ? evenSegmentSprite : oddSegmentSprite;
			}
		}
	}

	private async Task EmptyingSegment(int index)
	{
		Tween tween = PerformEffect(index, emptyingColor)
			 		 .OnComplete(() => _segments[index].sprite = emptySegmentSprite);
		
		_tweenPool.Add(tween);

		await tween.AsyncWaitForCompletion();
	}

	private async Task FillingSegment(int index)
	{
		_segments[index].sprite = (index % 2 == 0) ? evenSegmentSprite : oddSegmentSprite;

		Tween tween = PerformEffect(index, fillingColor);
		_tweenPool.Add(tween);

		await tween.AsyncWaitForCompletion();
	}

	private Tween PerformEffect(int index, Color color)
	{
		return _segments[index].DOColor(color, duration)
							   .SetEase(Ease.Flash, overshoot, period);
	}
}
