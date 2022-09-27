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

    [Tooltip("The shape of the attacks.")]
    [SerializeField] private AttackShape attackShape;

    private bool attacking = false;
    private float nextAttack = 0;

    [Tooltip("The reference to the experience bar.")]
    [SerializeField] private BarUI experienceBar;
    [Tooltip("The number modulo of the evolve trigger.")]
    [SerializeField] private int evolveAllNlevels = 15;
    [Tooltip("The reference to the level UI.")]
    [SerializeField] private TMPro.TMP_Text levelText;

    [Tooltip("The reference to the death screen layout.")]
    [SerializeField] private DeathScreen deathScreen;
    [Tooltip("The prefab for the death animation")]
    [SerializeField] private GameObject deathAnimation;
    [Tooltip("The duration of the death animation.")]
    [SerializeField] private float deathAnimationDuration;

    public uint UpgradePoints { get; set; }
    public ulong ExperiencePoints { get; private set; }
    private int level = 1;
    private ulong nextLevel = 100;
    private ulong previousLevel = 0;

    private float startedTime;

    private void Start() {
        if(attackShape == null) {
            Debug.LogError("Error, no attackshape for player.");
            enabled = false;
        }

        startedTime = Time.time;
        TimerUI.StartTimer();

        experienceBar.Init(previousLevel, nextLevel, ExperiencePoints);
        levelText.text = "Lvl " + level;
    }

	public void TryAttack(Orientation orientation) {
        // check cooldown.
        if(Time.time < nextAttack)
                return;
        nextAttack = Time.time + attackCooldown;

        attacking = true;
        attackShape.SpawnHurtbox(orientation, transform, attackDamage, attackDuration);

        // reset the boolean after some time.
        StartCoroutine(Utils.DoAfter(attackDuration, () => attacking = false));
	}

    public override bool IsPlayer() {
        return true;
    }

    public float GetSpeed() {
        if(attacking && !attackShape.CanMoveOnAttack)
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

        if(level % evolveAllNlevels == 0) {
            Debug.Log("Should evolve right now !");
		}

        levelText.text = "Lvl " + level;

        Debug.Log("Level up ! nex level="+level);
        Heal(MaxHealth * 0.2f); // heal de 20% ??

        //TODO vfx & sfx
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

}
