using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {

	[SerializeField] private bool damagePlayer = true;
	[SerializeField] private bool damageEnemies = false;

	[Tooltip("The damages of the projectile")]
	[SerializeField] private float damages = 10f;

	[Tooltip("The duration of the projectile (in seconds)")]
	[SerializeField] private float lifeDuration = 5f;

	[Tooltip("The speed of the projectile")]
	[SerializeField] private float speed = 100f;

	// Called by the code.
	public void Init(Transform parent, Vector2 direction) {
		// Set position and rotation
		transform.SetPositionAndRotation(parent.position, parent.rotation);

		// set direction
		GetComponent<Rigidbody2D>().AddForce(direction.normalized * speed);
	}

	private void Start() {
		// Destroy after
		StartCoroutine(Utils.DestroyAfter(gameObject, lifeDuration));
		// set the rotation
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		// Try do apply damages.
		var living = collision.gameObject.GetComponent<LivingEntity>();
		if(living != null) {
			// Pour le moment je laisse 2 cas différents, mais si ya besoin de rien changer autant faire 1 seul 'if'.
			if(damagePlayer && living.IsPlayer())
				living.Damage(damages);
			else if(damageEnemies && !living.IsPlayer())
				living.Damage(damages);
		}

		// Destroy the gameObject
		Destroy(gameObject);
	}

}
