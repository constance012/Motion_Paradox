using UnityEngine;

public class Tutorial : MonoBehaviour
{
	[Header("Disappear Time"), Space]
	[SerializeField] private float disappearTime;

	private void Start()
	{
		Destroy(gameObject, disappearTime);
	}
}