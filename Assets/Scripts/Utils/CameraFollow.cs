using UnityEngine;

public class CameraFollow : MonoBehaviour {

	[Tooltip("The target to follow")]
	[SerializeField] internal Transform target;

	[Tooltip("The offset")]
	[SerializeField] private Vector3 offset = new(0, 0, -10);

	[Tooltip("The smoothing time required for damping.")]
	[SerializeField] private float smoothTime = 0.25f;

	private Vector3 _velocity;

	private void Start() {
		if(target == null)
			Debug.LogWarning("Follow camera " + name + " does NOT have a referenced target.");
	}

	void FixedUpdate() {
		if(target == null)
			return;

		Vector3 targetPos = target.position + offset;
		transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, smoothTime);
	}

}