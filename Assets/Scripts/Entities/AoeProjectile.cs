using UnityEngine;

public class AoeProjectile : Projectile {

	[SerializeField] private AudioClip breakSoundSfx;
	[SerializeField] private AutoDestroy aoePrefab;

	public override void Init(Vector3 sourcePosition, Vector2 direction, Transform realParent) {
		base.Init(sourcePosition, direction, realParent);
	}

	protected override void OnStart() {
		StartCoroutine(Utils.DoAfter(lifeDuration, () => Break()));
	}

	private void Break() {
		var aoe = Instantiate(aoePrefab);
		aoe.transform.SetLocalPositionAndRotation(transform.position, Quaternion.identity);

		if(breakSoundSfx != null) {
			var obj = new GameObject("soundeffect_break");
			var asnd = obj.AddComponent<AudioSource>();
			asnd.clip = breakSoundSfx;
			obj.AddComponent<AutoDestroy>().lifeTime = aoe.lifeTime;
			asnd.Play();
		}

		Destroy(gameObject);
	}


	protected override void OnTrigger(Hurtbox box) {
		if((damagePlayer && box.IsPlayer()) || (damageEnemies && box.IsEnemy()) || box.IsBuilding()) {
			box.Damage(this);
			Break();
		}
	}

	protected override void Collides(Building building) {
		if(building) {
			building.Damage(damages);
		}
		Break();
	}

}
