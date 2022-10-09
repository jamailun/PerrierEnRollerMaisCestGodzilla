using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour {

	[SerializeField] private bool doAnimation = true;

	[Header("Static configuration")]
	[SerializeField] private Image background;
	[SerializeField] private Button shopButton;
	[SerializeField] private Button startGameButton;
	[SerializeField] private Button quitButton;
	[SerializeField] private RectTransform credits;
	[SerializeField] private Image lightningImage;
	[SerializeField] private TMPro.TMP_Text title1;
	[SerializeField] private TMPro.TMP_Text title2;

	[Header("Animation parameters")]
	[SerializeField] private Gradient backgroundGradient;
	[SerializeField] private float waitBeforeLight;
	[SerializeField] private float waitBeforeSound;
	[SerializeField] private float lightDuration;
	[SerializeField] private float waitBeforeCredits;
	[SerializeField] private float creditsSlideDuration;
	[SerializeField] private float waitbeforeButtons;

	[Header("Music")]
	[SerializeField] private AudioClip introClip;
	[SerializeField] private AudioClip outroClip;
	[SerializeField] private AudioClip menuMusic;

	private AudioSource audioSource;

	private void Start() {
		audioSource = gameObject.GetOrAddComponent<AudioSource>();

		// Animation
		if(!LoadingManager.MainMenuAnimated && doAnimation) {
			LoadingManager.MainMenuAnimated = true;
			background.color = backgroundGradient.Evaluate(0);
			startGameButton.gameObject.SetActive(false);
			quitButton.gameObject.SetActive(false);
			shopButton.gameObject.SetActive(false);
			credits.gameObject.SetActive(false);
			title1.gameObject.SetActive(false);
			title2.gameObject.SetActive(false);
			StartCoroutine(Animate());
		} else {
			PlayMusic();
		}
	}

	private void PlayMusic() {
		audioSource.loop = true;
		audioSource.clip = menuMusic;
		audioSource.Play();
		// shop
		if(PersistentData.RunsAmount > 0)
			shopButton.gameObject.SetActive(true);
	}

	private IEnumerator Animate() {
		yield return new WaitForSeconds(waitBeforeLight);

		audioSource.PlayOneShot(introClip);

		yield return new WaitForSeconds(waitBeforeSound);

		lightningImage.gameObject.SetActive(true);

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		lightningImage.gameObject.SetActive(false);

		// Background

		float elapsed = 0f;
		while(elapsed < lightDuration) {
			background.color = backgroundGradient.Evaluate(elapsed/lightDuration);
			elapsed += Time.deltaTime;
			yield return null;
		}

		// Credits
		yield return new WaitForSeconds(waitBeforeCredits);
		credits.anchoredPosition = new Vector2(-credits.sizeDelta.x, credits.anchoredPosition.y);
		credits.gameObject.SetActive(true);
		float speed = (credits.sizeDelta.x / creditsSlideDuration);
		elapsed = 0f;
		while(elapsed < creditsSlideDuration) {
			credits.anchoredPosition = new Vector2(credits.sizeDelta.x - (elapsed * speed), credits.anchoredPosition.y);
			elapsed += Time.deltaTime;
			yield return null;
		}

		// Buttons
		audioSource.PlayOneShot(outroClip);
		yield return new WaitForSeconds(waitbeforeButtons);

		lightningImage.gameObject.SetActive(true);

		startGameButton.gameObject.SetActive(true);
		quitButton.gameObject.SetActive(true);

		yield return new WaitForSeconds(0.1f);

		yield return new WaitForSeconds(0.15f);
		title1.gameObject.SetActive(true);

		yield return new WaitForSeconds(0.55f);

		title2.gameObject.SetActive(true);
		lightningImage.gameObject.SetActive(false);

		yield return new WaitForSeconds(0.5f);

		PlayMusic();
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