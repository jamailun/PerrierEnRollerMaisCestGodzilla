using UnityEditor;
using UnityEngine;

namespace JamUtils2D {
	[System.Serializable]
	public class SpriteAnimation {

		public float frameDuration = 0.15f;

		public Sprite[] sprites;

		public float Duration => (sprites != null ? sprites.Length : 0) * Mathf.Max(0, frameDuration);

	}
}