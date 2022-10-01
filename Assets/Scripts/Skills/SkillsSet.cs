using System.Collections.Generic;
using System.Linq;

public class SkillsSet {

	private readonly Dictionary<PassiveSkill, int> passiveSkills = new();
	private readonly Dictionary<ActiveSkill, int> activesSkills = new();

	private readonly int maxSkillPassives;
	private readonly int maxSkillActives;

	public SkillsSet(int maxSkillPassives, int maxSkillActives) {
		this.maxSkillPassives = maxSkillPassives;
		this.maxSkillActives = maxSkillActives;
	}

	public void AddSkill(Skill skill) {
		if(skill.IsActive()) {
			var s = (ActiveSkill) skill;
			if(activesSkills.ContainsKey(s)) {
				activesSkills[s] += 1;
			} else {
				activesSkills[s] = 1;
			}
		} else {
			var s = (PassiveSkill) skill;
			if(passiveSkills.ContainsKey(s)) {
				passiveSkills[s] += 1;
			} else {
				passiveSkills[s] = 1;
			}
		}
	}

	public Dictionary<Skill, int> GetSkills() {
		var map = new Dictionary<Skill, int>();
		foreach(var en in passiveSkills)
			map.Add(en.Key, en.Value);
		foreach(var en in activesSkills)
			map.Add(en.Key, en.Value);
		return map;
	}

	public Dictionary<PassiveSkill, int> GetPassiveSkills() {
		return passiveSkills;
	}
	public Dictionary<ActiveSkill, int> GetActiveSkills() {
		return activesSkills;
	}

	//TODO CECI EST DE LA GROSSE MERDE, IL FAUT QUE JE EFLECHISSE  COMMENT LE RENDRE MIEUX

	private List<PassiveSkill> GetPossiblesPassivesSkills() {
		// si on a rempli les passifs, on peut que avoir les notres
		if(passiveSkills.Count >= maxSkillPassives) {
			// Plus de nouveau skill dispo
			return new List<PassiveSkill>(
				// putain quel langage de merde, juste un simple filter+map ...
				new List<KeyValuePair<PassiveSkill, int>>(passiveSkills.AsEnumerable()).FindAll(e => e.Value < e.Key.LevelMax).Select(e => e.Key)
			);
		}

		// il reste des skills dispos. donc on prend tous les skills et on retire ceux qu'on a au niveau max
		return SkillLibrairy.GetPassiveSkills().FindAll(p => {
			if(passiveSkills.ContainsKey(p)) {
				return passiveSkills[p] < p.LevelMax;
			} else {
				return true;
			}
		});
	}
	private List<ActiveSkill> GetPossiblesActivesSkills() {
		// si on a rempli les passifs, on peut que avoir les notres
		if(activesSkills.Count >= maxSkillActives) {
			// Plus de nouveau skill dispo
			return new List<ActiveSkill>(
				// putain quel langage de merde, juste un simple filter+map ...
				new List<KeyValuePair<ActiveSkill, int>>(activesSkills.AsEnumerable()).FindAll(e => e.Value < e.Key.LevelMax).Select(e => e.Key)
			);
		}

		// il reste des skills dispos. donc on prend tous les skills et on retire ceux qu'on a au niveau max
		return SkillLibrairy.GetActivesSkills().FindAll(p => {
			if(activesSkills.ContainsKey(p)) {
				return activesSkills[p] < p.LevelMax;
			} else {
				return true;
			}
		});
	}

	public List<Skill> PickSkills(int amount) {
		List<Skill> allowed = new();
		allowed.AddRange(GetPossiblesPassivesSkills());
		allowed.AddRange(GetPossiblesActivesSkills());

		allowed.Shuffle();

		return allowed.GetRange(0, amount);
	}

}