using UnityEngine;

public class Intimidation : ActiveSkillProduction {

	[SerializeField] private float baseDuration = 2f;
	[SerializeField] private float baseRadius = 2f;

	private float radius = 1f;
	private float intimidateDuration = 1f;

	private void Start() {
		// get all objects
		foreach(var obj in Physics2D.OverlapCircleAll(transform.position, radius)) {
			Enemy enemy = obj.GetComponent<Enemy>();
			if(enemy != null && enemy.enabled)
				enemy.MakeAfraid(intimidateDuration);
		}
		// Destroy self after that
		Destroy(gameObject, .1f);
	}

	public override void Init(PlayerEntity player, int level) {
		intimidateDuration = baseDuration * level;
		radius = baseRadius * Mathf.Min(2f, player.GetCurrentRange());
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(this.transform.position, radius);
	}

}