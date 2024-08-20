using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public sealed class EnemyAnimation : MonoBehaviour
{
	[Header("Rotation"), Space]
	[SerializeField] private Vector3 rotateAngle;
	[SerializeField] private Vector2 spinSpeedRange;
	[SerializeField] private RotateMode rotateMode;

	[Header("Loops and Easing"), Space]
	[SerializeField] private LoopType loopType;
	[SerializeField] private Ease easeType = Ease.Linear;
	[SerializeField] private float delay;

	[Header("Custom update mode"), Space]
	[SerializeField] private UpdateType updateType;
	[SerializeField] private bool ignoreTimescale;

	// Private fields.
	private Tween _tween;

	private void Start()
	{
		_tween = transform.DOLocalRotate(rotateAngle, spinSpeedRange.RandomBetweenEnds(), RotateMode.FastBeyond360)
				 .SetLoops(-1, loopType)
				 .SetEase(easeType)
				 .SetSpeedBased(true)
				 .SetUpdate(updateType, ignoreTimescale)
				 .SetDelay(delay);
	}

	private void OnDestroy()
	{
		_tween.Kill(true);
	}
}