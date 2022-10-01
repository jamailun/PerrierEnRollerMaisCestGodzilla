using System.Collections.Generic;
using UnityEngine;

public class SkillLibrairy : MonoBehaviour {

	private static SkillLibrairy Instance;

	[SerializeField] PassiveSkill[] passiveSkills;

	[SerializeField] ActiveSkill[] activeSkills;

	private void Start() {
		if(Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public static List<PassiveSkill> GetPassiveSkills() {
		return new List<PassiveSkill>(Instance.passiveSkills);
	}
	public static List<ActiveSkill> GetActivesSkills() {
		return new List<ActiveSkill>(Instance.activeSkills);
	}
	public static List<Skill> GetSkills() {
		var list = new List<Skill>(Instance.activeSkills.Length + Instance.passiveSkills.Length);
		list.AddRange(Instance.activeSkills);
		list.AddRange(Instance.passiveSkills);
		return list;
	}


}