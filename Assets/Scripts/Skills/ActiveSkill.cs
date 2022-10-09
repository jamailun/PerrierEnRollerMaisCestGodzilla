using UnityEngine;

[CreateAssetMenu(fileName = "ActiveSkill", menuName = "PERMCG/Active Skill", order = 0)]
public class ActiveSkill : Skill {
	public override SkillType SkillType { get { return SkillType.Active; } }

	[SerializeField] private int maxStacks = 1;
	public int MaxStacks => maxStacks;

	[SerializeField] private float cooldown = 10f;
	public float Cooldown => cooldown;

	[SerializeField] private float activeDuration = 2f;
	public float ActiveDuration => activeDuration;

	[Header("Effects")] // ---------------------------------------------------------------

	[SerializeField] private float percentageHeal = 0f;

	[SerializeField] private StatisticModifier[] modifiers;

	[SerializeField] private ActiveSkillProduction[] spawnPrefabs;

	[Header("VFX / SFX")]
	[SerializeField] private ParticleSystem vfx;
	[SerializeField] private AudioClip sfx;

	public void Cast(PlayerEntity player, int level) {
		// Heal
		if(percentageHeal > 0)
			player.Heal(percentageHeal * player.MaxHealth);

		// Stats
		if(modifiers != null && modifiers.Length > 0) {
			foreach(var modifier in modifiers) {
				player.Buff(modifier, activeDuration * level);
			}
		}

		// Spawn
		if(spawnPrefabs != null && spawnPrefabs.Length > 0) {
			foreach(var prefab in spawnPrefabs) {
				var obj = Instantiate(prefab, player.GetOutputPosition(), Quaternion.identity);
				obj.Init(player, level);
			}
		}

		// VFX & SFX
		if(vfx != null) {
			var vfxInstance = Instantiate(vfx, player.transform.position, Quaternion.identity);
			vfxInstance.Play();
			Destroy(vfxInstance.gameObject, vfx.main.duration);
		}
		if(sfx != null) {
			var obj = new GameObject("sfx_skill_" + name);
			obj.transform.position = player.transform.position;
			var audio = obj.AddComponent<AudioSource>();
			audio.PlayOneShot(sfx);
			Destroy(obj, sfx.length);
		}

	}

}