using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour {
	public static LoadingManager Instance { private set; get; }

	public static bool MainMenuAnimated = false;

	[SerializeField] private int stage = 0;
	public int Stage => stage;
	[SerializeField] private string levelSceneName = "LevelScene";

	[Header("Generators")]
	[SerializeField] private MapGenerator zone_1_generator;
	[SerializeField] private MapGenerator zone_2_generator;
	[SerializeField] private MapGenerator zone_3_generator;
	[SerializeField] private MapGenerator zone_4_generator;

	[Header("Spawn data")]
	[SerializeField] private LevelEnemiesSpawner zone_1_spawns;
	[SerializeField] private LevelEnemiesSpawner zone_2_spawns;
	[SerializeField] private LevelEnemiesSpawner zone_3_spawns;

	[Header("Musics")]
	[SerializeField] private AudioClip[] zone_1_musics;
	[SerializeField] private AudioClip[] zone_2_musics;
	[SerializeField] private AudioClip[] zone_3_musics;
	[SerializeField] private AudioClip[] boss_musics;

	private void Start() {
		if(Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void StartGame() {
		stage = 0;
		NextStage(null);
	}

	public AudioClip PickNewMusic() {
		AudioClip[] clips = stage switch {
			1 => zone_1_musics,
			2 => zone_2_musics,
			3 => zone_3_musics,
			4 => boss_musics,
			_ => new AudioClip[0]
		};
		if(clips.Length == 0)
			return null;
		return clips[Random.Range(0, clips.Length - 1)];
	}

	public static void ResetGameAndGoMenu() {
		if(Instance != null)
			Instance.stage = 0;		
		if(ManagerUI.Instance != null) {
			Destroy(ManagerUI.Instance.gameObject);
		}
		SceneManager.LoadScene(0);
	}

	public LevelEnemiesSpawner GetSpawnData() {
		return stage switch {
			1 => zone_1_spawns,
			2 => zone_2_spawns,
			3 => zone_3_spawns,
			4 => new(),
			_ => throw new System.NotImplementedException("No generator for stage " + stage)
		};
	}

	private MapGenerator GetCurrentGenerator() {
		return stage switch {
			1 => zone_1_generator,
			2 => zone_2_generator,
			3 => zone_3_generator,
			4 => zone_4_generator,
			_ => throw new System.NotImplementedException("No generator for stage " + stage)
		};
	}

	private bool loading = false;

	public void NextStage(PlayerEntity player) {
		if(loading) {
			Debug.LogWarning("Oskour cannot load too much !");
			return;
		}
		loading = true;

		if(player != null)
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

		Debug.Log("Creating the level layout with generator " + generator);
		generator.GenerateSafe();

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
		generator.Populate(data, true);

		// Creating player
		if(player != null) {
			Destroy(data.player.gameObject);
			data.player = player;
		}
		data.player.transform.SetPositionAndRotation(generator.GetPlayerSpawn(), Quaternion.identity);

		// Place exit
		data.exit.transform.SetPositionAndRotation(generator.GetLevelExit(), Quaternion.identity);

		// Unload the previous Scene
		asyncLoad = SceneManager.UnloadSceneAsync("LoadingScreen");

		while(!asyncLoad.isDone) {
			yield return null;
		}

		Time.timeScale = 1f;

		// post load
		TimerUI.StartTimer(Time.time);
		Camera.main.transform.position = new Vector3(data.player.transform.position.x, data.player.transform.position.y, Camera.main.transform.position.z);
		Camera.main.GetComponent<CameraFollow>().target = data.player.transform;
		data.player.UpdateGrow();

		// IF LEVEL 4 ON MET LE BOSS AHAHAHA JE SAIS PAS CODER ET JAI PASLE TEMPS
		if(stage == 4) {
			data.boss.gameObject.SetActive(true);
			data.boss.transform.position = generator.GetBossPos();
		}

		// reset loading
		loading = false;
	}

	public Vector2 CurrentMapDimensions() {
		return GetCurrentGenerator().GetDimensions();
	}

}