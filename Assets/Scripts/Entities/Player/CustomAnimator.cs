using System.Collections.Generic;
using UnityEngine;

public class CustomAnimator : MonoBehaviour {

	private const bool DEBUG = "1" == "1";

	private readonly Dictionary<string, CustomAnimation> clips = new();

	private SpriteRenderer _spriteRenderer;

	private string nowPlayingName = null;
	private CustomAnimation nowPlaying = null;

	private int index = 0;
	private float nextIndexTime = float.MaxValue;

	private Utils.Predicate predicate;
	private Sprite predicateDefault;

	private void Start() {
		_spriteRenderer = GetComponent<SpriteRenderer>();
		if(_spriteRenderer == null)
			_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
	}

	private void LateUpdate() {
		if(nowPlaying == null)
			return;

		if(Time.time >= nextIndexTime) {
			UpdateImage(false);
		}
	}

	private float Framerate => nextAnimation == null ? nowPlaying.frameDuration : tempFrameDuration;

	public void SetClip(string name, CustomAnimation clip) {
		clips[name] = clip;
	}

	public void PlayConditional(string animationName, Utils.Predicate predicate, Sprite predicateDefault) {
		Play(animationName);
		this.predicate = predicate;
		this.predicateDefault = predicateDefault;
	}

	public void Play(string animationName, bool resetCallback = true) {
		if(nowPlayingName == animationName)
			return;
		if(!clips.ContainsKey(animationName)) {
			Debug.LogError("Could not play animation '" + animationName + "' : animation does NOT exist.");
			return;
		}
		nowPlayingName = animationName;
		nowPlaying = clips[animationName];

		if(DEBUG)
			Debug.Log("[CA " + gameObject.name + "] Started playing standard "+nowPlayingName+".");

		if(resetCallback)
			indexCallback = -1;

		nextIndexTime = Time.time + Framerate;
		index = 0;

		if(_spriteRenderer == null)
			Start();
		UpdateImage(true);
	}

	private string nextAnimation = null;
	private float tempFrameDuration;
	private float nextScale;

	public void PlayOnce(string animationName, float duration, string nextAnimation, float nextScale = 1f) {
		if(!clips.ContainsKey(animationName)) {
			Debug.LogError("Could not play ONCE animation '" + animationName + "' : animation does NOT exist.");
			return;
		}
		if(!clips.ContainsKey(nextAnimation)) {
			Debug.LogError("Could not play NEXT animation '" + nextAnimation + "' : animation does NOT exist.");
			return;
		}
		nowPlayingName = animationName;
		nowPlaying = clips[animationName];

		this.nextAnimation = nextAnimation;
		this.nextScale = nextScale;

		tempFrameDuration = duration / nowPlaying.points.Length;

		if(DEBUG)
			Debug.Log("[CA " + gameObject.name + "] Started playing " + animationName + " -> " + nextAnimation + ". Dur=" + duration + ". Frames=" + nowPlaying.points.Length + "; tmps/frame = "+ tempFrameDuration);

		nextIndexTime = Time.time + Framerate;
		index = 0;

		if(_spriteRenderer == null)
			Start();
		UpdateImage(true);
	}

	private int indexCallback = -1;
	private Utils.Runnable callback;
	public void SpecifyCallback(int indexExact, Utils.Runnable callback) {
		this.indexCallback = indexExact;
		this.callback = callback;
	}

	public void Stop() {
		nowPlaying = null;
		nowPlayingName = null;
	}

	public bool IsPlaying() {
		return nowPlayingName != null;
	}

	private void UpdateImage(bool onlyRefresh) {
		// CALLED ONLY WHEN (Time.time >= nextIndexTime)


		if(predicate != null && nextAnimation == null) {
			// predicat + pas d'override temporaire
			_spriteRenderer.sprite = predicate.Invoke() ? nowPlaying.points[index] : predicateDefault;
			if(DEBUG)
				Debug.Log("[CA " + gameObject.name + "] drew " + _spriteRenderer.sprite.name);
		} else {
			try {
				_spriteRenderer.sprite = nowPlaying.points[index];
			} catch {
				Debug.LogError("aaaa : " + _spriteRenderer + " / " + nowPlaying);
			}
			if(DEBUG)
				Debug.Log("[CA " + gameObject.name + "] drew index "+index+" : " + _spriteRenderer.sprite.name);
		}

		if(onlyRefresh)
			return;

		index++;
		nextIndexTime += Framerate;

		if(index == indexCallback) {
			callback.Invoke();
		}

		if(index == nowPlaying.points.Length) {
			index = 0;

			// fin de la bande.
			if(nextAnimation != null) {
				// il y a autre chose après.

				nowPlayingName = nextAnimation;
				nowPlaying = clips[nextAnimation];
				nextAnimation = null;
				_spriteRenderer.transform.localScale = new Vector3(nextScale, nextScale, 1f);
				_spriteRenderer.sprite = nowPlaying.points[index];

				if(DEBUG)
					Debug.Log("[CA " + gameObject.name + "] end of anim, went back to " + nowPlayingName);
			} else {
				// rien après.
			}
		}
	}

}