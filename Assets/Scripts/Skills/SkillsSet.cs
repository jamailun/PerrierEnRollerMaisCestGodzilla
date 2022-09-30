using System.Collections.Generic;
using System.Linq;

public class SkillsSet {

	private readonly Dictionary<Skill, int> skills = new();

	private readonly int maxSkills;

	public SkillsSet(int maxSkills) {
		this.maxSkills = maxSkills;
	}

	public void AddSkill(Skill skill) {
		if(skills.ContainsKey(skill)) {
			skills[skill] += 1;
		} else {
			skills[skill] = 1;
		}
	}

	public Dictionary<Skill, int> GetSkills() {
		return skills;
	}

	public Dictionary<PassiveSkill, int> GetPassiveSkills() {
		Dictionary<PassiveSkill, int> passives = new();
		foreach(var en in skills) {
			if( ! en.Key.IsActive()) {
				passives.Add((PassiveSkill) en.Key, en.Value);
			}
		}
		return passives;
	}

	public List<Skill> GetPossiblesSkills() {
		// si on a rempli les passifs, on peut que avoir les notres
		if(skills.Count >= maxSkills) {
			// Plus de nouveau skill dispo
			return new List<Skill>(
				// putain quel langage de merde, juste un simple filter+map ...
				new List<KeyValuePair<Skill, int>>(skills.AsEnumerable()).FindAll(e => e.Value < e.Key.LevelMax).Select(e => e.Key)
			);
		}

		// il reste des skills dispos. donc on prend tous les skills et on retire ceux qu'on a au niveau max
		return SkillLibrairy.GetSkills().FindAll(p => {
			if(skills.ContainsKey(p)) {
				return skills[p] < p.LevelMax;
			} else {
				return true;
			}
		});
	}

	public List<Skill> PickSkills(int amount) {
		List<Skill> allowed = GetPossiblesSkills();

		allowed.Shuffle();

		return allowed.GetRange(0, amount);
	}

}