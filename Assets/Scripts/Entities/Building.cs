using UnityEngine;

public class Building : LivingEntity {

	[Header("Building attributes")]
	[Tooltip("The sprites for the building.")]
	[SerializeField] private Sprite[] sprites;

	[Tooltip("The reward for breaking the building")]
	[SerializeField] private int rewardPoints = 0;

	[Tooltip("Optional particle system")]
	[SerializeField] private ParticleSystem vfx;
	[Tooltip("Describe the amoutn of emitted particles over damage ratio.")]
	[SerializeField] private AnimationCurve vfxOverDamage;

	private SpriteRenderer _spriteRenderer;

	protected override void Start() {
		base.Start(); // still shitty.

		_spriteRenderer = GetComponent<SpriteRenderer>();
		if(_spriteRenderer == null)
			_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		if(_spriteRenderer == null) {
			Debug.LogError("Could not find sprite renderer of building " + name);
			enabled = false;
			return;
		}

		if(sprites == null || sprites.Length == 0) {
			Debug.LogError("No sprite for building " + name);
			enabled = false;
			return;
		}

		HealthChanged();
	}


	protected override void HealthChanged() {
		float ratio = Health / MaxHealth;

		// Change the sprite index
		int index = Mathf.Max(0, Mathf.Min(sprites.Length-1, sprites.Length - Mathf.FloorToInt(ratio * sprites.Length)));
		Debug.Log("ratio=" + ratio+", => index="+index+", vfx="+ vfxOverDamage.Evaluate(1f - ratio));
		_spriteRenderer.sprite = sprites[index];

		// Change the VFX particle output
		if(vfx != null) {
			if(ratio < 1 && ratio > 0) {
				if(!vfx.isPlaying)
					vfx.Play();
				var emission = vfx.emission;
				emission.rateOverTimeMultiplier = vfxOverDamage.Evaluate(1f - ratio);
			} else {
				if(!vfx.isStopped)
					vfx.Stop();
			}
		}
	}

	protected override void Die() {
		Debug.LogWarning("allo?");
		base.Die();
		//TODO drop points on the ground.
		Debug.LogWarning("building " + name + " dead. dropped" + rewardPoints);
	}

}