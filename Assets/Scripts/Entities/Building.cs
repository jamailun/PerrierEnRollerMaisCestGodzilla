using UnityEngine;

public class Building : LivingEntity {

	[Header("Building attributes")]
	[Tooltip("The sprites for the building.")]
	[SerializeField] private Sprite[] sprites;

	[Tooltip("The reward prefab for breaking the building")]
	[SerializeField] private RewardPoints rewardPointPrefab;
	[Tooltip("The reward for breaking the building")]
	[SerializeField] private uint rewardPoints = 0;

	[Tooltip("Optional particle system")]
	[SerializeField] private ParticleSystem vfx;
	[Tooltip("Describe the amoutn of emitted particles over damage ratio.")]
	[SerializeField] private AnimationCurve vfxOverDamage;

	private SpriteRenderer _spriteRenderer;

	private void Start() {
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

		HealthChanged(0);
	}

	public override EntityType GetEntityType() {
		return EntityType.Building;
	}

	protected override void HealthChanged(float delta) {
		float ratio = Health / MaxHealth;

		// Change the sprite index
		int index = Mathf.Max(0, Mathf.Min(sprites.Length-1, sprites.Length - Mathf.FloorToInt(ratio * sprites.Length)));
		//Debug.Log("ratio=" + ratio+", => index="+index+", vfx="+ vfxOverDamage.Evaluate(1f - ratio));
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
		// Drop item before delete the gameobject
		int c = 1;
		uint amount = rewardPoints / 10;
		while(amount >= 5) {
			amount /= 5;
			c += 1;
		}
		c = Mathf.Max(1, c + Random.Range(-1, 1));

		uint perRewardPoints = rewardPoints / (uint) c;

		for(int i = 0; i < c; i++) {
			Vector3 pos = Random.insideUnitCircle * 1.2f;
			var reward = Instantiate(rewardPointPrefab);
			reward.transform.position = transform.position + pos;
			reward.rewardAmount = perRewardPoints;
		}

		base.Die();
	}

}