using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// pour faire un truc avec les boss, il faudrait que cette classe soit abstraite avec juste une methode Getter de enemytype;
// et que une sous-class soit genre "IAEnemy" et dont le comportement change en fonction du type.
// Comme ça la classe boss peut override Enemy et override les méthodes qui choississent le comportement.
public class Enemy : LivingEntity {

#region parameters

	[Header("Enemy attributes")]
	[Tooltip("Describes the IA of the Enemy.")]
	[SerializeField] private EnemyType enemyType;

    [SerializeField] private bool invertFlip = false;

    // -------------------------------------------------------------
    protected Transform Target { get; private set; }

    [Space]

    [SerializeIf("enemyType", EnemyType.None, ComparisonType.NotEqual)]
    [Tooltip("Time between recalculation (in seconds).")]
    [SerializeField] protected float recalculateAfter = 0.2f;

    [SerializeIf("enemyType", EnemyType.None, ComparisonType.NotEqual)]
    [Tooltip("Time between two attacks of an enemy, in seconds")]
    [SerializeField] protected float attackSpeed = 1f;

    [SerializeIf("enemyType", EnemyType.None, ComparisonType.NotEqual)]
    [Tooltip("Percentage of intentional error between two attacks")]
    [Range(0f, 0.5f)]
    [SerializeField] protected float attackSpeedEpsilon = 0.05f;

    [SerializeIf("enemyType", EnemyType.None, ComparisonType.NotEqual)]
    [Tooltip("Attack effect on shot")]
    [SerializeField] private ParticleSystem attackEffect;

    [SerializeIf("enemyType", EnemyType.None, ComparisonType.NotEqual)]
    [Tooltip("Sound effect on shot")]
    [SerializeField] private AudioClip attackSoundEffect;

    [SerializeIf("enemyType", EnemyType.None, ComparisonType.NotEqual)]
    [Tooltip("If true, don't repeat s/vfx on multiple attacks")]
    [SerializeField] private bool attackEffectsOnlyOnce = false;

    [SerializeIf("enemyType", EnemyType.None, ComparisonType.NotEqual)]
    [Tooltip("Transform to shot attacks from")]
    [SerializeField] protected Transform attack_output;

    [SerializeIf("enemyType", EnemyType.None, ComparisonType.NotEqual)]
    [Tooltip("Geometric scale for attacks")]
    [SerializeField] protected float attackScale = 1f;

    // Melee

    [SerializeIf("enemyType", EnemyType.Melee)]
    [Tooltip("The range required to attack the player")]
    [SerializeField] protected float meleeRange = .5f;

    [SerializeIf("enemyType", EnemyType.Melee)]
    [Tooltip("The duration of the attack")]
    [SerializeField] protected float meleeAttackDuration = .1f;

    [SerializeIf("enemyType", EnemyType.Melee)]
    [Tooltip("The hitbox to spawn to attack")]
    [SerializeField] private Hitbox meleeAttackPrefab_side;

    [SerializeIf("enemyType", EnemyType.Melee)]
    [Tooltip("Index on the animation to spawn the hitbox. IF DEFINED !!")]
    [SerializeField] protected int melee_animation_spawn = -1;

    // Distance

    [SerializeIf("enemyType", EnemyType.Distance, ComparisonType.GreaterOrEqual)]
    [Tooltip("The distance exact to keep from the target")]
    [SerializeField] protected float distance_wanted = 2.2f;
    
    [SerializeIf("enemyType", EnemyType.Distance, ComparisonType.GreaterOrEqual)]
    [Tooltip("The allowed error of distance to keep")]
    [SerializeField] private float distance_epsilon = .3f;

    [SerializeIf("enemyType", EnemyType.Distance, ComparisonType.GreaterOrEqual)]
    [Tooltip("Time before shoot")]
    [SerializeField] private float distance_shot_load = .8f;

    [SerializeIf("enemyType", EnemyType.Distance, ComparisonType.GreaterOrEqual)]
    [Tooltip("Time waiting after shoot")]
    [SerializeField] private float distance_shot_postload = 0f;

    [SerializeIf("enemyType", EnemyType.Distance, ComparisonType.GreaterOrEqual)]
    [Tooltip("Index on the animation to spawn the projectile. IF DEFINED !!")]
    [SerializeField] private int distance_animation_spawn = -1;

    [SerializeIf("enemyType", EnemyType.Distance, ComparisonType.GreaterOrEqual)]
    [Tooltip("Projectile to shot")]
    [SerializeField] protected Projectile projectile_prefab;

    [SerializeIf("enemyType", EnemyType.Distance, ComparisonType.GreaterOrEqual)]
    [Tooltip("Imprecision radius of a projectile")]
    [SerializeField] [Range(0f, 5f)] private float projectile_imprecision = 0f;

    [SerializeIf("enemyType", EnemyType.Distance, ComparisonType.GreaterOrEqual)]
    [Tooltip("Amount of projectiles to fire")]
    [SerializeField] private int distance_projectiles_amount = 1;

    [SerializeIf("enemyType", EnemyType.Distance, ComparisonType.GreaterOrEqual)]
    [Tooltip("if multiple projectiles, frame to attack at")]
    [SerializeField] private int distance_projectiles_amount_deltaFrames = 3;

    [Space]

    [SerializeIf("enemyType", EnemyType.Boss, ComparisonType.NotEqual)]
    [Tooltip("The experience prefab to use.")]
    [SerializeField] private ExperienceBall experiencePrefab;

    [SerializeIf("enemyType", EnemyType.Boss, ComparisonType.NotEqual)]
    [Tooltip("The amount of experience to drop")]
    [SerializeField] private ulong droppedExp = 10;

    // -------------------------------------------------------------

    [Header("Animation")]

    [SerializeField] private float walkAnimationScale = 1f;
    [SerializeField] private CustomAnimation walkAnimation;
    [SerializeField] private float attackAnimationScale = 1f;
    [SerializeField] private CustomAnimation attackAnimation;

    protected NavMeshAgent agent;
    protected SpriteRenderer spriteRenderer;
    protected CustomAnimator animator;

    protected float nextRecalculate; // next time to recalulate trajectory
    protected float nextAttackAllowed; // prochaine attaque.

    protected bool isAfraid = false;

#endregion

	protected virtual void Start() {
        ChangeStatsToScale();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if(spriteRenderer == null) {
            Debug.LogError("No sprite renderer in " + name + ".");
		}

        if(enemyType == EnemyType.None)
            return;

        agent = GetComponent<NavMeshAgent>();
        if(agent == null) {
            Debug.LogWarning("Warning enemy " + name + "should have a NavMeshAgent component.");
            agent = gameObject.AddComponent<NavMeshAgent>();
		}
        // 2d tricks
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        agent.speed = _speed;
        agent.acceleration = _speed * 1.5f;

        animator = gameObject.GetOrAddComponent<CustomAnimator>();
        animator.SetClip(ANIM_WALK, walkAnimation);
        animator.SetClip(ANIM_ATTACK, attackAnimation);
        PlayAnimationWalk();

        if(Target == null) {
            RefreshTarget();
            Recalculate();
        }
    }

    protected void ChangeStatsToScale() {
        float mult = DifficultyDisplayer.GetDifficultyMultiplier();
        _speed = Mathf.Min(_speed * 2f, _speed * mult);
        attackScale *= Mathf.Max(1f, mult);
        _flatDamages *= mult;
        SetMaxHealth(MaxHealth * mult);
	}

    protected const string ANIM_WALK = "anim_walk";
    protected const string ANIM_ATTACK = "anim_attack";

    protected void PlayAnimationWalk() {
        spriteRenderer.transform.localScale = new Vector3(walkAnimationScale, walkAnimationScale, 1f);
        //Debug.Log("(Enemy "+gameObject.name+") scale du renderer = " + walkAnimationScale);
        if(enemyType == EnemyType.Melee || enemyType == EnemyType.Distance)
            animator.PlayConditional(ANIM_WALK, PredicateWalkAnimation, walkAnimation.GetFirst());
        else
            animator.Play(ANIM_WALK);
    }

    protected void PlayAttackTemp(float duration) {
        spriteRenderer.transform.localScale = new Vector3(attackAnimationScale, attackAnimationScale, 1f);
        //Debug.Log("(Enemy " + gameObject.name + ") scale du renderer = " + walkAnimationScale);

        animator.PlayOnce(ANIM_ATTACK, duration, ANIM_WALK, walkAnimationScale);
	}

    protected bool PredicateWalkAnimation() {
        if(Target == null)
            return false;
        return agent.remainingDistance > 0.05f + agent.stoppingDistance;
	}

    private void Update() {
		if(enemyType == EnemyType.None || null == Target)
			return; // pas d'IA => on fait rien :)

        // Change direction
        if(animator.IsPlaying(ANIM_WALK))
            spriteRenderer.flipX = invertFlip != Target.position.x < transform.position.x;

        // Change target
        if(Time.time >= nextRecalculate)
            Recalculate();
    }

    private bool IsFlip() {
        return spriteRenderer.flipX != invertFlip;
    }

    protected virtual void Recalculate() {
        if(isAfraid && enemyType != EnemyType.None) {
            Vector3 dir = 2f * new Vector3(transform.position.x - Target.position.x, transform.position.y - Target.position.y, 0).normalized;
            Vector3 newTarget = transform.position + dir;
            Vector2 notSure = Random.insideUnitCircle * 1f;
            agent.isStopped = false;
            agent.SetDestination(newTarget + new Vector3(notSure.x, notSure.y, 0));
            nextRecalculate = Time.time + recalculateAfter * 3;
            return;
	    }
        // Set destination according to the type
        float d = Vector2.Distance(transform.position, Target.position);
        switch(enemyType) {

            // Melee always rush to the player
            case EnemyType.Melee:
                if(d <= meleeRange && Time.time >= nextAttackAllowed) {
                    Attack(false);
                    break;
				} else {
                    agent.SetDestination(Target.position);
                }
                break;

            case EnemyType.Distance:
                if((d < distance_wanted - distance_epsilon) || (d > distance_wanted + distance_epsilon)) {
                    var vec = (transform.position - Target.position);
                    var movement = vec.normalized * (distance_wanted - d);

                    agent.SetDestination(transform.position + movement);
				} else {
                    // on est dans la range, on attaque
                    if(Time.time >= nextAttackAllowed) {
                        Attack(true);
                        break;
                    }
                }
                break;

        }

        // Change recalculation.
        nextRecalculate = Time.time + recalculateAfter;
    }

    protected int previous_callback;
    protected int proj_n = 0;
    private void Attack(bool distance) {
        nextAttackAllowed = Time.time + (attackSpeed * (1f + Random.Range(-attackSpeedEpsilon, attackSpeedEpsilon)));
        agent.isStopped = true;

        // DISTANCE
        if(distance) {
            proj_n = 0;
            nextAttackAllowed += distance_shot_load;
            nextRecalculate += distance_shot_load * 1.1f;

            StartCoroutine(Utils.DoAfter(distance_shot_load + distance_shot_postload, () => agent.isStopped = false));
            if(distance_animation_spawn >= 0) {
                previous_callback = distance_animation_spawn;
                PlayAttackTemp(distance_shot_load);
                animator.SpecifyCallback(distance_animation_spawn, () => SpawnProjectile());
            } else {
                // simple
                StartCoroutine(Cor_AttackDistance());
            }
            return;
        }

        // MELEE    
        nextAttackAllowed += meleeAttackDuration;

        if(melee_animation_spawn > -1) {
            animator.SpecifyCallback(melee_animation_spawn, () => {
                float size = animator.GetClipSize(ANIM_ATTACK);
                SpawnHitboxMelee((size - melee_animation_spawn) / (float)size);
            });
		} else {
            SpawnHitboxMelee(1f);
        }

        // Animation
        PlayAttackTemp(meleeAttackDuration);

        StartCoroutine(Utils.DoAfter(meleeAttackDuration, () => {
            agent.isStopped = false;
        }));

    }

    protected void SpawnHitboxMelee(float durationPer) {
        var source = new Vector3(attack_output.position.x, attack_output.position.y, -.1f);

        AttackEffect(source);

        var hitbox = Instantiate(meleeAttackPrefab_side);
        hitbox.transform.position = source;
        hitbox.transform.localScale = new Vector3(attackScale, attackScale, 1f);
        hitbox.Spawn(_flatDamages, meleeAttackDuration * durationPer, IsFlip(), transform);
    }

    protected Vector3 GetOutput() {
        var source = new Vector3(attack_output.position.x, attack_output.position.y, -.1f);
        if(IsFlip()) {
            source.x -= 2 * attack_output.localPosition.x;
        }
        return source;
    }

    // Play the effects of the attack
    protected void AttackEffect(Vector3 source) {
        if(proj_n > 1 && attackEffectsOnlyOnce)
            return;
        if(attackEffect != null) {
            var vfx = Instantiate(attackEffect);
            vfx.transform.SetPositionAndRotation(source, Quaternion.identity);
        }
        if(attackSoundEffect != null) {
            var audio = gameObject.GetOrAddComponent<AudioSource>();
            audio.clip = attackSoundEffect;
            audio.Play();
        }
    }

    private IEnumerator Cor_AttackDistance() {
        PlayAttackTemp(distance_shot_load);

        yield return new WaitForSeconds(distance_shot_load);

        SpawnProjectile();
    }

    protected Projectile SpawnProjectile() {
        proj_n++;
        if(distance_animation_spawn > -1 && proj_n < distance_projectiles_amount) {
            previous_callback += distance_projectiles_amount_deltaFrames;
            animator.UpdateCallback(previous_callback);
        }

        // Calculate
        var source = GetOutput();

        // SFX
        AttackEffect(source);

        // Projectil
        var proj = Instantiate(projectile_prefab);
        Vector3 target = this.Target.position;
        if(projectile_imprecision > 0) {
            var mod = Random.insideUnitCircle.normalized * projectile_imprecision;
            target += new Vector3(mod.x, mod.y, 0);
		}
        if(proj.GetType() == typeof(FollowProjectile)) {
            ((FollowProjectile) proj).InitFollow(Target, transform);
        } else {
            proj.Init(source, target - source, transform);
        }
        proj.transform.localScale = new Vector3(attackScale, attackScale, 1f);
        proj.damages = _flatDamages;

        return proj;
    }

	protected override void Die() {
        // Drop item before delete the gameobject
        int c = 1;
        ulong amount = droppedExp / 10;
        while(amount >= 10) {
            amount /= 10;
            c += 2;
		}
        c = Mathf.Max(1, c + Random.Range(-1,1));

        ulong perBallExp = droppedExp / (ulong) c;

        for(int i = 0; i < c; i++) {
            Vector3 pos = Random.insideUnitCircle * 1.2f;
            var ball = Instantiate(experiencePrefab);
            ball.transform.position = transform.position + pos;
            ball.experienceAmount = perBallExp;
        }

		base.Die();
	}

    public void RefreshTarget() {
        var player = FindObjectOfType<PlayerEntity>();
        if(player == null) {
            Debug.LogWarning("Enemy " + name + " could NOT find player...");
            agent.isStopped = true;
            return;
        }
        Target = player.transform;
    }

    public virtual void MakeAfraid(float duration) {
        if(isAfraid)
            return;
        isAfraid = true;
        if(spriteRenderer != null)
            spriteRenderer.color = Color.gray;
        float oldBreak = agent.stoppingDistance;
        agent.stoppingDistance = 0f;
        invertFlip = !invertFlip;
        StartCoroutine(Utils.DoAfter(duration, () => {
            isAfraid = false;
            if(spriteRenderer != null)
                spriteRenderer.color = Color.white;
            invertFlip = !invertFlip;
            agent.stoppingDistance = oldBreak;
        }));
	}

}