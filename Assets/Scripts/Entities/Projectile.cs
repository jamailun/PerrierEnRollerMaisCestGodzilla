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
	public void Init(Vector3 sourcePosition, Vector2 direction, Transform realParent) {
		this.parent = realParent;
		//Debug.Log("Parent = " + this.parent + "("+ this.parent.gameObject.name+"), " + this.parent.GetInstanceID());

		// Set position and rotation
		transform.SetPositionAndRotation(sourcePosition, Quaternion.identity);

		Vector3 dir = direction * PLAN;

		// set direction
		GetComponent<Rigidbody2D>().AddForce(dir.normalized * speed);
	}

	private void Start() {
		// Destroy after
		StartCoroutine(Utils.DestroyAfter(gameObject, lifeDuration));
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		var box = collision.GetComponent<Hurtbox>();
		if(box == null)
			return;
		if((damagePlayer && box.IsPlayer()) || (damageEnemies && box.IsEnemy()) || box.IsBuilding()) {
			box.Damage(this);
			Destroy(gameObject);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if(collision.transform == parent || collision.transform.IsChildOf(parent))
			return;
		var building = collision.gameObject.GetComponent<Building>();
		if(building) {
			building.Damage(damages);
			Destroy(gameObject);
			return;
		}
		//Debug.Log("projectile entered with " + collision.gameObject.name);
		Destroy(gameObject);
	}

}
