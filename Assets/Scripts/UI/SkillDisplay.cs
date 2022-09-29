using UnityEngine;
using UnityEngine.UI;

public class SkillDisplay : MonoBehaviour {

	[SerializeField] private Image skillSprite;
	[SerializeField] private TMPro.TMP_Text levelLabel;

	public void SetSkill(Skill skill, int level) {
		if(skill == null) {
			skillSprite.sprite = null;
			skillSprite.color = Color.gray;
			levelLabel.text = "";
		} else {
			skillSprite.sprite = skill.Sprite;
			skillSprite.color = Color.white;
			levelLabel.text = ""+level;
		}
	}

}