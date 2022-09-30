using UnityEngine;
using System.Collections;

public class PlayerEntity : LivingEntity {

    [Header("UI link")]

    [SerializeField] private ManagerUI UI;

    [Header("Player configuration.")]

    [Tooltip("The current monster form.")]
    [SerializeField] private PlayerForm currentForm;

    [Tooltip("UI Manager reference")]

    private bool attacking = false;
    private float nextAttack = 0;

    [Tooltip("The number modulo of the skill choice trigger.")]
    [SerializeField] private int skillEveryNLevels = 3;
    [Tooltip("The number modulo of the evolve trigger.")]
    [SerializeField] private int evolveEveryNLevels = 15;

    [Tooltip("The prefab for the death animation")]
    [SerializeField] private GameObject deathAnimation;
    [Tooltip("The duration of the death animation.")]
    [SerializeField] private float deathAnimationDuration;

    [SerializeField] private int maxNumberOfPassives = 3;

    private SkillsSet skills;

    public uint UpgradePoints { get; set; }
    public ulong ExperiencePoints { get; private set; }
    private int level = 1;
    private ulong nextLevel = 100;
    private ulong previousLevel = 0;

    private float startedTime;

    public CustomAnimator Animator { get; private set; }
    public PlayerForm PlayerForm => currentForm;

    private void Start() {
        Animator = GetComponentInChildren<CustomAnimator>();
        if(currentForm == null) {
            Debug.LogError("Error, no Animation for player.");
            enabled = false;
            return;
        }
        if(currentForm == null) {
            Debug.LogError("Error, no currentForm for player.");
            enabled = false;
            return;
        }

        UpdatePlayerForm();

        startedTime = Time.time;
        TimerUI.StartTimer();

        ExperiencePoints = 0;
        UI.ExperienceBar.Init(previousLevel, nextLevel, ExperiencePoints);
        UI.ExperienceLabel.text = "Lvl " + level;

        skills = new SkillsSet(maxNumberOfPassives);
    }

	public void TryAttack(Orientation orientation) {
        // check cooldown.
        if(Time.time < nextAttack)
                return;
        nextAttack = Time.time + currentForm.AttackShape.AttackCooldown;

        attacking = true;
        float attackDamage = currentForm.AttackShape.AttackDamage; //TODO ajouter les stats
        currentForm.AttackShape.SpawnHurtbox(orientation, transform, attackDamage, currentForm.AttackShape.AttackDuration);

        // reset the boolean after some time.
        StartCoroutine(Utils.DoAfter(currentForm.AttackShape.AttackDuration, () => attacking = false));
	}

    public override bool IsPlayer() {
        return true;
    }

    public float GetSpeed() {
        if(attacking && !currentForm.AttackShape.CanMoveOnAttack)
            return 0;

        // Can be used to do slow/run effects.
        return _speed;
	}

    public void AddExperience(ulong amount) {
        ExperiencePoints += amount;
        while(ExperiencePoints > nextLevel) {
            LevelUp();
		}
        UI.ExperienceBar.Init(previousLevel, nextLevel, ExperiencePoints);
	}

    private void LevelUp() {
        level++;
        previousLevel = nextLevel;
        nextLevel *= 2;

        if(level % skillEveryNLevels == 0) {
            // Skill...

            Time.timeScale = 0; //TODO faire un manager pour ce genre de connerie ?
            UI.NewSkillScreen.gameObject.SetActive(true);

            if(level % evolveEveryNLevels == 0) {
                // Skill + Evolve
                UI.NewSkillScreen.FindSkills(skills, LevelUp_ThenEvolve);
            } else {
                // Skill.
                UI.NewSkillScreen.FindSkills(skills, LevelUp_Over);
            }
        } else {
            // Elvove.
            if(level % evolveEveryNLevels == 0) {
                Time.timeScale = 0;
                UI.EvolveScreen.gameObject.SetActive(true);
                UI.EvolveScreen.DisplayForms(currentForm.Descendants, ChangePlayerForm);
            }
        }

        UI.ExperienceLabel.text = "Lvl " + level;

        Debug.Log("Level up ! nex level="+level);
        Heal(MaxHealth * 0.2f); // heal de 20% ??

        //TODO vfx & sfx
	}

    private void LevelUp_Over(Skill skill) {
        Time.timeScale = 1f;
        AddSkill(skill);
    }
    private void LevelUp_ThenEvolve(Skill skill) {
        // Check si on PEUT évoluer
        if(currentForm.Descendants.Count == 0) {
            LevelUp_Over(skill);
            return;
        }
        // Sinon

        AddSkill(skill);

        UI.EvolveScreen.gameObject.SetActive(true);
        UI.EvolveScreen.DisplayForms(currentForm.Descendants, ChangePlayerForm);
    }

    private void AddSkill(Skill skill) {
        // add the skill
        UI.NewSkillScreen.gameObject.SetActive(false);
        skills.AddSkill(skill);

        // Refresh global displayed list
        UI.SkillsDisplayer.SetSkills(skills.GetPassives());

        //TODO update stats !
    }

    protected override void Die() {
        // Save data
        PersistentData.EndRun(Time.time - startedTime, level, UpgradePoints);
        TimerUI.Stop();

        //TODO SFX
        if(deathAnimation != null) {
            var da = Instantiate(deathAnimation);
            da.transform.position = transform.position;
        }

        StartCoroutine(Utils.DoAfter(deathAnimationDuration, () => {
            // display the screen after X time.
            UI.DeathScreen.gameObject.SetActive(true);
        }));

        // hop on devient tout mort
        dead = true;
        // NON : base.Die();

        // PUTAIIINI trop chiant si on destroy cet objet, ben la couroutine plus haut ne se lance pas car c'est D ELA MERDE
        // donc du coup je désactive les autres scripts comme un GROS CONNARD puis je supprimer mes enfats COMME UN MEC QUI FINIT EN PRISON
        // CAR J4AI PAS DE STYLEE >:((((

        var sorter = GetComponent<RendererSorter>();
        if(sorter != null) {
            sorter.enabled = false;
        }
        var ctrl = GetComponent<PlayerController>();
        if(ctrl != null) {
            ctrl.enabled = false;
        }

        foreach(Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void ChangePlayerForm(PlayerForm form) {
        Time.timeScale = 1f;
        UI.EvolveScreen.gameObject.SetActive(false);


        currentForm = form;
        UpdatePlayerForm();
	}

    public const string ANIM_IDLE = "anim_idle";
    public const string ANIM_RIGHT = "anim_walk_right";
    public const string ANIM_TOP = "anim_walk_top";
    public const string ANIM_DOWN = "anim_walk_down";
    private void UpdatePlayerForm() {
        Animator.SetClip(ANIM_IDLE, currentForm.animation_Idle);
        Animator.SetClip(ANIM_RIGHT, currentForm.animation_Right);
        Animator.SetClip(ANIM_TOP, currentForm.animation_Top);
        Animator.SetClip(ANIM_DOWN, currentForm.animation_Bottom);

        // add skill bonus ?
        // 

    }

}
