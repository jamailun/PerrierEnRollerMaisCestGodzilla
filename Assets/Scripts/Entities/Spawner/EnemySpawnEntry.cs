using System.Collections;
using UnityEngine;

[System.Serializable]
public struct EnemySpawnEntry {

	public Enemy enemyPrefab;

	public float minX, maxX;

	public float additionalRadius;

	public float cooldown;
	
}