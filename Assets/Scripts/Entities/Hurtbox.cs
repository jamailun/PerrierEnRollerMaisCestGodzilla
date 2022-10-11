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
	public bool IsEnemy() {
		return _entity.GetEntityType() == EntityType.Enemy;
	}
	public bool IsBuilding() {
		return _entity.GetEntityType() == EntityType.Building;
	}

	public void Damage(Projectile proj) {
		_entity.Damage(proj.damages);
	}

	public float Damage(Hitbox hitbox) {
		float damages = hitbox.CurrentDamages;

		if(_entity.GetEntityType() == EntityType.Enemy)
			damages += hitbox.BonusDamagesEnemies;
		if(_entity.GetEntityType() == EntityType.Building)
			damages += hitbox.BonusDamagesBuilding;

		return _entity.Damage(damages);
	}

}