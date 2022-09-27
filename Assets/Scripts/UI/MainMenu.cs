using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	// Objectif : savoir si le joueur peut accéder aux "secrets" et "shop"

	[SerializeField] private Button shopButton;

	private void Start() {
		var runs = PersistentData.RunsAmount;
		if(runs > 0)
			shopButton.gameObject.SetActive(true);
	}

	public void SelectedQuitApplication() {
		Debug.Log("Quitting application. See you a next time !");
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit(0);
#endif
	}

	public void SelectedStartGame() {
		SceneManager.LoadScene("LoadingScreen");
		LoadingManager.Instance.NextStage();
	}

	public void SelectedShop() {
		Debug.Log("shop selected.");
	}
	
}