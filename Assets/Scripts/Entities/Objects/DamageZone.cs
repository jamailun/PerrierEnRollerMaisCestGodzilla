using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Zone that apply change to the health of entities in it.
/// </summary>
public class DamageZone : MonoBehaviour {

	[Tooltip("The cooldown between damages")]
	[SerializeField] private float cooldown = 0.05f;

	[Tooltip("The delta to apply to the health at each iteration")]
	[SerializeField] public float HealthDelta = -1f;

	// next time the damage delta will be applied
	private float next;

	private HashSet<LivingEntity> contained = new();

	private void Start() {
		next = Time.time + cooldown;
	}

	private void Update() {
		if(Time.time >= next) {
			next = Time.time + cooldown;

			try {
				foreach(var living in contained) {
					if(HealthDelta >= 0)
						living.Heal(HealthDelta);
					else
						living.Damage(-HealthDelta);
				}
			} catch(System.InvalidOperationException) {
				// don't care
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		var living = collision.GetComponent<LivingEntity>();
		if(living != null) {
			contained.Add(living);
		}
	}

	private void OnTriggerExit2D(Collider2D collision) {
		var living = collision.GetComponent<LivingEntity>();
		if(living != null) {
			contained.Remove(living);
		}
	}

}