using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LevelMusicPlayer : MonoBehaviour {

	private AudioSource source;

	private void Start() {
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

}