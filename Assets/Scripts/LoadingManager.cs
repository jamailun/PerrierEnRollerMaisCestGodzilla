using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour {
	public static LoadingManager Instance { private set; get; }

	[SerializeField] private int stage = 0;
	[SerializeField] private string levelSceneName = "LevelScene";
	[SerializeField] private PlayerEntity playerPrefab;

	[Header("Generators")]
	[SerializeField] private MapGenerator zone_1_generator;
	[SerializeField] private MapGenerator zone_2_generator;

	private void Start() {
		if(Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
		if(stage == 0)
			stage = 1;
#endif
	}

	public static void ResetGameAndGoMenu() {
		if(Instance != null)
			Instance.stage = 0;		
		if(ManagerUI.Instance != null) {
			Destroy(ManagerUI.Instance.gameObject);
		}
		SceneManager.LoadScene(0);
	}

	private MapGenerator GetCurrentGenerator() {
		return stage switch {
			1 => zone_1_generator,
			2 => zone_2_generator,
			_ => throw new System.NotImplementedException("NO generator for stage " + stage)
		};
	}

	public void NextStage(PlayerEntity player) {
		DontDestroyOnLoad(player.gameObject);
		StartCoroutine(PreloadAsync(player));
	}

	private IEnumerator PreloadAsync(PlayerEntity player) {

		var loading = SceneManager.LoadSceneAsync("LoadingScreen");
		while(!loading.isDone) {
			yield return null;
		}

		stage++;
		Debug.Log("Preparing stage " + stage + ". Creating procedural generator...");
		MapGenerator generator = GetCurrentGenerator();

		Debug.Log("Creating the level layout...");
		generator.Generate();

		Debug.Log("Creating scene...");
		StartCoroutine(LoadAsyncScene(generator, player));

	}

	private IEnumerator LoadAsyncScene(MapGenerator generator, PlayerEntity player) {

		// Premier player = il faut l'init.
		PlayerEntity.shouldStart = player == null;

		// The Application loads the Scene in the background at the same time as the current Scene.
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelSceneName, LoadSceneMode.Additive);

		// Wait until the last operation fully loads to return anything
		while(!asyncLoad.isDone) {
			yield return null;
		}

		var target = SceneManager.GetSceneByName(levelSceneName);

		// Creating tilemap
		var data = new SceneData(target);
		generator.Populate(data, false);

		// Creating player
		if(player != null) {
			Destroy(data.player.gameObject);
			data.player = player;
		}
		data.player.transform.SetPositionAndRotation(generator.GetPlayerSpawn(), Quaternion.identity);

		// Unload the previous Scene
		asyncLoad = SceneManager.UnloadSceneAsync("LoadingScreen");

		while(!asyncLoad.isDone) {
			yield return null;
		}

		Time.timeScale = 1f;

		// post load
		TimerUI.UnpauseTimer();
		Camera.main.transform.position = new Vector3(data.player.transform.position.x, data.player.transform.position.y, Camera.main.transform.position.z);
		Camera.main.GetComponent<CameraFollow>().target = data.player.transform;
	}

	public Vector2 CurrentMapDimensions() {
		return GetCurrentGenerator().GetDimensions();
	}

}