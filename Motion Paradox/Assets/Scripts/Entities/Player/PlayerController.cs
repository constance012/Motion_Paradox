using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private Rigidbody2D rb2D;
	[SerializeField] private Animator animator;
	[SerializeField] private Stats stats;

	[Header("Movement Settings"), Space]
	[SerializeField] private float acceleration;
	[SerializeField] private float deceleration;

	public static Vector2 Position { get; private set; }

	// Private fields.d
	private Vector2 _movementDirection;
	private Vector2 _previousDirection;
	private float _maxSpeed;
	private float _currentSpeed;

	private void Start()
	{
		_maxSpeed = stats.GetDynamicStat(Stat.MoveSpeed);
	}

	private void Update()
	{
		_movementDirection = InputManager.Instance.ReadValue<Vector2>(KeybindingActions.Movement);

		if (_movementDirection.sqrMagnitude > .01f)
			_previousDirection = _movementDirection;
	}
	
	private void FixedUpdate()
	{
		UpdateVelocity();

		Position = rb2D.position;
	}

	private void UpdateVelocity()
	{
		animator.SetFloat("Speed", rb2D.velocity.sqrMagnitude);

		if (rb2D.velocity.sqrMagnitude >= .04f)
			TimeManager.LocalTimeScale = Mathf.InverseLerp(_maxSpeed, 0f, _currentSpeed);
		else
			TimeManager.LocalTimeScale = 1f;

		if (_movementDirection.sqrMagnitude > .01f)
		{
			_currentSpeed += acceleration * Time.deltaTime;
			_currentSpeed = Mathf.Min(_maxSpeed, _currentSpeed);
			
			rb2D.velocity = _movementDirection * _currentSpeed;
		}

		else if (_currentSpeed > 0f)
		{
			_currentSpeed -= deceleration * Time.deltaTime;
			_currentSpeed = Mathf.Max(0f, _currentSpeed);

			rb2D.velocity = _previousDirection * _currentSpeed;
		}
	}
}
