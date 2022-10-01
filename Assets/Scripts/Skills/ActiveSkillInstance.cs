using UnityEngine;

public class ActiveSkillInstance : MonoBehaviour {

	[SerializeField] private ActiveSkill skill;

	public int Level { get; private set; }
	public int Stacks { get; private set; }
	public float CurrentStackReload { get; private set; }

	private bool activated;

	private void Start() {
		Level = 0;
		SetOrUpgradeSkill(skill);
	}

	private void Update() {
		if(Stacks == skill.MaxStacks)
			return;
		CurrentStackReload += Time.deltaTime;
		if(CurrentStackReload >= skill.Cooldown) {
			CurrentStackReload -= skill.Cooldown;
			Stacks++;
		}
	}

	public bool CanActivate() {
		return !activated && Stacks > 0;
	}

	public void Activate() {
		Stacks--;
		activated = true;
		StartCoroutine(Utils.DoAfter(skill.ActiveDuration, () => activated = false));

		//TODO call the skill itself.
	}

	public void SetOrUpgradeSkill(ActiveSkill skill) {
		if(this.skill == skill) {
			Level++;
			return;
		} else {
			Level = 1;
		}
		this.skill = skill;	
		Stacks = 0;
		CurrentStackReload = 0;
		activated = false;
	}

}