using UnityEngine;
using UnityEngine.UI;

public class ActiveSkillButton : MonoBehaviour {

	private readonly Color ACTIVE = new(0.4842921f, 0.5566038f, 0.3649431f);
	private readonly Color INACTIVE = new(0.3018868f, 0.3018868f, 0.3018868f);

	[SerializeField] private KeyCode keyPress;
	private ActiveSkill skillType;

	[Header("Configuration de l'image")]

	[SerializeField] private Image skillBackground;
	[SerializeField] private Image skillImage;
	[SerializeField] private TMPro.TMP_Text stackLabel;
	[SerializeField] private Image loadingStackImage;
	[SerializeField] private Image activeAnimation;

	[Header("Configuration du petit bouton")]

	[SerializeField] private TMPro.TMP_Text keyLabel;
	[SerializeField] private Animation keyAnimation;

	private PlayerEntity target;
	private readonly ActiveSkillInstance skill = new();

	private void Start() {
		keyLabel.text = "" + keyPress;
		activeAnimation.gameObject.SetActive(false);
	}

	private void Update() {
		if(skillType == null)
			return;


		if(Input.GetKeyDown(keyPress)) {
			if(skill.CanActivate()) {
				// Activate animations, stack decrement...
				skill.Activate();
				keyAnimation.Play();
				stackLabel.text = "" + skill.Stacks;

				// Animation of the flame
				activeAnimation.gameObject.SetActive(true);
				StartCoroutine(Utils.DoAfter(skillType.ActiveDuration, () => activeAnimation.gameObject.SetActive(false)));

				// Cast the skill.
				skillType.Cast(target);
			}
		}

		if(skill.Update()) {
			stackLabel.text = "" + skill.Stacks;
		}
/*		if(skill.Stacks == 0) { REMOVED BECAUSE OF THE FULL BACKGROUND
			skillBackground.color = INACTIVE;
		} else {
			skillBackground.color = ACTIVE;
		}*/

		loadingStackImage.fillAmount = skill.NextStackPercentage();
	}

	public ActiveSkill GetSkill() {
		return skillType;
	}

	public void SetSkill(ActiveSkill skillType, PlayerEntity target) {
		this.target = target;
		this.skillType = skillType;

		skill.SetOrUpgradeSkill(skillType);

		skillImage.sprite = skillType.Sprite;
		stackLabel.text = "" + skill.Stacks;
	}

}