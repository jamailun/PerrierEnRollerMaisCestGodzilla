using UnityEngine;

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

    public uint UpgradePoints { get; set; }
    public ulong ExperiencePoints { get; private set; }
    private int level = 1;
    private ulong nextLevel = 1000;

    private void Start() {
        if(attackShape == null) {
            Debug.LogError("Error, no attackshape for player.");
            enabled = false;
		}
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
	}

    private void LevelUp() {
        level++;
        nextLevel *= 2;

        Debug.Log("Level up ! nex level="+level);
        Heal(MaxHealth * 0.2f); // heal de 20% ??
	}

}
