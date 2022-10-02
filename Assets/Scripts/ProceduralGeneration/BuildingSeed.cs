using UnityEngine.Tilemaps;

[System.Serializable]
public class BuildingSeed {

	public Building buildingPrefab;
	public float lockRadius = 2f;

	public float minX = 2f;
	public float maxX = 4f;

	public int minPerGroup = 1;
	public int maxPerGroup = 1;

	public Tile leftTile;
	public Tile rightTile;
}