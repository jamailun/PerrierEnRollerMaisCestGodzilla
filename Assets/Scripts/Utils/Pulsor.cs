using UnityEngine;

public class Pulsor : MonoBehaviour {
	[SerializeField] private float amplitude = 0.5f;
	[SerializeField] private float frequency = 0.5f;

	private Vector3 baseScale;
	private float currentScale = 1f;
	private bool down = true;

	private void Start() {
		baseScale = transform.localScale;
	}

	private void Update() {
		currentScale += frequency * Time.deltaTime * (down?-1f:1f);
		if(Mathf.Abs(currentScale) - 1f < amplitude)
			down = !down;
		transform.localScale = baseScale * currentScale;
	}

}