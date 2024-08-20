using TMPro;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
	[Header("References"), Space]
	[SerializeField] private TextMeshProUGUI timeLeftText;

	// Callback method when the player dies.
	public void SetTimeLeftText()
	{
		if (!TimeManager.IsPortalTimerUp)
			timeLeftText.text = $"only <color=#250300>{TimeManager.PortalTimerLeft:mm\\:ss}</color> left before the portal activates";
		else
			timeLeftText.text = $"the portal is activated, but <color=#250300>no one</color> passes through";
	}
}