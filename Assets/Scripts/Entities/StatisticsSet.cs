using System.Collections.Generic;

public class StatisticsSet {

	private readonly Dictionary<Statistic, float> flats = new();
	private readonly Dictionary<Statistic, float> mutls = new();

	private readonly Dictionary<Statistic, float> temp_flats = new();
	private readonly Dictionary<Statistic, float> temp_mutls = new();

	private struct Buff {
		public StatisticModifier m;
		public float ends;
	}

	public StatisticsSet() {
		Reset();
	}

	private void Reset() {
		foreach(Statistic s in System.Enum.GetValues(typeof(Statistic))) {
			flats[s] = 0;
			mutls[s] = 1;
		}
	}

	public void ResetFrom(SkillsSet set) {
		// Reset maps
		Reset();
		// Enter all skills values
		foreach(var entry in set.GetPassiveSkills()) {
			RegisterPassiveSkill(entry.Key, entry.Value);
		}
	}

	public void TryRegisterSkill(Skill skill) {
		if(!skill.IsActive())
			RegisterPassiveSkill((PassiveSkill) skill);
	}

	public void RegisterPassiveSkill(PassiveSkill skill, int level = 1) {
		if(skill.IsMultiplicative) {
			mutls[skill.Statistic] += (level * skill.MultiplicatorPerLevel);
		} else {
			flats[skill.Statistic] += (level * skill.AdditionPerLevel);
		}
	}

	public float GetPower(Statistic type, float baseValue) {
		return (baseValue + flats[type]) * mutls[type];
	}

	public void AddTemporaryStats(StatisticModifier modifier) {
		if(modifier.IsMultiplicative()) {
			temp_mutls[modifier.statistic] += modifier.modifier;
		} else {
			temp_flats[modifier.statistic] += modifier.modifier;
		}
	}

	public void RemoveTemporaryStats(StatisticModifier modifier) {
		if(modifier.IsMultiplicative()) {
			temp_mutls[modifier.statistic] -= modifier.modifier;
		} else {
			temp_flats[modifier.statistic] -= modifier.modifier;
		}
	}

}