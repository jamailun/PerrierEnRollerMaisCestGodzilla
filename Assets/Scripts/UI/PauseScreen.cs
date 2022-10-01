using UnityEngine;

public class PauseScreen : MonoBehaviour {

	public void SelectedRagequit() {
		Debug.Log("Ragequit. kill the player");

		//TODO kill the player
	}

	public void SelectedContinue() {
		GetComponentInParent<ManagerUI>().Unpause();
	}
	
}