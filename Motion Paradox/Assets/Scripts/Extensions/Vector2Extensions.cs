using UnityEngine;

public static class Vector2Extensions
{
	public static float RandomBetweenEnds(this Vector2 vector)
	{
		return Random.Range(vector.x, vector.y);
	}

	public static int RandomBetweenEnds(this Vector2Int vector)
	{
		return Random.Range(vector.x, vector.y);
	}
}