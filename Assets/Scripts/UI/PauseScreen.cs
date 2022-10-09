using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour {

	private bool ragequitting = false;

	public void SelectedRagequit() {
		ragequitting = true;
		Debug.Log("Ragequit. kill the player");

		// Save data
		new SceneData(SceneManager.GetActiveScene()).player.Conceede();
		// go to main menu
		LoadingManager.ResetGameAndGoMenu();
	}

	public void SelectedContinue() {
		if(ragequitting)
			return;
		GetComponentInParent<ManagerUI>().Unpause();
	}
	
}