using Unity.VisualScripting;
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

	public static bool IsBetweenEnds(this Vector2 vector, float target)
	{
		return target >= vector.x && target <= vector.y;
	}
	
	public static bool IsBetweenEnds(this Vector2Int vector, float target)
	{
		return target >= vector.x && target <= vector.y;
	}

	public static float Clamp(this Vector2 vector, float target)
	{
		return vector.x < vector.y ? Mathf.Clamp(target, vector.x, vector.y) :
									 Mathf.Clamp(target, vector.y, vector.x);
	}
	
	public static int Clamp(this Vector2Int vector, int target)
	{
		return vector.x < vector.y ? Mathf.Clamp(target, vector.x, vector.y) :
									 Mathf.Clamp(target, vector.y, vector.x);
	}

	public static float RangeLength(this Vector2 vector, bool absolute = true)
	{
		float range = vector.y - vector.x;
		return absolute ? Mathf.Abs(range) : range;
	}
	
	public static int RangeLength(this Vector2Int vector, bool absolute = true)
	{
		int range = vector.y - vector.x;
		return absolute ? Mathf.Abs(range) : range;
	}

	public static float Interpolate(this Vector2 vector, float t)
	{
		return Mathf.Lerp(vector.x, vector.y, t);
	}
}