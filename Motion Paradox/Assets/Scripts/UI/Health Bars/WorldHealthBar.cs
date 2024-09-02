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
			Debug.Log("World canvas is null");
			worldCanvas = GameObject.FindWithTag("WorldCanvas").GetComponent<Canvas>();
		}
		
		transform.SetParent(worldCanvas.transform, false);
	}

	private void LateUpdate()
	{
		worldPos.position = worldPos.parent.position + Vector3.up;
		transform.position = worldPos.position;
	}
}
