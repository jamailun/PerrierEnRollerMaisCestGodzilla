using UnityEngine;

public abstract class Attractible : MonoBehaviour {

	private bool attracted;
	public bool Attracted => attracted;
	private float force = 1f;
	private Transform target;

	private void Start() {
		// disable the Update loop at start
		this.enabled = false;
	}

	public void StartAttract(Transform transform, float force) {
		attracted = true;
		target = transform;
		this.force = force;
		this.enabled = true;
	}

	private void Update() {
		if(!attracted)
			return;

		transform.position = Vector2.MoveTowards(transform.position, target.position, force * Time.deltaTime);
	}

}