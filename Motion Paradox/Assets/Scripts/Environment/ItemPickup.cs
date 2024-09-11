using UnityEngine;
using DG.Tweening;

public sealed class ItemPickup : Interactable
{
	[Header("References"), Space]
	public Item itemSO;
	[SerializeField] private Rigidbody2D rb2D;

	[Header("Life Time"), Space]
	[Tooltip("The life time of this item after it's dropped, enter -1 to for infinite life time.")]
	[SerializeField] private float lifeTime;

	[Header("Fly Settings"), Space]
	[SerializeField] private float flyDistance;
	[SerializeField] private float flySpeed;
	[SerializeField] private float pickUpMinDistance;
	[SerializeField] private float pickUpFailDelay;

	public int ItemQuantity
	{
		get { return _currentItem.quantity; }
		set { _overrideQuantity = value; }
	}

	// Private fields.
	private Item _currentItem;
	private int _overrideQuantity = -1;
	private bool _pickedUp;
	private float _delay;

	private void Start()
	{		
		_currentItem = Instantiate(itemSO);
		_currentItem.name = itemSO.name;

		if (_overrideQuantity != -1)
			_currentItem.quantity = _overrideQuantity;

		spriteRenderer.sprite = _currentItem.icon;		
	}

	protected override void CheckForInteraction(float mouseDistance, float playerDistance)
	{
		CheckForLifeTime();		

		if (playerDistance <= flyDistance)
			FlyTowardsPlayer();

		base.CheckForInteraction(mouseDistance, playerDistance);
	}

	protected override void CreatePopupLabel()
	{
		Transform foundLabel = _worldCanvas.transform.Find("Popup Label");

		string itemName = _currentItem.displayName;
		int quantity = _currentItem.quantity;
		Color textColor = _currentItem.rarity.color;

		// Create a clone if not already exists.
		if (foundLabel == null)
		{
			base.CreatePopupLabel();
			_popupLabel.SetLabelName(itemName, quantity, textColor);
		}

		// Otherwise, append to the existing one.
		else
		{
			_popupLabel = foundLabel.GetComponent<InteractionPopupLabel>();

			_popupLabel.SetLabelName(itemName, quantity, textColor, true);
			_popupLabel.RestartAnimation();
		}
	}

	public override void Interact()
	{
		if (!_pickedUp)
		{
			Debug.Log($"You're picking up a(n) {_currentItem.displayName}");
			
			if (_currentItem.autoUse && _currentItem.Use(_player, forced: _delay > 0f))
			{
				_pickedUp = true;
				DestroyGameObject();
			}
			else
			{
				_delay = pickUpFailDelay;
			}
		}
	}

	private void CheckForLifeTime()
	{
		if (lifeTime != -1f)
		{
			lifeTime -= Time.deltaTime;
			if (lifeTime <= 0f)
				DestroyGameObject();
		}
	}

    private void FlyTowardsPlayer()
	{
		_delay -= Time.deltaTime;

		if (_delay <= 0f)
		{
			Vector2 flyDirection = _player.position - transform.position;
			rb2D.velocity = flyDirection.normalized * flySpeed;

			if (flyDirection.sqrMagnitude <= Mathf.Pow(pickUpMinDistance, 2))
				Interact();
		}
	}

	private void DestroyGameObject()
	{
		transform.DOScale(0f, .2f)
				.OnComplete(() =>
				{
					if (_popupLabel != null)
						Destroy(_popupLabel.gameObject);
					Destroy(gameObject);
				});

		lifeTime = -1f;
	}

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, flyDistance);
    }
}