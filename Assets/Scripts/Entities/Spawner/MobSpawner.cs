using UnityEngine;

public class MobSpawner : MonoBehaviour {

	[SerializeField] public LevelEnemiesSpawner spawnData;

	private float nextSpawn = 1f;

	private void Update() {
		if(spawnData == null)
			return;

		// main wave
		if(Time.time >= nextSpawn) {
			nextSpawn = Time.time + spawnData.NextCooldown();

			float spawnedPower = 0;

			var DIM = LoadingManager.Instance.CurrentMapDimensions();

			var center = CameraCenter;
			float minX = CameraLeft;
			float maxX = CameraRight;
			float mult = DifficultyDisplayer.GetDifficultyMultiplier();

			spawnData.GenerateEntries(minX, maxX);

			float requiredPower = mult * (spawnData.baseRequiredPower + (spawnData.basePowerHorizontal * center.x));

			Debug.Log("New wave. Difficulty is " + DifficultyDisplayer.GetDifficultyMultiplier() + "=> power to spawn = " + requiredPower);

			while(spawnedPower < requiredPower) {
				var entry = spawnData.GetRandomEntry();
				if( ! entry.Valid) {
					Debug.Log("No possible spawn. cancel this wave.");
					break;
				}
				int nextSpawn = entry.NextSpawnAmount();
				if(nextSpawn < 1) {
					spawnedPower += 1f;
					Debug.LogWarning("CAREFUL ! No min/max spawn set for entry " + entry.enemyPrefab.name + " !");
				}

				//Debug.Log("TRY entry "+entry.enemyPrefab.name + " x " + nextSpawn);

				for(int n = 0; n < nextSpawn && spawnedPower < requiredPower; n++) {
					var pos = center + (Random.insideUnitCircle.normalized * ((maxX - minX)/2f + entry.additionalRadius));

					if(pos.x < 0 || pos.x > DIM.x || pos.y < 0 || pos.y > DIM.y) {
						spawnedPower += Mathf.Max(0.1f, entry.power);
						continue;
					}

					var mob = Instantiate(entry.enemyPrefab);
					mob.transform.SetPositionAndRotation(pos, Quaternion.identity);
					mob.RefreshTarget();

					if(entry.power < 0.1f) {
						spawnedPower += 1f;
						Debug.LogWarning("CAREFULL no power set for entry " + entry.enemyPrefab.name);
					}
					spawnedPower += entry.power;
				}
			}

		}
	}

	private Vector2 CameraCenter => Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2));
	private float CameraLeft => Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x;
	private float CameraRight => Camera.main.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x;

}