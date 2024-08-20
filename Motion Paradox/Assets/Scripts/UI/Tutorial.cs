using UnityEngine;

public class Tutorial : MonoBehaviour
{
	[Header("Disappear Time"), Space]
	[SerializeField] private float disappearTime;

	private void Start()
	{
		#if UNITY_EDITOR
			disappearTime = 3f;
		#endif
		Destroy(gameObject, disappearTime);
	}
}