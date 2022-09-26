using UnityEngine;

public class PlayerEntity : LivingEntity {

    [Header("Player configuration.")]
    [Tooltip("Duration of an attack")]
    [SerializeField] private float attackDuration = 0.2f;

    [Tooltip("Damage of an attack")]
    [SerializeField] private float attackDamage = 30f;

    [Tooltip("Cooldown between 2 attacks")]
    [SerializeField] private float attackCooldown = 0.25f;

    private float nextAttack; // time à partir duquel on peut attaquer.

    public void TryAttack() {
        if(Time.time < nextAttack)
            return;
        nextAttack = Time.time + attackCooldown;
        // spawn attack zone ?
	}

    public override bool IsPlayer() {
        return true;
    }

    public float GetSpeed() {
        // Can be used to do slow/run effects.
        return _speed;
	}

}
