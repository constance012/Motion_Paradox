using UnityEngine;

public class WorldHealthBar : HealthBar
{
	[Header("World Position"), Space]
	[SerializeField] private Transform worldPos;
	
	// Private fields.
	private static Canvas worldCanvas;

	protected override void Awake()
	{
		base.Awake();
		if (worldCanvas == null)
		{
			worldCanvas = GameObject.FindWithTag("WorldCanvas").GetComponent<Canvas>();
		}
		
		transform.SetParent(worldCanvas.transform, false);
	}

	private void LateUpdate()
	{
		transform.position = worldPos.position;
	}
}
