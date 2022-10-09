using UnityEngine;

public class Explosion : ActiveSkillProduction {

	private float damages = 2f;
	[SerializeField] private float baseRadius = 2f;

	private float radius;

	private void Start() {
		// get all objects
		foreach(var obj in Physics2D.OverlapCircleAll(transform.position, radius)) {
			LivingEntity entity = obj.GetComponent<LivingEntity>();
			if(entity != null && entity.enabled && !entity.IsPlayer())
				entity.Damage(damages);
		}
		// Destroy self after that
		Destroy(gameObject, .1f);
	}

	public override void Init(PlayerEntity player, int level) {
		damages = player.GetCurrentDamages() * 2f * level;
		radius = baseRadius * player.GetCurrentRange();
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(this.transform.position, radius);
	}

}