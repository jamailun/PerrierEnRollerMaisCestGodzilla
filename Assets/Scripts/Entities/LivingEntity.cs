using UnityEngine;

public abstract class LivingEntity : MonoBehaviour {

    [Header("Entity attributes")]

    [Tooltip("The max health of this entity.")]
    [SerializeField] private float _maxHealth = 100f;

    [Tooltip("Regeneration per second.")]
    [SerializeField] protected float _healthRegen = 0.1f;

    [Tooltip("Flat armor")]
    [SerializeField] protected float _flatArmor = 0f;

    [Tooltip("Flat damages")]
    [SerializeField] protected float _flatDamages = 0f;

    [Tooltip("The movement speed of this entity.")]
    [SerializeField] protected float _speed = 8f;

    [Tooltip("Optional BarUI for the health.")]
    [SerializeField] private BarUI healthBar;

    [Tooltip("IF the entity can take damages or not.")]
    [SerializeField] protected bool invincible = false;

    // THe current health of the entity.
    public float Health { get; protected set; }
    public float MaxHealth => _maxHealth;

    private bool initialized = false;
    protected bool dead = false;

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

    // Moyen le plus rapide de savoir si une entit� est joueur ou pas, plutot que du typeof.
    // Notons que �a augmente la flexibilit� du truc.
    public virtual bool IsPlayer() {
        return false;
	}

    public void AddMaxHealth(float amount) {
        if(IsDead())
            return;
        _maxHealth += amount;
        healthBar.Init(0, _maxHealth, Health);
	}

    public void Damage(float damage) {
        AssertInitialized();

        damage -= GetDamageReduction();
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
        HealthChanged();
    }

    /// <summary>
    /// Called when the health of the entity changed.
    /// </summary>
    protected virtual void HealthChanged() { }

    private void SpawnDamageText(float amount) {
        /*var obj = new GameObject();
        var txt = obj.AddComponent<DamageText>();
        txt.SetDamageText(amount, transform.position + new Vector3(0, .1f, 0));
        // */
    }

    protected virtual float GetDamageReduction() {
        return _flatArmor;
	}

	private void LateUpdate() {
        if(_healthRegen > 0)
            Heal(_healthRegen * Time.deltaTime, false);
	}

	private void AssertInitialized() {
        if(!initialized)
            Debug.LogError("LIVING ENTITY " + name + " HASN'T BEEN INITIALIZED.");
    }

    public void Heal(float heal, bool showText = true) {
        AssertInitialized();
        if(heal < 0 || dead)
            return;

        Health += heal;
        if(showText)
            SpawnDamageText(-heal);

        if(Health > MaxHealth)
            Health = MaxHealth;

        healthBar?.SetValue(Health);
        HealthChanged();
    }

    public bool IsDead() {
        return dead;
    }
}
