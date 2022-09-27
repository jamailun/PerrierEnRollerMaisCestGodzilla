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

        // Change target
        if(Time.time >= nextRecalculate)
            Recalculate();

        // Change direction
        spriteRenderer.flipX = target.position.x < transform.position.x;

    }
    private void Recalculate() {
        // Set destination
        agent.SetDestination(target.position);

        // Change recalculation.
        nextRecalculate = Time.time + recalculateAfter;
    }

}