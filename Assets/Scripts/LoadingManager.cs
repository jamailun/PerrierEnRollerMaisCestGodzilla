using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour {
	public static LoadingManager Instance { private set; get; }

	[SerializeField] private int stage = 0;
	[SerializeField] private string levelSceneName = "LevelScene";
	[SerializeField] private GameObject testPrefab;

	private void Start() {
		if(Instance != null) {
			Debug.LogWarning("Cannot have multiple LoadingManager. Removing this one.");
			Destroy(this);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void Reset() {
		stage = 0;		
	}

	public void NextStage() {
		stage++;
		Debug.Log("Preparing stage " + stage + ".");

		Debug.Log("Creating procedural generator...");
		//TODO ProceduralGenerator generator = new ProceduralGenerator();

		Debug.Log("Creating the level layout...");
		//TODO Texture2D layout  = generator.GenerateTexture(w, h);

		Debug.Log("Creating scene...");
		StartCoroutine(LoadAsyncScene());
	}

	private IEnumerator LoadAsyncScene(/*Texture2D layout*/) {
		// The Application loads the Scene in the background at the same time as the current Scene.
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelSceneName, LoadSceneMode.Additive);

		// Wait until the last operation fully loads to return anything
		while(!asyncLoad.isDone) {
			yield return null;
		}

		// TODO : IMPLEMENT LAYOUT !
		// TODO : create ENEMIES !

		var test = Instantiate(testPrefab);
		SceneManager.MoveGameObjectToScene(test, SceneManager.GetSceneByName(levelSceneName));

		// Unload the previous Scene
		SceneManager.UnloadSceneAsync("LoadingScreen");
	}

}