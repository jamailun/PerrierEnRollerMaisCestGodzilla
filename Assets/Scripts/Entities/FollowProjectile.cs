using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FollowProjectile : Projectile {

	[SerializeField] private float targetFrequency = 0.2f;

	private Rigidbody2D _rb;
	private Transform target;

	private float nextRetarget;

	private void Awake() {
		_rb = GetComponent<Rigidbody2D>();
	}

	public void InitFollow(Transform target, Transform parent) {
		base.parent = parent;
		this.target = target;
		TargetAgain();
	}

	private void Update() {
		if(Time.time >= nextRetarget)
			TargetAgain();
	}

	private void TargetAgain() {
		nextRetarget = Time.time + targetFrequency;

		Vector3 dir = new Vector2(target.position.x - transform.position.x, target.position.y - transform.position.y) * PLAN;

		// set direction
		_rb.AddForce(dir.normalized * speed);
	}

}