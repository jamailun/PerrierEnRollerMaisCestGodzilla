using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	[SerializeField] private Button shopButton;
	[SerializeField] private AudioClip menuMusic;

	private AudioSource audioSource;

	private void Start() {
		var runs = PersistentData.RunsAmount;
		if(runs > 0)
			shopButton.gameObject.SetActive(true);

		// Music
		audioSource = gameObject.GetOrAddComponent<AudioSource>();
		audioSource.loop = true;
		audioSource.clip = menuMusic;
		audioSource.Play();
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
		audioSource.Stop();
		LoadingManager.Instance.StartGame();
	}

	public void SelectedShop() {
		Debug.Log("shop selected.");
	}
	
}