using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LevelMusicPlayer : MonoBehaviour {

	public static LevelMusicPlayer CurrentInstance { get; private set; }

	[SerializeField] private AudioClip deathMusic;
	private AudioSource source;

	private void Start() {
		CurrentInstance = this;
		source = GetComponent<AudioSource>();
		NewMusic();
	}

	public void NewMusic() {
		source.clip = LoadingManager.Instance.PickNewMusic();
		if(source.clip == null) {
			Debug.LogWarning("Could not find new music for level music player, for stage " + LoadingManager.Instance.Stage + ".");
			return;
		}
		Debug.Log("New music for stage " + LoadingManager.Instance.Stage + " : " + source.clip);
		source.Play();
		StartCoroutine(Utils.DoAfter(source.clip.length, () => NewMusic()));
	}

	public void PlayDeathMusic() {
		source.Stop();
		source.clip = deathMusic;
		source.loop = true;
		source.Play();
	}

}