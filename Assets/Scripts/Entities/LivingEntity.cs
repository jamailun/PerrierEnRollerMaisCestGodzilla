using UnityEngine;

public class LivingEntity : RendererSorter /* Car toutes les entités sont SORT selon le Y. */ {

    [Header("Entity attributes")]

    [Tooltip("The max health of this entity.")]
    [SerializeField] private float _maxHealth = 100f;

    [Tooltip("The movement speed of this entity.")]
    [SerializeField] protected float _speed = 8f;

    [Tooltip("Optional BarUI for the health.")]
    [SerializeField] private BarUI healthBar;

    [Tooltip("IF the entity can take damages or not.")]
    [SerializeField] private bool invincible = false;

    // THe current health of the entity.
    public float Health { get; protected set; }
    public float MaxHealth => _maxHealth;

    private bool initialized = false;
    private bool dead = false;

    protected virtual float LifeChangeRequest(float oldAmount, float newAmount) {
        if(invincible && newAmount < oldAmount)
            return oldAmount;
        return newAmount;
    }

	private void Awake() {
        // If a child overrides #Awake, it has to call the #InitLiving.
        InitLiving();
    }

    protected void InitLiving() {
        // Health
        if(MaxHealth <= 0) {
            Debug.LogError("Entity " + name + " cannot have a MaxHealth <= 0.");
            dead = true;
            enabled = false;
            return;
        }
        Health = MaxHealth;

        // Health bar
        if(healthBar != null) {
            healthBar.Init(0, MaxHealth, Health);
        }
        // End
        initialized = true;
    }

    protected virtual void Die() {
        dead = true;
        //TODO VFX ?
        Destroy(gameObject);
    }

    // Moyen le plus rapide de savoir si une entité est joueur ou pas, plutot que du typeof.
    // Notons que ça augmente la flexibilité du truc.
    public virtual bool IsPlayer() {
        return false;
	}

    public void Damage(float damage) {
        AssertInitialized();
        if(damage < 0 || dead || invincible)
            return;

        Health -= damage;
        SpawnDamageText(damage);

        if(Health <= 0) {
            Health = 0;
            dead = true;
            Die();
        }

        healthBar?.SetValue(Health);
    }

    private void SpawnDamageText(float amount) {
        /*var obj = new GameObject();
        var txt = obj.AddComponent<DamageText>();
        txt.SetDamageText(amount, transform.position + new Vector3(0, .1f, 0));
        // */
    }

    private void AssertInitialized() {
        if(!initialized)
            Debug.LogError("LIVING ENTITY " + name + " HASN'T BEEN INITIALIZED.");
    }

    public void Heal(float heal) {
        AssertInitialized();
        if(heal < 0 || dead)
            return;

        Health += heal;
        SpawnDamageText(-heal);

        if(Health > MaxHealth)
            Health = MaxHealth;

        healthBar?.SetValue(Health);
    }

    public bool IsDead() {
        return dead;
    }
}
