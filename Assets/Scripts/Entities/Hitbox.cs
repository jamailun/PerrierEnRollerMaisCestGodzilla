using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Hitbox : MonoBehaviour {

	private Collider2D _collider;

	public float CurrentDamages { private set; get; }

	private void Awake() {
		_collider = GetComponent<Collider2D>();
		_collider.enabled = false;
	}

	public void Spawn(float damages, float duration) {
		if(_collider.enabled)
			return; // Already attacking : do nothing.

		// Enable the collider.
		CurrentDamages = damages;
		_collider.enabled = true;

		// Disable after time.
		Utils.DoAfter(duration, () => {
			_collider.enabled = false;
		});
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		Debug.Log(name + " HITBOX collides with " + collision.gameObject.name);
	}

}