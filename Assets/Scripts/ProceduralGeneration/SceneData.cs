using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

public struct SceneData {

	public NavMeshSurface2d navmesh;
	public Tilemap tilemap;
	public PlayerEntity player;
	public EntitiesOrigin origin;

	public SceneData(Scene scene) {
		navmesh = null;
		tilemap = null;
		player = null;
		origin = null;

		foreach(var obj in scene.GetRootGameObjects()) {
			if(navmesh == null && obj.GetComponent<NavMeshSurface2d>() != null) {
				navmesh = obj.GetComponent<NavMeshSurface2d>();
				tilemap = obj.GetComponentInChildren<Tilemap>();
			}
			else if(player == null && obj.GetComponent<PlayerEntity>() != null) {
				player = obj.GetComponent<PlayerEntity>();
			}
			else if(origin == null && obj.GetComponent<EntitiesOrigin>() != null) {
				origin = obj.GetComponent<EntitiesOrigin>();
			}
		}
	}

	public bool IsValid { get { return navmesh != null && tilemap != null && player != null && origin != null; } }

}