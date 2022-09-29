using UnityEngine;
using System.Collections;

public class PlayerEntity : LivingEntity {

    [Header("Player configuration.")]
    [Tooltip("Duration of an attack")]
    [SerializeField] private float attackDuration = 0.2f;

    [Tooltip("Damage of an attack")]
    [SerializeField] private float attackDamage = 30f;

    [Tooltip("Cooldown between 2 attacks")]
    [SerializeField] private float attackCooldown = 0.25f;

    [Tooltip("The current monster form.")]
    [SerializeField] private PlayerForm currentForm;

    private bool attacking = false;
    private float nextAttack = 0;

    [Tooltip("The reference to the experience bar.")]
    [SerializeField] private BarUI experienceBar;
    [Tooltip("The number modulo of the evolve trigger.")]
    [SerializeField] private int evolveAllNlevels = 15;
    [Tooltip("The number modulo of the skill choice trigger.")]
    [SerializeField] private int skillEveryNLevels = 3;
    [Tooltip("The reference to the new skill UI.")]
    [SerializeField] private NewSkillScreen newSkillScreen;
    [Tooltip("The reference to the skills display UI.")]
    [SerializeField] private SkillsDisplayer skillsDisplay;
    [Tooltip("The reference to the evolve UI.")]
    [SerializeField] private EvolveScreen evolveScreen;
    [Tooltip("The reference to the level UI.")]
    [SerializeField] private TMPro.TMP_Text levelText;

    [Tooltip("The reference to the death screen layout.")]
    [SerializeField] private DeathScreen deathScreen;
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
        experienceBar.Init(previousLevel, nextLevel, ExperiencePoints);
        levelText.text = "Lvl " + level;

        skills = new SkillsSet(maxNumberOfPassives);
    }

	public void TryAttack(Orientation orientation) {
        // check cooldown.
        if(Time.time < nextAttack)
                return;
        nextAttack = Time.time + attackCooldown;

        attacking = true;
        currentForm.AttackShape.SpawnHurtbox(orientation, transform, attackDamage, attackDuration);

        // reset the boolean after some time.
        StartCoroutine(Utils.DoAfter(attackDuration, () => attacking = false));
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
        experienceBar.Init(previousLevel, nextLevel, ExperiencePoints);
	}

    private void LevelUp() {
        level++;
        previousLevel = nextLevel;
        nextLevel *= 2;

        if(level % skillEveryNLevels == 0) {
            // Skill...

            Time.timeScale = 0; //TODO faire un manager pour ce genre de connerie ?
            newSkillScreen.gameObject.SetActive(true);

            if(level % evolveAllNlevels == 0) {
                // Skill + Evolve
                newSkillScreen.FindSkills(skills, LevelUp_ThenEvolve);
            } else {
                // Skill.
                newSkillScreen.FindSkills(skills, LevelUp_Over);
            }
        } else {
            // Elvove.
            if(level % evolveAllNlevels == 0) {
                Time.timeScale = 0;
                evolveScreen.gameObject.SetActive(true);
                evolveScreen.DisplayForms(currentForm.Descendants, ChangePlayerForm);
            }
        }

        levelText.text = "Lvl " + level;

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

        evolveScreen.gameObject.SetActive(true);
        evolveScreen.DisplayForms(currentForm.Descendants, ChangePlayerForm);
    }

    private void AddSkill(Skill skill) {
        // add the skill
        newSkillScreen.gameObject.SetActive(false);
        skills.AddSkill(skill);

        // Refresh global displayed list
        skillsDisplay.SetSkills(skills.GetPassives());

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
            deathScreen.gameObject.SetActive(true);
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
        evolveScreen.gameObject.SetActive(false);


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
