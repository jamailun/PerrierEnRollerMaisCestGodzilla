using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// pour faire un truc avec les boss, il faudrait que cette classe soit abstraite avec juste une methode Getter de enemytype;
// et que une sous-class soit genre "IAEnemy" et dont le comportement change en fonction du type.
// Comme ça la classe boss peut override Enemy et override les méthodes qui choississent le comportement.
public class Enemy : LivingEntity {

	[Header("Enemy attributes")]
	[Tooltip("Descries the IA of the Enemy.")]
	[SerializeField] private EnemyType enemyType;

    private NavMeshAgent agent;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Transform target;
    [Tooltip("Time between recalculation (in seconds).")]
    [SerializeField] private float recalculateAfter = 0.2f;

    [SerializeField] private float distance_wanted = 2.2f;
    [SerializeField] private float distance_epsilon = .3f;

    private float nextRecalculate; // next time to recalulate trajectory

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
        switch(enemyType) {

            // Melee always rush to the player
            case EnemyType.Melee:
                agent.SetDestination(target.position);
                break;

            case EnemyType.Distance:
                float d = Vector2.Distance(transform.position, target.position);
                if((d < distance_wanted - distance_epsilon) || (d > distance_wanted + distance_epsilon)) {
                    var vec = (transform.position - target.position);
                    var movement = vec.normalized * (distance_wanted - d);

                    agent.SetDestination(transform.position + movement);
				}
                break;

        }

        // Change recalculation.
        nextRecalculate = Time.time + recalculateAfter;
    }

}