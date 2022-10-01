using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CustomAnimatorUI : MonoBehaviour {

	[SerializeField] private Sprite[] sprites;
	[SerializeField] private float frameDuration;

	private Image _renderer;

	private float nextAnim;

	private void Start() {
		_renderer = GetComponent<Image>();
		nextAnim = Time.time;
	}

	private void LateUpdate() {
		if(Time.time >= nextAnim)
			UpdateImage();
	}

	private void UpdateImage() {
		_renderer.sprite = GetCurrent(nextAnim);
		nextAnim += frameDuration;
	}

	private Sprite GetCurrent(float time) {
		int frame = Mathf.FloorToInt(time / frameDuration) % sprites.Length;
		return sprites[frame];
	}

}