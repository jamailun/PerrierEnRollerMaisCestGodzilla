using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

public struct SceneData {

	public NavMeshSurface2d navmesh;
	public Tilemap tilemap;
	public PlayerEntity player;
	public WorldBorder borders;
	public ExitPoint exit;
	public Boss boss;

	public SceneData(Scene scene) {
		navmesh = null;
		tilemap = null;
		player = null;
		borders = null;
		exit = null;
		boss = null;

		foreach(var obj in scene.GetRootGameObjects()) {
			if(navmesh == null && obj.GetComponent<NavMeshSurface2d>() != null) {
				navmesh = obj.GetComponent<NavMeshSurface2d>();
				tilemap = obj.GetComponentInChildren<Tilemap>();
			}
			else if(player == null && obj.GetComponent<PlayerEntity>() != null) {
				player = obj.GetComponent<PlayerEntity>();
			}
			else if(borders == null && obj.GetComponent<WorldBorder>() != null) {
				borders = obj.GetComponent<WorldBorder>();
			}
			else if(exit == null && obj.GetComponent<ExitPoint>() != null) {
				exit = obj.GetComponent<ExitPoint>();
			}
			else if(boss == null && obj.GetComponent<Boss>() != null) {
				boss = obj.GetComponent<Boss>();
			}
		}
	}

	public bool IsValid { get { return navmesh != null && tilemap != null && player != null && borders != null && exit != null && boss != null; } }

}