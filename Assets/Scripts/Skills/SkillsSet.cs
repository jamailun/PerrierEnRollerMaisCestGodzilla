using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SkillsSet {

	private Dictionary<PassiveSkill, int> passives = new();

	private readonly int maxPassives;

	public SkillsSet(int maxPassives) {
		this.maxPassives = maxPassives;
	}

	public void AddSkill(Skill skill) {
		if(skill.IsActive()) {
			//TODO add active

		} else {
			// Add PASSIVE
			PassiveSkill passive = (PassiveSkill) skill;
			if(passives.ContainsKey(passive)) {
				passives[passive] += 1;
			} else {
				passives[passive] = 1;
			}
		}
		
	}

	public Dictionary<PassiveSkill, int> GetPassives() {
		return passives;
	}

	public List<PassiveSkill> GetPossiblesPassivesSkills() {
		// si on a rempli les passifs, on peut que avoir les notres
		if(passives.Count >= maxPassives) {
			// Plus de nouveau skill dispo
			return new List<PassiveSkill>(
				// putain quel langage de merde, juste un simple filter+map ...
				new List<KeyValuePair<PassiveSkill, int>>(passives.AsEnumerable()).FindAll(e => e.Value < e.Key.LevelMax).Select(e => e.Key)
			);
		}

		// il reste des skills dispos. donc on prend tous les skills et on retire ceux qu'on a au niveau max
		return SkillLibrairy.GetPassiveSkills().FindAll(p => {
			if(passives.ContainsKey(p)) {
				return passives[p] < p.LevelMax;
			} else {
				return true;
			}
		});
	}
	public List<Skill> GetPossiblesActivesSkills() {
		//TODO
		return new();
	}

	public List<Skill> PickSkills(int amount) {
		List<PassiveSkill> allowedPassives = GetPossiblesPassivesSkills();
		List<Skill> allowedActives = GetPossiblesActivesSkills();

		List<Skill> allowed = new();
		allowed.AddRange(allowedPassives);
		allowed.AddRange(allowedActives);

		allowed.Shuffle();

		return allowed.GetRange(0, amount);
	}

}