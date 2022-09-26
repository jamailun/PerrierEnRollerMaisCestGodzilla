using UnityEngine;

public class Hurtbox : MonoBehaviour {

	private LivingEntity _entity;

	private void Start() {
		_entity = GetComponentInParent<LivingEntity>();
		if(_entity == null) {
			Debug.LogError("Hurtbox " + name + " could NOT get the parent entity.");
			enabled = false;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		Debug.Log(name + " HURTBOX collides with " + collision.gameObject.name);
	}

}