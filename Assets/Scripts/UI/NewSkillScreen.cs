using UnityEngine;
using System.Collections.Generic;

public class NewSkillScreen : MonoBehaviour {

	private NewSkillButton[] buttons;

	public delegate void SkillCallback(Skill skill);

	private SkillCallback callback;

	private void Start() {
		if(buttons == null)
			Init();
	}

	private void Init() {
		buttons = GetComponentsInChildren<NewSkillButton>();
		if(buttons == null || buttons.Length == 0)
			Debug.LogError("Warning ! no NewSkillButton for NewSkillScreen.");
	}

	public void SkillChose(Skill skill) {
		Debug.Log("chose skill " + skill);

		// call the callback at the end.
		callback?.Invoke(skill);
	}

	public void FindSkills(SkillsSet skillsSet, SkillCallback callback) {
		if(buttons == null)
			Init();

		this.callback = callback;

		List<Skill> skills = skillsSet.PickSkills(buttons.Length);
		if(skills.Count == 0) {
			callback?.Invoke(null);
			return;
		}

		// remplir les skills
		int i = 0;
		foreach(var s in skills) {
			buttons[i].SetSkill(s);
			i++;
		}
		// remplir les trous
		for(; i < buttons.Length; i++) {
			buttons[i].SetSkill(null);
		}

	}
	
}