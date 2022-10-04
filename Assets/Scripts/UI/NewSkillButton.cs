using UnityEngine;
using UnityEngine.UI;

public class NewSkillButton : MonoBehaviour {

	[SerializeField] private Image skillSprite;
	[SerializeField] private TMPro.TMP_Text spriteLabel;
	[SerializeField] private Image border;
	private NewSkillScreen owner;

	private Skill currentSkill;

	private void Start() {
		owner = GetComponentInParent<NewSkillScreen>();
		if(owner == null) {
			Debug.LogError("Could not find NewSkillScreen parent for NewSkillButton in " + gameObject.name);
			enabled = false;
		}
	}

	public void SetSkill(Skill skill) {
		this.currentSkill = skill;
		// update graphics
		if(skill == null) {
			skillSprite.sprite = null;
			skillSprite.color = Color.gray;
			border.color = Color.gray;
			spriteLabel.text = "";
		} else {
			skillSprite.sprite = skill.Sprite;
			skillSprite.color = Color.white;
			border.color = Color.yellow;
			spriteLabel.text = skill.Name;
		}
	}

	public void SkillClicked() {
		if(currentSkill == null)
			return;
		owner.SkillChose(currentSkill);
	}

}