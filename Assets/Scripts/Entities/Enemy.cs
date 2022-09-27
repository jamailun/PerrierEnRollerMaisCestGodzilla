using System.Collections;
using UnityEngine;

// pour faire un truc avec les boss, il faudrait que cette classe soit abstraite avec juste une methode Getter de enemytype;
// et que une sous-class soit genre "IAEnemy" et dont le comportement change en fonction du type.
// Comme ça la classe boss peut override Enemy et override les méthodes qui choississent le comportement.
public class Enemy : LivingEntity {

	[Header("Enemy attributes")]
	[Tooltip("Descries the IA of the Enemy.")]
	[SerializeField] private EnemyType enemyType;

	private void Update() {
		if(enemyType == EnemyType.None)
			return; // pas d'IA => on fait rien :)


	}

}