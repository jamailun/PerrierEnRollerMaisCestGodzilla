using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelEnemySpawner", menuName = "PERMCG/Level Enemy Spawner", order = 8)]
public class LevelEnemiesSpawner : ScriptableObject {

	[SerializeField] private EnemySpawnEntry[] enemies;
	public EnemySpawnEntry[] Enemies => enemies;

	[SerializeField] private float minWaveCooldown = 8f;
	[SerializeField] private float maxWaveCooldown = 12f;
	public float baseRequiredPower = 5;
	public float basePowerHorizontal = 1.002f;

	public float NextCooldown() {
		return Random.Range(minWaveCooldown, maxWaveCooldown);
	}


	private readonly List<EnemySpawnEntry> entries = new();
	private float totalPriority;

	public void GenerateEntries(float minX, float maxX) {
		entries.Clear();
		totalPriority = 0f;
		foreach(var entry in enemies) {
			bool cannotSpawn = minX > entry.maxX || maxX < entry.minX;
			if(!cannotSpawn) {
				entries.Add(entry);
				totalPriority += entry.spawnPriority;
			}
		}
	}

	public EnemySpawnEntry GetRandomEntry() {
		if(entries.Count == 0)
			return new(); // invalid struct

		entries.Shuffle();
		float rand = Random.Range(0f, totalPriority);
		foreach(var entry in entries) {
			if(rand >= entry.spawnPriority)
				return entry;
			rand -= entry.spawnPriority;
		}
		// worst case
		return entries[0];

	}

}