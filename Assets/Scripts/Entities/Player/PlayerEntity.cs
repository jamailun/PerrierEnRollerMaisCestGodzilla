using UnityEngine;
using System.Collections;

public class PlayerEntity : LivingEntity {

    [Header("UI link")]

    [Tooltip("UI Manager reference")]
    [SerializeField] private ManagerUI UI;

    [Header("Player configuration.")]

    [Tooltip("The current monster form.")]
    [SerializeField] private PlayerForm currentForm;
    public PlayerForm PlayerForm => currentForm;

    // config with numbers
    [Tooltip("The number modulo of the skill choice trigger.")]
    [SerializeField] private int skillEveryNLevels = 3;
    [Tooltip("The number modulo of the evolve trigger.")]
    [SerializeField] private int evolveEveryNLevels = 15;
    [Tooltip("Amount of differents passive skills")]
    [SerializeField] private int maxNumberOfPassives = 3;

    // config of death
    [Tooltip("The prefab for the death animation")]
    [SerializeField] private GameObject deathAnimation;
    [Tooltip("The duration of the death animation.")]
    [SerializeField] private float deathAnimationDuration;

    // Data set
    private SkillsSet skills;
    private readonly StatisticsSet stats = new();
    // Buffers, to not recalculate everything 200 times per second.
    private float buffer_Speed;
    private float buffer_Armor;

    // In case of multiple level up, we ave to keep track of how many skills/evolve we can get.
    private int skillsToGet = 0;
    private int evolvesToGet = 0;

    // Points get during this run
    public uint UpgradePoints { get; set; }
    public ulong ExperiencePoints { get; private set; }

    // Level variables
    private int level = 1;
    private ulong nextLevelExo = 100;
    private ulong previousLevelExp = 0;

    // Attacking variables
    private bool attacking = false;
    private float nextAttack = 0;

    // The start time of the player. Used to determine the length of a run.
    private float startedTime;

    // Reference to the animator.
    public CustomAnimator Animator { get; private set; }

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
        AddExperience(0); // update the bar
        UI.ExperienceLabel.text = "Lvl " + level;

        skills = new SkillsSet(maxNumberOfPassives);

        // calculate buffers
        UpdateBufferStats();
    }

	public void TryAttack(Orientation orientation) {
        // check cooldown.
        if(Time.time < nextAttack)
                return;
        nextAttack = Time.time + currentForm.AttackShape.AttackCooldown;
        attacking = true;

        // Create the attack itself
        float attackDuration = currentForm.AttackShape.AttackDuration;
        float attackDamage = stats.GetPower(Statistic.Attack, _flatDamages + currentForm.AttackShape.AttackDamageBonus);
        currentForm.AttackShape.SpawnHurtbox(orientation, transform, attackDamage, attackDuration);
        //TODO ajouter les dmg spécifiques sur batiments et ennemis.

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
        return buffer_Speed;
	}

    public void AddExperience(ulong amount) {
        ExperiencePoints += (ulong) stats.GetPower(Statistic.ExpGained, amount);
        while(ExperiencePoints > nextLevelExo) {
            LevelUp();
		}
        UI.ExperienceBar.Init(previousLevelExp, nextLevelExo, ExperiencePoints);
    }


    private void LevelUp() {
        // exp variables
        level++;
        previousLevelExp = nextLevelExo;
        nextLevelExo *= 2;

        // add stats
        AddMaxHealth(currentForm.bonusMaxHealthPerLevel);
        _flatDamages += currentForm.bonusAttackPerLevel;
        UpdateBufferStats();

        // skills & evolve
        if(level % skillEveryNLevels == 0) {
            skillsToGet++;
        }
        if(level % evolveEveryNLevels == 0) {
            evolvesToGet++;
        }
        Heal(MaxHealth * 0.2f);

        TryUpgrade();

        UI.ExperienceLabel.text = "Lvl " + level;
	}

    private void UpdateBufferStats() {
        buffer_Speed = stats.GetPower(Statistic.Speed, _speed);
        buffer_Armor = stats.GetPower(Statistic.Defense, _flatDamages);
    }

	protected override float GetDamageReduction() {
        return buffer_Armor;
	}

	private void TryUpgrade() {
        // Gagner un skill
        if(skillsToGet > 0) {
            Time.timeScale = 0f;
            UI.NewSkillScreen.gameObject.SetActive(true);
            UI.NewSkillScreen.FindSkills(skills, Upgrade_Skill);
            return;
		}

        // Évoluer
        if(evolvesToGet > 0 && currentForm.Descendants.Count > 0) {
            Time.timeScale = 0f;
            UI.EvolveScreen.gameObject.SetActive(true);
            UI.EvolveScreen.DisplayForms(currentForm.Descendants, Upgrade_PlayerForm);
            return;
        }

        // Si rien à faire, on continue le temps.
        Time.timeScale = 1f;
    }

    private void Upgrade_Skill(Skill skill) {
        UI.NewSkillScreen.gameObject.SetActive(false);

        AddSkill(skill);

        skillsToGet--;

        TryUpgrade();
    }

    private void Upgrade_PlayerForm(PlayerForm form) {
        UI.EvolveScreen.gameObject.SetActive(false);

        ChangePlayerForm(form);

        evolvesToGet--;

        TryUpgrade();
    }

    private void AddSkill(Skill skill) {
        // add the skill
        UI.NewSkillScreen.gameObject.SetActive(false);
        skills.AddSkill(skill);

        // Refresh global displayed list
        UI.SkillsDisplayer.SetSkills(skills.GetSkills());

        // Update stats
        stats.ResetFrom(skills);
        UpdateBufferStats();
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

    public void ChangePlayerForm(PlayerForm form) {
        currentForm = form;
        UpdatePlayerForm();
	}

    public const string ANIM_IDLE = "anim_idle";
    public const string ANIM_RIGHT = "anim_walk_right";
    public const string ANIM_TOP = "anim_walk_top";
    public const string ANIM_DOWN = "anim_walk_down";
    private void UpdatePlayerForm() {
        // Change scale
        gameObject.transform.localScale = new Vector3(currentForm.Scale, currentForm.Scale, 1f);

        // Change hitbox
        currentForm.HurtboxDescriptor.CreateCollider(GetComponentInChildren<Hurtbox>());

        // Change magnet
        var magnet = GetComponentInChildren<AttractiblesMagnet>();
        magnet.SetParameters(currentForm.magnetOffset, currentForm.magnetRange);

        // Change ground collider
        var groundCollider = GetComponent<CapsuleCollider2D>();
        groundCollider.enabled = currentForm.groundCollider;
        if(currentForm.groundCollider) {
            groundCollider.offset = currentForm.groundColliderOffset;
            groundCollider.size = currentForm.groundColliderSize;
        }

#if UNITY_EDITOR
        GetComponentInChildren<SpriteRenderer>().sprite = currentForm.animation_Idle.GetCurrent(0);
#endif

        // Change animation
        if(Animator != null) {
            Animator.SetClip(ANIM_IDLE, currentForm.animation_Idle);
            Animator.SetClip(ANIM_RIGHT, currentForm.animation_Right);
            Animator.SetClip(ANIM_TOP, currentForm.animation_Top);
            Animator.SetClip(ANIM_DOWN, currentForm.animation_Bottom);
        }

        // add skill bonus ?
    }

}
