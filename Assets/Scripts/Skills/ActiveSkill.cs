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

	[SerializeField] private bool changeStats = false;
	[SerializeField] private StatisticModifier[] modifiers;

	[SerializeField] private bool spawnObjects = false;
	[SerializeField] private GameObject[] spawnPrefabs;

	public void Cast(PlayerEntity player) {
		// Heal
		if(percentageHeal > 0)
			player.Heal(percentageHeal * player.MaxHealth);

		// Stats
		if(changeStats) {
			foreach(var modifier in modifiers) {
				player.Buff(modifier, activeDuration);
			}
		}

		// Spawn
		if(spawnObjects) {
			foreach(var prefab in spawnPrefabs) {
				var obj = Instantiate(prefab);
				obj.transform.position = player.GetOutputPosition();
			}
		}

	}

}