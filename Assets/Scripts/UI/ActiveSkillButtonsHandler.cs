using UnityEngine;

public class ActiveSkillButtonsHandler : MonoBehaviour {

	private ActiveSkillButton[] _buttons;

	private void Start() {
		_buttons = GetComponentsInChildren<ActiveSkillButton>();
		// Hide buttons at start
		foreach(var b in _buttons)
			b.gameObject.SetActive(false);
	}

	public void Add(ActiveSkill skill, PlayerEntity target) {
		for(int i = 0; i < _buttons.Length; i++) {
			var button = _buttons[i];
			if( ! button.gameObject.activeSelf || button.GetSkill() == null || button.GetSkill() == skill) {
				button.gameObject.SetActive(true);
				button.SetSkill(skill, target);
				return;
			}
		}
	}

}