using System.Collections.Generic;
using UnityEngine;

namespace JamUtils2D {
	public class SpriteAnimator : MonoBehaviour {

		[SerializeField] private SpriteRenderer _spriteRenderer;
		[SerializeField] private bool playOnStart = false;
		[SerializeField] public SpriteAnimation defaultAnimation;

		private readonly Dictionary<string, SpriteAnimation> animations = new();

		private bool playing = false;
		private SpriteAnimation currentAnimation;
		public float currentFramerate; // cooldown between 2 images

		private void Awake() {
			if(_spriteRenderer == null)
				_spriteRenderer = GetComponent<SpriteRenderer>();
			if(_spriteRenderer == null)
				_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
			if(_spriteRenderer == null) {
				Debug.LogError("Could not find / get any SpriteRenderer component for SpriteAnimator.");
				enabled = false;
			}
		}

		private void Start() {
			if(playOnStart)
				Play();
		}

		public void Play() {
			if(defaultAnimation == null) {
				Debug.LogWarning("No default animation for SpriteAnimator. Cannot start a #Play().");
				return;
			}
			playing = true;
			currentAnimation = defaultAnimation;
		}

		public void SetAnimation(string animationName, SpriteAnimation animation, bool overrides = false) {
			if(animations.ContainsKey(animationName) && ! overrides) {
				Debug.LogError("Cannot register animation '" + animationName + "' without the 'overrides' parameter.");
				return;
			}
			animations[animationName] = animation;
		}

		public void RemoveAnimation(string animationName, bool ignoresNotHere = false) {
			if( ! animations.ContainsKey(animationName) && !ignoresNotHere) {
				Debug.LogError("Cannot remove animation '" + animationName + "' if it does not exist.");
				return;
			}
			animations.Remove(animationName);
		}

		public void Stop() {
			playing = false;
		}

		public void PlayOnce(string animation, float frameRateModifier = 1f) {
			if(animations.ContainsKey(animation)) {
				Debug.LogError("Cannot play(once) animation '" + animation + "' because it doesn't exist.");
				return;
			}
			playing = true;
			currentAnimation = animations[animation];
			currentFramerate = frameRateModifier;
		}
	}

}