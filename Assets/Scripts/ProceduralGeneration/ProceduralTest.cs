using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProceduralTest : MonoBehaviour {

	[SerializeField] private MapGenerator generator;

	public void Generate() {
		var scene = SceneManager.GetActiveScene();
		Debug.Log("Active scene = \"" + scene.name + "\".");
		var data = new SceneData(scene);
		if( ! data.IsValid) {
			Debug.LogError("Could not find every elements for the scene data.");
			Debug.LogError("navmesh = " + data.navmesh);
			Debug.LogError("tilemap = " + data.tilemap);
			Debug.LogError("player = " + data.player);
			Debug.LogError("origin = " + data.origin);
			return;
		}

		generator?.Generate();

		generator?.Populate(data);
	}
	
}