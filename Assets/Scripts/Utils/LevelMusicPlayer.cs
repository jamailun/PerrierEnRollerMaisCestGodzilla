using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LevelMusicPlayer : MonoBehaviour {

	public static LevelMusicPlayer CurrentInstance { get; private set; }

	[SerializeField] private AudioClip deathMusic;
	[SerializeField] private AudioClip bossMusic;

	private AudioSource source;

	private void Start() {
		CurrentInstance = this;
		source = GetComponent<AudioSource>();
		NewMusic();
	}

	private bool canChange = true;

	public void NewMusic() {
		canChange = true;
		source.clip = LoadingManager.Instance.PickNewMusic();
		if(source.clip == null) {
			return;
		}
		Debug.Log("New music for stage " + LoadingManager.Instance.Stage + " : " + source.clip);
		source.Play();
		StartCoroutine(Utils.DoAfter(source.clip.length, () => {
			if(canChange)
				NewMusic();
		}));
	}

	public void Stop() {
		canChange = false;
		source.Stop();
	}

	public void PlayDeathMusic() {
		canChange = false;
		source.Stop();
		source.clip = deathMusic;
		source.loop = true;
		source.Play();
	}
	public void PlayBossMusic() {
		canChange = false;
		source.Stop();
		source.clip = bossMusic;
		source.loop = true;
		source.Play();
	}

}