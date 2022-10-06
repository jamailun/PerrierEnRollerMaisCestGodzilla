using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour {
	public static LoadingManager Instance { private set; get; }

	[SerializeField] private int stage = 0;
	[SerializeField] private string levelSceneName = "LevelScene";
	[SerializeField] private GameObject testPrefab;

	[Header("Generators")]
	[SerializeField] private MapGenerator zone_1_generator;

	private void Start() {
		if(Instance != null) {
			Debug.LogWarning("Cannot have multiple LoadingManager. Removing this one.");
			Destroy(this);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
		if(stage == 0)
			stage = 1;
#endif
	}

	public static void Reset() {
		if(Instance != null)
			Instance.stage = 0;		
	}

	private MapGenerator GetCurrentGenerator() {
		return stage switch {
			1 => zone_1_generator,
			_ => throw new System.NotImplementedException("NO generator for stage " + stage)
		};
	}

	public void NextStage() {
		Time.timeScale = 0f;

		stage++;
		Debug.Log("Preparing stage " + stage + ".");

		Debug.Log("Creating procedural generator...");
		MapGenerator generator = GetCurrentGenerator();

		Debug.Log("Creating the level layout...");
		generator.Generate();

		Debug.Log("Creating scene...");
		StartCoroutine(LoadAsyncScene(generator));
	}

	private IEnumerator LoadAsyncScene(MapGenerator generator) {
		// The Application loads the Scene in the background at the same time as the current Scene.
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelSceneName, LoadSceneMode.Additive);

		// Wait until the last operation fully loads to return anything
		while(!asyncLoad.isDone) {
			yield return null;
		}

		var target = SceneManager.GetSceneByName(levelSceneName);

		// Creating tilemap
		generator.Populate(new SceneData(target), false);

		// Creating entities
		var test = Instantiate(testPrefab);
		SceneManager.MoveGameObjectToScene(test, target);

		// Unload the previous Scene
		SceneManager.UnloadSceneAsync("LoadingScreen");

		Time.timeScale = 1f;
	}

	public Vector2 CurrentMapDimensions() {
		return GetCurrentGenerator().GetDimensions();
	}

}