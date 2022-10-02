using UnityEngine.Tilemaps;

[System.Serializable]
public class BuildingSeed {

	public Building buildingPrefab;
	public float lockRadius = 2f;

	public float minX = 2f;
	public float maxX = 4f;

	public int minPerGroup = 1;
	public int maxPerGroup = 1;

	public int optimalAmount = 15;

	public struct Placed {
		public Building building;
		public float x, y;
		public float radius;

		public Placed(BuildingSeed seed, UnityEngine.Vector2 pos) {
			building = seed.buildingPrefab;
			radius = seed.lockRadius;
			x = pos.x;
			y = pos.y;
		}
	}

}