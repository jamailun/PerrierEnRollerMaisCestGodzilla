using UnityEngine;

public class AoeProjectile : Projectile {

	private Rigidbody2D _rb;

	public override void Init(Vector3 sourcePosition, Vector2 direction, Transform realParent) {
		base.Init(sourcePosition, direction, realParent);
		_rb = GetComponent<Rigidbody2D>();
		_rb.drag = 0.2f;
	}

	private void Update() {
		if(_rb.velocity.magnitude < 0.1f) {
			Debug.LogWarning("TROP RALENTI!!!");
			Destroy(gameObject);
		}
	}


	protected override void OnTrigger(Hurtbox box) {
		if((damagePlayer && box.IsPlayer()) || (damageEnemies && box.IsEnemy()) || box.IsBuilding()) {
			box.Damage(this);
			Destroy(gameObject);
		}
	}

	protected override void Collides(Building building) {
		if(building) {
			building.Damage(damages);
		}
		Destroy(gameObject);
	}

}
