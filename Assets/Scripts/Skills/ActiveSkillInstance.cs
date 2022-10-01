using UnityEngine;

public class ActiveSkillInstance {

	private ActiveSkill skill;

	public int Level { get; private set; }
	public int Stacks { get; private set; }
	public float CurrentStackReload { get; private set; }

	private float nextActivated = 0f;

	public bool Update() {
		if(Stacks == skill.MaxStacks)
			return false;
		CurrentStackReload += Time.deltaTime;
		if(CurrentStackReload >= skill.Cooldown) {
			CurrentStackReload -= skill.Cooldown;
			Stacks++;
			return true;
		}
		return false;
	}

	public float NextStackPercentage() {
		if(Stacks == skill.MaxStacks)
			return 0f;
		return CurrentStackReload / skill.Cooldown;
	}

	public bool CanActivate() {
		return Time.time >= nextActivated && Stacks > 0;
	}

	public void Activate() {
		Stacks--;
		nextActivated = Time.time + skill.ActiveDuration;
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
		CurrentStackReload = 0f;
		nextActivated = Time.time;
	}

}