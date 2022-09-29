using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CustomAnimator : MonoBehaviour {

	private readonly Dictionary<string, CustomAnimation> clips = new();

	private SpriteRenderer _spriteRenderer;

	private string nowPlayingName = null;
	private CustomAnimation nowPlaying;

	private float nextAnim;

	private void Start() {
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void LateUpdate() {
		if(nowPlaying == null)
			return;
		if(Time.time >= nextAnim)
			UpdateImage();
	}

	public void SetClip(string name, CustomAnimation clip) {
		clips[name] = clip;
	}

	public void Play(string animationName) {
		if(nowPlayingName == animationName)
			return;
		if(!clips.ContainsKey(animationName)) {
			Debug.LogError("Could not play animation '" + animationName + "' : animation does NOT exist.");
			return;
		}
		nowPlayingName = animationName;
		nowPlaying = clips[animationName];

		nextAnim = Time.time;

		UpdateImage();
	}

	private void UpdateImage() {
		_spriteRenderer.sprite = nowPlaying.GetCurrent(nextAnim);

		nextAnim += nowPlaying.frameDuration;
	}

}