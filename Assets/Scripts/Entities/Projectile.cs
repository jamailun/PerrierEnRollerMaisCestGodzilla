using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {

	private static Vector3 PLAN = new(){ x = 1, y = 1, z = 0 };

	[SerializeField] private bool damagePlayer = true;
	[SerializeField] private bool damageEnemies = false;

	[Tooltip("The damages of the projectile")]
	[SerializeField] public float damages = 10f;

	[Tooltip("The duration of the projectile (in seconds)")]
	[SerializeField] private float lifeDuration = 5f;

	[Tooltip("The speed of the projectile")]
	[SerializeField] private float speed = 100f;

	private Transform parent;

	// Called by the code.
	public void Init(Transform parent, Vector2 direction, Transform realParent = null) {
		this.parent = realParent ?? parent;
		Debug.Log("Parent = " + this.parent + "("+ this.parent.gameObject.name+"), " + this.parent.GetInstanceID());

		// Set position and rotation
		transform.SetPositionAndRotation(parent.position, parent.rotation);

		Vector3 dir = direction * PLAN;

		// set direction
		GetComponent<Rigidbody2D>().AddForce(dir.normalized * speed);
	}

	private void Start() {
		// Destroy after
		StartCoroutine(Utils.DestroyAfter(gameObject, lifeDuration));
	}

	/*private void Collides(GameObject other) {
		Debug.Log("OTHER = " + other + "(" + other.gameObject.name + "), " + other.GetInstanceID());
		if(other.transform == parent || other.transform.IsChildOf(parent))
			return;

		var living = other.GetComponent<LivingEntity>();
		if(living != null) {
			// Pour le moment je laisse 2 cas différents, mais si ya besoin de rien changer autant faire 1 seul 'if'.
			if(damagePlayer && living.IsPlayer()) {
				Debug.Log("attack player " + other.name);
				living.Damage(damages);
				Destroy(gameObject);
			} else if(damageEnemies && !living.IsPlayer()) {
				Debug.Log("attack enemy " + other.name);
				living.Damage(damages);
				Destroy(gameObject);
			}
		} else {
			// mur
			Destroy(gameObject);
			Debug.Log("collides with wall " + other.name);
		}
	}*/

	private void OnTriggerEnter2D(Collider2D collision) {
		var box = collision.GetComponent<Hurtbox>();
		if(box == null)
			return;
		if(damagePlayer && box.IsPlayer()) {
			Debug.Log("attack player " + box.name);
			box.Damage(this);
			Destroy(gameObject);
		} else if(damageEnemies && !box.IsPlayer()) {
			Debug.Log("attack enemy " + box.name);
			box.Damage(this);
			Destroy(gameObject);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if(collision.transform == parent || collision.transform.IsChildOf(parent))
			return;
		Destroy(gameObject);
	}

}
