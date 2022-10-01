using UnityEngine;
using UnityEngine.UI;

public class ActiveSkillButton : MonoBehaviour {

	private readonly Color ACTIVE = new(0.4842921f, 0.5566038f, 0.3649431f);
	private readonly Color INACTIVE = new(0.3018868f, 0.3018868f, 0.3018868f);

	[SerializeField] private KeyCode keyPress;
	[SerializeField] private ActiveSkill skillType;

	[Header("Configuration de l'image")]

	[SerializeField] private Image skillBackground;
	[SerializeField] private Image skillImage;
	[SerializeField] private TMPro.TMP_Text stackLabel;
	[SerializeField] private Image loadingStackImage;

	[Header("Configuration du petit bouton")]

	[SerializeField] private TMPro.TMP_Text keyLabel;
	[SerializeField] private Animation keyAnimation;

	private readonly ActiveSkillInstance skill = new();

	private void Start() {
		SetSkill(skillType);

		keyLabel.text = "" + keyPress;
	}

	private void Update() {
		if(skillType == null)
			return;


		if(Input.GetKeyDown(keyPress)) {
			if(skill.CanActivate()) {
				skill.Activate();
				keyAnimation.Play();
				stackLabel.text = "" + skill.Stacks;
				//TODO cast the skill.
			}
		}

		if(skill.Update()) {
			stackLabel.text = "" + skill.Stacks;
		}
		if(skill.Stacks == 0) {
			skillBackground.color = INACTIVE;
		} else {
			skillBackground.color = ACTIVE;
		}

		loadingStackImage.fillAmount = skill.NextStackPercentage();
	}

	public ActiveSkill GetSkill() {
		return skillType;
	}

	public void SetSkill(ActiveSkill skillType) {
		this.skillType = skillType;

		skill.SetOrUpgradeSkill(skillType);

		stackLabel.text = "" + skill.Stacks;
	}

}