using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLibrairy : MonoBehaviour {

	private static SkillLibrairy Instance;

	[SerializeField] PassiveSkill[] passiveSkills;

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
	public static List<Skill> GetSkills() {
		return new List<Skill>(Instance.passiveSkills);
	}


}