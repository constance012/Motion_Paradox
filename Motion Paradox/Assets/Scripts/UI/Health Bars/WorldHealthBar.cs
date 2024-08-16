﻿using UnityEngine;

public class WorldHealthBar : HealthBar
{
	[Header("World Position"), Space]
	[SerializeField] private Transform worldPos;
	
	// Private fields.
	private static Canvas worldCanvas;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void ClearStatic()
	{
		worldCanvas = null;
	}

	protected override void Awake()
	{
		base.Awake();
		if (worldCanvas == null)
		{
			worldCanvas = GameObject.FindWithTag("WorldCanvas").GetComponent<Canvas>();
		}
		
		transform.SetParent(worldCanvas.transform);
	}

	private void LateUpdate()
	{
		transform.position = worldPos.position;
	}
}
