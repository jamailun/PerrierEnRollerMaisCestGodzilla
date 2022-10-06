using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour {

	public void SelectedRagequit() {
		Debug.Log("Ragequit. Go to main menu");
		//gameObject.SetActive(false);
		//SceneManager.LoadScene(0);
		LoadingManager.ResetGameAndGoMenu();
	}

	public void SelectedShop() {
		Debug.Log("shop selected. mais ça n'existe pas. donc go au menu.");
		LoadingManager.ResetGameAndGoMenu();
	}
	
}