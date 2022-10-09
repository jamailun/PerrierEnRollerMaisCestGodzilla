using UnityEngine;

public class GameOverScreen : MonoBehaviour {

	[Header("References")]
	[SerializeField] private AudioClip congratsSounds;
	[SerializeField] private AudioClip winLoop;

	private AudioSource source;

	private void Start() {
		source = gameObject.GetOrAddComponent<AudioSource>();
		source.PlayOneShot(congratsSounds);
		StartCoroutine(Utils.DoAfter(winLoop.length - 0.5f, () => {
			source.clip = winLoop;
			source.loop = true;
			source.Play();
		}));
	}

	public void GoToMainMenu() {
		LoadingManager.ResetGameAndGoMenu();
	}
	
}