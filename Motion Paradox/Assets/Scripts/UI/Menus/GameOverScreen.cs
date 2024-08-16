using TMPro;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private TextMeshProUGUI timeLeftText;

	// Callback method when the player dies.
	public void SetTimeLeftText()
	{
		timeLeftText.text = $"only <color=#250300>{TimeManager.PortalTimerLeft:mm\\:ss}</color> left before the portal activates";
	}
}