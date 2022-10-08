using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour {

	protected static Vector3 PLAN = new(){ x = 1, y = 1, z = 0 };

	[SerializeField] protected bool damagePlayer = true;
	[SerializeField] protected bool damageEnemies = false;

	[Tooltip("The damages of the projectile")]
	[SerializeField] public float damages = 10f;

	[Tooltip("The duration of the projectile (in seconds)")]
	[SerializeField] protected float lifeDuration = 5f;

	[Tooltip("The speed of the projectile")]
	[SerializeField] protected float speed = 100f;

	protected Transform parent;

	// Called by the code.
	public virtual void Init(Vector3 sourcePosition, Vector2 direction, Transform realParent) {
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

	protected virtual void OnTrigger(Hurtbox box) {
		if((damagePlayer && box.IsPlayer()) || (damageEnemies && box.IsEnemy()) || box.IsBuilding()) {
			box.Damage(this);
			Destroy(gameObject);
		}
	}

	protected virtual void Collides(Building building) {
		if(building) {
			building.Damage(damages);
		}
		Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		var box = collision.GetComponent<Hurtbox>();
		if(box != null)
			OnTrigger(box);
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if(collision == null || collision.transform == parent || collision.transform.IsChildOf(parent))
			return;
		var building = collision.gameObject.GetComponent<Building>();
		Collides(building);
	}

}
