using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProceduralTest : MonoBehaviour {

	[SerializeField] private MapGenerator generator;

	private SceneData GetSceneData() {
		var scene = SceneManager.GetActiveScene();
		Debug.Log("Active scene = \"" + scene.name + "\".");
		var data = new SceneData(scene);
		if(!data.IsValid) {
			Debug.LogError("Could not find every elements for the scene data.");
			Debug.LogError("navmesh = " + data.navmesh);
			Debug.LogError("tilemap = " + data.tilemap);
			Debug.LogError("player = " + data.player);
			Debug.LogError("borders = " + data.borders);
			throw new System.Exception("Scene data incomplete");
		}
		return data;
	}

	public void GenerateAndPopulate() {
		var data = GetSceneData();

		generator?.Generate();

		generator?.Populate(data);
	}
	
	public void ClearBuildings() {
		var data = GetSceneData();
		// Remove buildings
		var tempList = data.navmesh.transform.Cast<Transform>().ToList();
		foreach(var child in tempList) {
			if(child.gameObject.GetComponent<Building>() != null)
				DestroyImmediate(child.gameObject);
		}
	}

}