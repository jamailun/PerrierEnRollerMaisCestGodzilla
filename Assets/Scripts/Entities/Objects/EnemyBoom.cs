using UnityEngine;

public class EnemyBoom : Activable {

	[SerializeField] private float damageMult = 1f;
	[SerializeField] private AudioClip clip;
	[SerializeField] private ParticleSystem vfx;
	[SerializeField] private float radius = 5f;

	private float baseDamage = 1f;

	public override void Activate(float damages = 0, float radiusBonus = 0) {
		this.baseDamage = damageMult;
		radius += radiusBonus;

		if(clip) {
			var obj = new GameObject("sfx_enemyboom_" + name);
			obj.transform.position = transform.position;
			var audio = obj.AddComponent<AudioSource>();
			audio.PlayOneShot(clip);
			Destroy(obj, clip.length);
		}

		if(vfx)
			vfx.Play();

		// damage
		foreach(var obj in Physics2D.OverlapCircleAll(transform.position, radius)) {
			PlayerEntity pl = obj.GetComponent<PlayerEntity>();
			if(pl != null && pl.enabled && !pl.IsDead())
				pl.Damage(damageMult * baseDamage);
		}
		// destroy
		Destroy(gameObject, .1f);
	}

}