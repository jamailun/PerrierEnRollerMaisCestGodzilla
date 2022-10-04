using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelEnemySpawner", menuName = "PERMCG/Level Enemy Spawner", order = 8)]
public class LevelEnemiesSpawner : ScriptableObject {

	public EnemySpawnEntry[] enemies;

	public float minWaveCooldown;

	public float maxWaveCooldown;

	public float NextCooldown() {
		return Random.Range(minWaveCooldown, maxWaveCooldown);
	}

}