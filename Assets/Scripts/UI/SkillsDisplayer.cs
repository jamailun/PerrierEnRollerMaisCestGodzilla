using UnityEngine;
using System.Collections.Generic;

public class SkillsDisplayer : MonoBehaviour {

	[SerializeField] private SkillDisplay displayPrefab;
	[SerializeField] private RectTransform container;

	public void SetSkills(Dictionary<Skill, int> skills) {
		container.DestroyChildren();
		foreach(var entry in skills) {
			var display = Instantiate(displayPrefab, container);
			display.SetSkill(entry.Key, entry.Value);
		}
	}

}