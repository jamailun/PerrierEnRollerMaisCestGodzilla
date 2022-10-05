using UnityEngine;

[System.Serializable]
public struct EnemySpawnEntry {

	public Enemy enemyPrefab;

	public float minX, maxX;
	public int minSpawn, maxSpawn;

	public float spawnPriority;

	public float additionalRadius;

	public float power;

	public bool Valid => enemyPrefab != null;

	public int NextSpawnAmount() {
		return Random.Range(minSpawn, maxSpawn);
	}
	
}