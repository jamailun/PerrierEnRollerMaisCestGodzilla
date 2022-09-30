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

	public bool IsPlayer() {
		return _entity.IsPlayer();
	}

	public void Damage(Projectile proj) {
		Debug.Log("DAMAGE FROM " + proj + " : " + proj.damages);
		_entity.Damage(proj.damages);
	}

	public void Damage(Hitbox hitbox) {
		Debug.Log("DAMAGE FROM " + hitbox + " : " + hitbox.CurrentDamages);
		_entity.Damage(hitbox.CurrentDamages);
	}

}