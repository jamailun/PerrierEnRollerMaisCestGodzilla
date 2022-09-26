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

	public void Damage(Hitbox hitbox) {
		Debug.Log("DAMAGE FROM " + hitbox + " : " + hitbox.CurrentDamages);
		_entity.Damage(hitbox.CurrentDamages);
	}

}