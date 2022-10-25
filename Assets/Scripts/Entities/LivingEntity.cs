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
    [SerializeField] protected BarUI healthBar;

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

    public bool IsPlayer() {
        return GetEntityType() == EntityType.Player;
	}

    public virtual EntityType GetEntityType() {
        return EntityType.Enemy;
	}

    public void AddMaxHealth(float amount) {
        if(IsDead())
            return;
        _maxHealth += amount;
        healthBar.Init(0, _maxHealth, Health);
	}

    public void SetMaxHealth(float amount) {
        _maxHealth = amount;
        healthBar.Init(0, _maxHealth, Health);
    }
    public void SetMaxHealthAndHeal(float amount) {
        _maxHealth = amount;
        Health = amount;
        healthBar.Init(0, _maxHealth, Health);
    }

    /// <summary>
    /// Damage an entity.
    /// </summary>
    /// <param name="damage">The amount of damages to deal. If negative, does nothing.</param>
    /// <returns>The real amount of damages applied.</returns>
    public float Damage(float damage) {
        AssertInitialized();

        damage -= GetDamageReduction();
        if(damage < 0 || dead || invincible) {
            SpawnDamageText("[Blocked]", DamageText.DamageType.Blocked);
            return 0;
        }

        Health -= damage;
        SpawnDamageText("-"+(damage < 1 ? "0":"")+damage.ToString("#.##"), DamageText.GetTypeFromEntityType(GetEntityType()));

        if(Health <= 0) {
            Health = 0;
            dead = true;
            Die();
        }

        healthBar?.SetValue(Health);
        HealthChanged(-damage);

        return damage;
    }

    /// <summary>
    /// Called when the health of the entity changed.
    /// </summary>
    protected virtual void HealthChanged(float delta) { }

    protected void SpawnDamageText(string value, DamageText.DamageType type) {
        var text = Instantiate(ManagerUI.Instance.DamageTextPrefab);
        text.SetDamageText(value, transform.position + new Vector3(0, .5f, 0), type);
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
            SpawnDamageText("+" + heal.ToString("#.##"), DamageText.DamageType.PlayerHeal);

        if(Health > MaxHealth)
            Health = MaxHealth;

        healthBar?.SetValue(Health);
        HealthChanged(heal);
    }

    public bool IsDead() {
        return dead;
    }
}
