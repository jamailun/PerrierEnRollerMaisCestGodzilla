using System.Collections;
using UnityEngine;
using UnityEditor;
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
    [Tooltip("Amont of damages to deal")]
    [SerializeField] private float attackDamages = 10f;

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

    // Melee

    [SerializeIf("enemyType", EnemyType.Melee)]
    [Tooltip("The RAnge required to attack the player")]
    [SerializeField] private float meleeRange = .5f;

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
    [Tooltip("Projectile to shot")]
    [SerializeField] private Projectile projectile_prefab;

    [SerializeIf("enemyType", EnemyType.Distance)]
    [Tooltip("Transform to shot projectiles from")]
    [SerializeField] private Transform projectile_output;

    [Space]

    [Tooltip("The experience prefab to use.")]
    [SerializeField] private ExperienceBall experiencePrefab;

    [Tooltip("The amount of experience to drop")]
    [SerializeField] private ulong droppedExp = 10;

    // -------------------------------------------------------------

    private NavMeshAgent agent;
    private SpriteRenderer spriteRenderer;

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

        if(target == null) {
            var player = FindObjectOfType<PlayerEntity>();
            if(player == null) {
                Debug.LogWarning("Enemy " + name + " could NOT find player...");
                return;
			}
            target = player.transform;
            Recalculate();
		}

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


    private void Recalculate() {
        // Set destination according to the type
        float d = Vector2.Distance(transform.position, target.position);
        switch(enemyType) {

            // Melee always rush to the player
            case EnemyType.Melee:
                if(d <= meleeRange && Time.time >= nextAttackAllowed) {
                    Attack(false);
                    break;
				}
                agent.SetDestination(target.position);
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

        Debug.Log(gameObject.name + " ATTACK !");

        if(distance) {
            nextAttackAllowed += distance_shot_load;
            nextRecalculate += distance_shot_load * 1.1f;

            StartCoroutine(Utils.DoAfter(distance_shot_load * 1.1f, () => agent.isStopped = false));
            StartCoroutine(Cor_AttackDistance());
            return;
        }


	}

    private IEnumerator Cor_AttackDistance() {
        yield return new WaitForSeconds(distance_shot_load);

        if(attackEffect != null) {
            var vfx = Instantiate(attackEffect);
            vfx.transform.SetPositionAndRotation(new Vector3(projectile_output.position.x, projectile_output.position.y, -.1f), Quaternion.identity);
            //SFX ?
        }

        var proj = Instantiate(projectile_prefab);
        proj.Init(projectile_output, target.position - transform.position, transform);
        proj.damages = attackDamages;
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

}