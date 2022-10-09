using UnityEngine;

public class PlayerEntity : LivingEntity {

    [Header("UI link")]

    [Tooltip("UI Manager reference")]
    [SerializeField] private ManagerUI UI;

    [Header("Player configuration.")]

    [Tooltip("The current monster form.")]
    [SerializeField] private PlayerForm currentForm;
    public PlayerForm PlayerForm => currentForm;

    [SerializeField] private Transform spellOutput;

    // config with numbers
    [Tooltip("The number modulo of the skill choice trigger.")]
    [SerializeField] private int skillEveryNLevels = 3;
    [Tooltip("The number modulo of the evolve trigger.")]
    [SerializeField] private int evolveEveryNLevels = 15;
    [Tooltip("Amount of differents passive skills")]
    [SerializeField] private int maxNumberOfPassives = 3;

    [Tooltip("The EXP (Y) needed per level (X)")]
    [SerializeField] private AnimationCurve experienceCurve;
    [Tooltip("Maximum level the player can get")]
    [SerializeField] [Range(10, 50)] private int maxLevel = 35;

    private float currentScaleMult = 1f;
    [SerializeField] private float growScaleOnNewSkill = 1f;
    [SerializeField] private float growScaleOnEvolve = 1f;

    [SerializeField] private AudioClip levelupSfx;

    // config of death
    [Tooltip("The prefab for the death animation")]
    [SerializeField] private GameObject deathAnimation;
    [Tooltip("The duration of the death animation.")]
    [SerializeField] private float deathAnimationDuration;

    [SerializeField] private float startCameraSize = 4f;

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
    private ulong nextLevelExp;
    private ulong previousLevelExp = 0;

    // Attacking variables
    private bool attacking = false;
    private float nextAttack = 0;

    // Dash variable
    [HideInInspector] public Vector3 DashTarget;
    [HideInInspector] public Vector2 DashDirection;
    [SerializeField] private ParticleSystem dashVFX;
    private float nextDash;
    private bool dashing = false;

    // damage effect
    [SerializeField] private ParticleSystem bloodVFX;
    [SerializeField] private float damagedDuration = 0.1f;
    [SerializeField] private Color damagedColor = Color.white;

    // The start time of the player. Used to determine the length of a run.
    private float startedTime;

    // Reference to the animator.
    public CustomAnimator Animator { get; private set; }
    public AudioSource AudioSource { get; private set; }

    public static bool shouldStart = true;
	private void Start() {
        if(!shouldStart)
            return;
        shouldStart = false;
        AudioSource = gameObject.GetOrAddComponent<AudioSource>();
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
        TimerUI.StartTimer(startedTime);
        if(DifficultyDisplayer.Instance != null) {
            DifficultyDisplayer.Instance.Init(startedTime);
		} else {
            Debug.LogError("No difficulty manager(displayer) set.");
		}

        ExperiencePoints = 0;
        nextLevelExp = (ulong) Mathf.FloorToInt(experienceCurve.Evaluate(level + 1));
        AddExperience(0); // update the bar
        UI.ExperienceLabel.text = "Lvl " + level;

        skills = new SkillsSet(maxNumberOfPassives, 3);

        UpgradePoints = 0;
        UI.ScoreDisplay.UpdateScore(0);

        // calculate buffers
        UpdateBufferStats();

        // DEBUG ONLY
        //StartCoroutine(Utils.DoAfter(1f, () => AddSkill(SkillLibrairy.GetActivesSkills()[1])));
    }

	public void TryAttack(Orientation orientation) {
        // check cooldown.
        if(Time.time < nextAttack)
                return;

        nextAttack = Time.time + currentForm.AttackShape.AttackCooldown;
        attacking = true;

        // Create the attack itself
        float attackDuration = currentForm.AttackShape.AttackDuration;
        float attackDamage = GetCurrentDamages();
        float attackScale = GetCurrentRange();
        var hitbox = currentForm.AttackShape.SpawnHitbox(orientation, transform, attackDamage, attackDuration, attackScale);
        // Add additional damages
        hitbox.BonusDamagesBuilding = stats.GetPower(Statistic.AttackBonusBuildings, attackDamage) - attackDamage;
        hitbox.BonusDamagesEnemies = stats.GetPower(Statistic.AttackBonusEnemies, attackDamage) - attackDamage;

        // reset the boolean after some time.
        StartCoroutine(Utils.DoAfter(attackDuration, () => attacking = false));
	}

    public float GetCurrentDamages() {
        return stats.GetPower(Statistic.Attack, _flatDamages + currentForm.AttackShape.AttackDamageBonus);
    }
    public float GetCurrentRange() {
        return stats.GetPower(Statistic.Range, currentForm.AttackScale);
    }

    public void TryDash(float horizontal, float vertical) {
        if(horizontal == 0 && vertical == 0 || attacking) return;
        if(dashing || Time.time < nextDash || !currentForm.CanDash) return;

        dashing = true;
        nextDash = Time.time + currentForm.dashCooldown;

        DashDirection = new Vector2(horizontal, vertical).normalized;
        var rayhit = Physics2D.Raycast(transform.position, DashDirection, currentForm.dashDistance, LayerMask.NameToLayer("Obstacles"));
        if(rayhit) {
            DashTarget = new Vector3(rayhit.point.x, rayhit.point.y, transform.position.z);
        } else {
            DashTarget = transform.position + new Vector3(DashDirection.x, DashDirection.y, 0) * currentForm.dashDistance;
        }

        DashStarted = Time.time;
        dashVFX.Play();
    }

	public override EntityType GetEntityType() {
        return EntityType.Player;
	}
	public bool IsDashing() { return dashing; }
    public void StopDashing() { dashing = false; dashVFX.Stop(); }
    public float DashStarted { get; private set; }

    public float GetSpeed() {
        if(dead || attacking && !currentForm.AttackShape.CanMoveOnAttack)
            return 0;

        // dahs = fast
        if(dashing)
            return buffer_Speed * 2f;

        // Can be used to do slow/run effects.
        return buffer_Speed;
	}

    public void AddExperience(ulong amount) {
        ExperiencePoints += (ulong) stats.GetPower(Statistic.ExpGained, amount);
        while(ExperiencePoints > nextLevelExp && level < maxLevel) {
            LevelUp();
		}
        UI.ExperienceBar.Init(previousLevelExp, nextLevelExp, ExperiencePoints);
    }

    private void LevelUp() {
        // exp variables
        level++;
        Debug.Log("new level : " + level);
        previousLevelExp = nextLevelExp;
        nextLevelExp = (ulong) Mathf.FloorToInt(experienceCurve.Evaluate(level+1));
        if(nextLevelExp <= previousLevelExp) {
            Debug.LogWarning("Carefull ! Level " + level + " as smaller required exp than the preivous ("+previousLevelExp + " -> " + nextLevelExp+"). +5% of the previous exp then.");
            nextLevelExp = (ulong) (((double) nextLevelExp)* 1.05);
		}

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

    private void GrowScale(float amount) {
        currentScaleMult *= amount;
        float scale = currentScaleMult * currentForm.Scale;
        gameObject.transform.localScale = new Vector3(scale, scale, 1);

        Camera.main.orthographicSize = startCameraSize * amount;
    }

    private void UpdateBufferStats() {
        buffer_Speed = stats.GetPower(Statistic.Speed, _speed);
        buffer_Armor = stats.GetPower(Statistic.Defense, _flatArmor);
        _healthRegen = stats.GetPower(Statistic.HealthRegen, 0);
    }

	protected override float GetDamageReduction() {
        return buffer_Armor;
	}

	private void TryUpgrade() {
        // Gagner un skill
        if(skillsToGet > 0) {
            Time.timeScale = 0f;
            if(levelupSfx != null) {
                AudioSource.PlayOneShot(levelupSfx);
			}
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
        skillsToGet--;

        // Grow for evolve
        GrowScale(growScaleOnNewSkill);

        UI.NewSkillScreen.gameObject.SetActive(false);

        AddSkill(skill);

        TryUpgrade();
    }

    private void Upgrade_PlayerForm(PlayerForm form) {
        evolvesToGet--;

        // Grow for evolve
        GrowScale(growScaleOnEvolve);

        UI.EvolveScreen.gameObject.SetActive(false);

        ChangePlayerForm(form);

        TryUpgrade();
    }

    private void AddSkill(Skill skill) {
        // add the skill
        if(skill == null)
            return;

        skills.AddSkill(skill);
        if(skill.IsActive()) {
            UI.ActiveButtons.Add((ActiveSkill)skill, this);
		}

        // Refresh global displayed list
        UI.SkillsDisplayer.SetSkills(skills.GetPassiveSkills());

        // Update stats
        stats.ResetFrom(skills);
        UpdateBufferStats();
    }

    public void Conceede() {
        TimerUI.Stop();
        PersistentData.EndRun(Time.time - startedTime, level, UpgradePoints);
        LoadingManager.ResetGameAndGoMenu();
    }

    protected override void Die() {
        // Save data
        TimerUI.Stop();
        PersistentData.EndRun(Time.time - startedTime, level, UpgradePoints);

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

        // remove enemies IA
        foreach(var enemy in FindObjectsOfType<Enemy>())
            enemy.enabled = false;

        // music
        if(LevelMusicPlayer.CurrentInstance)
            LevelMusicPlayer.CurrentInstance.PlayDeathMusic();

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
        GrowScale(1f);

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

	public void Pause() {
        UI.Pause();
	}

    public Vector3 GetOutputPosition() {
        return spellOutput.position;
    }

    public void Buff(StatisticModifier modifier, float duration) {
        stats.AddTemporaryStats(modifier);
        UpdateBufferStats();

        StartCoroutine(Utils.DoAfter(duration, () => {
            stats.RemoveTemporaryStats(modifier);
            UpdateBufferStats();
        }));
    }

	protected override void HealthChanged(float delta) {
        if(delta >= 0) // only damage effect
            return;

        // VFX
        if(bloodVFX != null)
            bloodVFX.Play();
        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        if(renderer != null)
            renderer.color = damagedColor;

        // non-damage period
        invincible = true;


        // reset variables
        StartCoroutine(Utils.DoAfter(damagedDuration, () => {
            if(bloodVFX != null)
                bloodVFX.Stop();
            if(renderer != null)
                renderer.color = Color.white;
            invincible = false;
        }));
	}

    public void AddRewardPoints(uint amount) {
        UpgradePoints += amount;
        UI.ScoreDisplay.UpdateScore(UpgradePoints);
    }

    public GameData ExportData() {
        return new GameData {
            currentForm = currentForm,
            currentHealth = Health,
            experience = ExperiencePoints,
            level = level,
            rewardPoints = UpgradePoints,
            runStarted = startedTime,
            skills = skills,
            flatDamages = _flatDamages,
            maxHealth = MaxHealth
        };
	}

    public void ImportData(GameData data) {
        Start();

        currentForm = data.currentForm;
        base.SetMaxHealth(data.maxHealth);
        Health = data.currentHealth;
        ExperiencePoints = data.experience;
        level = data.level;
        startedTime = data.runStarted;
        UpgradePoints = data.rewardPoints;
        skills = data.skills;
        _flatDamages = data.flatDamages;

        UpdatePlayerForm();
        GrowScale(1f);
        UpdateBufferStats();
	}
}
