using UnityEngine;

public class EnemyIndicator : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private Rigidbody2D rb2D;
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private Transform indicator;

	[Header("Settings"), Space]
	[SerializeField] private float distanceOffset;

	// Private fields.
	private Camera _camera;
	private Transform _cameraTransform;

	private void Start()
	{
		_camera = Camera.main;
		_cameraTransform = _camera.transform;
	}

	private void LateUpdate()
	{
		if (!spriteRenderer.isVisible)
		{
			indicator.gameObject.SetActive(true);

			Vector2 direction = rb2D.position - (Vector2)_cameraTransform.position;
			Vector2 positionDelta = direction.normalized * (_camera.orthographicSize - distanceOffset);

			indicator.position = (Vector2)_cameraTransform.position + positionDelta;
			indicator.right = direction;
		}
		else
		{
			indicator.gameObject.SetActive(false);
		}
	}
}