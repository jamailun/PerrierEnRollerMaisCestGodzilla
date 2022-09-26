using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Hitbox : MonoBehaviour {

	private Collider2D _collider;

	public float CurrentDamages { private set; get; }

	private void Awake() {
		_collider = GetComponent<Collider2D>();
		_collider.enabled = false;
	}

	public void Spawn(float damages, float duration, bool swapX) {
		if(_collider.enabled)
			return; // Already attacking : do nothing.

		// Enable the collider.
		CurrentDamages = damages;
		_collider.enabled = true;

		// Flip X if left oriented
		if(swapX) {
			// flip renderer
			var renderer = GetComponent<SpriteRenderer>();
			if(renderer != null)
				renderer.flipX = true;
			// miror position
			var pos = transform.localPosition;
			pos.x = -pos.x;
			transform.localPosition = pos;
		}

		// Destroy after time
		StartCoroutine(Utils.DestroyAfter(gameObject, duration));
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		Debug.Log(name + " HITBOX hits " + collision.gameObject.name);
		var target = collision.GetComponent<Hurtbox>();
		if(target != null) {
			Debug.Log("DAMAGE done  : "+collision.gameObject.name);
			target.Damage(this);
		}
	}

}