using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LevelMusicPlayer : MonoBehaviour {

	private AudioSource source;

	private void Start() {
		source = GetComponent<AudioSource>();
		NewMusic();
	}

	public void NewMusic() {
		Debug.Log("NEW MUSIC ! (" + LoadingManager.Instance.Stage + ")");
		source.clip = LoadingManager.Instance.PickNewMusic();
		if(source.clip == null) {
			Debug.LogWarning("Could not find new music for level music player.");
			return;
		}
		source.Play();
		StartCoroutine(Utils.DoAfter(source.clip.length, () => NewMusic()));
	}

}