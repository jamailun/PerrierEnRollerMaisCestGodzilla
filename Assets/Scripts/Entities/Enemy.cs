using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// pour faire un truc avec les boss, il faudrait que cette classe soit abstraite avec juste une methode Getter de enemytype;
// et que une sous-class soit genre "IAEnemy" et dont le comportement change en fonction du type.
// Comme ça la classe boss peut override Enemy et override les méthodes qui choississent le comportement.
public class Enemy : LivingEntity {

	[Header("Enemy attributes")]
	[Tooltip("Describes the IA of the Enemy.")]
	[SerializeField] private EnemyType enemyType;

    // -------------------------------------------------------------

    [Space]

    [SerializeIf("enemyType", EnemyType.None, ComparisonType.NotEqual)]
    [SerializeField] private Transform target;
    [SerializeIf("enemyType", EnemyType.None, ComparisonType.NotEqual)]
    [Tooltip("Time between recalculation (in seconds).")]
    [SerializeField] private float recalculateAfter = 0.2f;

    [SerializeIf("enemyType", EnemyType.None, ComparisonType.NotEqual)]
    [Tooltip("Time between two attacks of an enemy, in seconds")]
    [SerializeField] private float attackSpeed = 1f;

    [SerializeIf("enemyType", EnemyType.None, ComparisonType.NotEqual)]
    [Tooltip("Percentage of intentional error between two attacks")]
    [Range(0f, 0.5f)]
    [SerializeField] private float attackSpeedEpsilon = 0.05f;

    [SerializeIf("enemyType", EnemyType.None, ComparisonType.NotEqual)]
    [Tooltip("Attack effect on shot")]
    [SerializeField] private ParticleSystem attackEffect;

    [SerializeIf("enemyType", EnemyType.None, ComparisonType.NotEqual)]
    [Tooltip("Transform to shot attacks from")]
    [SerializeField] private Transform attack_output;

    [SerializeIf("enemyType", EnemyType.None, ComparisonType.NotEqual)]
    [Tooltip("Geometric scale for attacks")]
    [SerializeField] private float attackScale = 1f;

    // Melee

    [SerializeIf("enemyType", EnemyType.Melee)]
    [Tooltip("The range required to attack the player")]
    [SerializeField] private float meleeRange = .5f;

    [SerializeIf("enemyType", EnemyType.Melee)]
    [Tooltip("The duration of the attack")]
    [SerializeField] private float meleeAttackDuration = .1f;

    [SerializeIf("enemyType", EnemyType.Melee)]
    [Tooltip("The hitbox to spawn to attack")]
    [SerializeField] private Hitbox meleeAttackPrefab_side;

    [SerializeIf("enemyType", EnemyType.Melee)]
    [Tooltip("Index on the animation to spawn the hitbox. IF DEFINED !!")]
    [SerializeField] private int melee_animation_spawn = -1;

    // Distance

    [SerializeIf("enemyType", EnemyType.Distance)]
    [Tooltip("The distance exact to keep from the target")]
    [SerializeField] private float distance_wanted = 2.2f;
    
    [SerializeIf("enemyType", EnemyType.Distance)]
    [Tooltip("The allowed error of distance to keep")]
    [SerializeField] private float distance_epsilon = .3f;

    [SerializeIf("enemyType", EnemyType.Distance)]
    [Tooltip("Time before shoot")]
    [SerializeField] private float distance_shot_load = .8f;

    [SerializeIf("enemyType", EnemyType.Distance)]
    [Tooltip("Time waiting after shoot")]
    [SerializeField] private float distance_shot_postload = 0f;

    [SerializeIf("enemyType", EnemyType.Distance)]
    [Tooltip("Index on the animation to spawn the projectile. IF DEFINED !!")]
    [SerializeField] private int distance_animation_spawn = -1;

    [SerializeIf("enemyType", EnemyType.Distance)]
    [Tooltip("Projectile to shot")]
    [SerializeField] private Projectile projectile_prefab;

    [Space]

    [Tooltip("The experience prefab to use.")]
    [SerializeField] private ExperienceBall experiencePrefab;

    [Tooltip("The amount of experience to drop")]
    [SerializeField] private ulong droppedExp = 10;

    // -------------------------------------------------------------

    [Header("Animation")]

    [SerializeField] private float walkAnimationScale = 1f;
    [SerializeField] private CustomAnimation walkAnimation;
    [SerializeField] private float attackAnimationScale = 1f;
    [SerializeField] private CustomAnimation attackAnimation;

    private NavMeshAgent agent;
    private SpriteRenderer spriteRenderer;
    private CustomAnimator animator;

    private float nextRecalculate; // next time to recalulate trajectory
    private float nextAttackAllowed; // prochaine attaque.

    private void Start() {
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

        if(target == null) {
            RefreshTarget();
            Recalculate();
        }
    }

    private const string ANIM_WALK = "anim_walk";
    private const string ANIM_ATTACK = "anim_attack";

    private void PlayAnimationWalk() {
        spriteRenderer.transform.localScale = new Vector3(walkAnimationScale, walkAnimationScale, 1f);
        //Debug.Log("(Enemy "+gameObject.name+") scale du renderer = " + walkAnimationScale);
        if(enemyType == EnemyType.Melee || enemyType == EnemyType.Distance)
            animator.PlayConditional(ANIM_WALK, PredicateWalkAnimation, walkAnimation.GetFirst());
        else
            animator.Play(ANIM_WALK);
    }

    private void PlayAttackTemp(float duration) {
        spriteRenderer.transform.localScale = new Vector3(attackAnimationScale, attackAnimationScale, 1f);
        //Debug.Log("(Enemy " + gameObject.name + ") scale du renderer = " + walkAnimationScale);

        animator.PlayOnce(ANIM_ATTACK, duration, ANIM_WALK, walkAnimationScale);
	}

    private bool PredicateWalkAnimation() {
        if(target == null)
            return false;
        return agent.remainingDistance > 0.05f + agent.stoppingDistance;
	}

    private void Update() {
		if(enemyType == EnemyType.None || null == target)
			return; // pas d'IA => on fait rien :)

        // Change direction
        spriteRenderer.flipX = target.position.x < transform.position.x;

        // Change target
        if(Time.time >= nextRecalculate)
            Recalculate();
    }

    private bool IsFlip() {
        return spriteRenderer.flipX;
    }

    private void Recalculate() {
        // Set destination according to the type
        float d = Vector2.Distance(transform.position, target.position);
        switch(enemyType) {

            // Melee always rush to the player
            case EnemyType.Melee:
                if(d <= meleeRange && Time.time >= nextAttackAllowed) {
                    Attack(false);
                    break;
				} else {
                    agent.SetDestination(target.position);
                }
                break;

            case EnemyType.Distance:
                if((d < distance_wanted - distance_epsilon) || (d > distance_wanted + distance_epsilon)) {
                    var vec = (transform.position - target.position);
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

    private void Attack(bool distance) {
        nextAttackAllowed = Time.time + (attackSpeed * (1f + Random.Range(-attackSpeedEpsilon, attackSpeedEpsilon)));

        agent.isStopped = true;

        //Debug.Log(gameObject.name + " ATTACK !");

        // DISTANCE
        if(distance) {
            nextAttackAllowed += distance_shot_load;
            nextRecalculate += distance_shot_load * 1.1f;

            StartCoroutine(Utils.DoAfter(distance_shot_load + distance_shot_postload, () => agent.isStopped = false));
            if(distance_animation_spawn >= 0) {
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
                SpawnHitboxMelee(melee_animation_spawn / (float)animator.GetClipSize(ANIM_ATTACK));
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

    private void SpawnHitboxMelee(float durationPer) {
        var source = new Vector3(attack_output.position.x, attack_output.position.y, -.1f);

        AttackEffect(source);

        var hitbox = Instantiate(meleeAttackPrefab_side);
        hitbox.transform.position = source;
        hitbox.transform.localScale = new Vector3(attackScale, attackScale, 1f);
        hitbox.Spawn(_flatDamages, meleeAttackDuration * durationPer, IsFlip(), transform);
    }

    private Vector3 GetOutput() {
        var source = new Vector3(attack_output.position.x, attack_output.position.y, -.1f);
        if(IsFlip()) {
            source.x -= 2 * attack_output.localPosition.x;
        }
        return source;
    }

    // Play the effects of the attack
    private void AttackEffect(Vector3 source) {
        if(attackEffect != null) {
            var vfx = Instantiate(attackEffect);
            vfx.transform.SetPositionAndRotation(source, Quaternion.identity);
            //TODO SFX
        }
    }

    private IEnumerator Cor_AttackDistance() {
        PlayAttackTemp(distance_shot_load);

        Debug.Log("(Enemy "+gameObject.name+") shot wait.");
        yield return new WaitForSeconds(distance_shot_load);

        SpawnProjectile();

        Debug.Log("(Enemy " + gameObject.name + ") shot over.");
    }

    private void SpawnProjectile() {
        // Calculate
        var source = GetOutput();

        // SFX
        AttackEffect(source);

        // Projectil
        var proj = Instantiate(projectile_prefab);
        proj.Init(source, target.position - transform.position, transform);
        proj.transform.localScale = new Vector3(attackScale, attackScale, 1f);
        proj.damages = _flatDamages;
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
        target = player.transform;
    }

}