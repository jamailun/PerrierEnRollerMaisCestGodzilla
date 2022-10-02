using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class MapGenerator : ScriptableObject {

	[SerializeField] protected float sizePerTile = 2f;
	[SerializeField] protected int widthTiles = 10;
	[SerializeField] protected int heightTiles = 5;

	[Header("Buildings seeds")]
	[SerializeField] protected int triesPerSeed = 3;
	[SerializeField] protected BuildingSeed[] buildings;

	protected List<BuildingSeed.Placed> placeds = new();

	/// <summary>
	/// Generate elements of the zone.
	/// </summary>
	public abstract void Generate();

	/// <summary>
	/// Populate a scene with the generated data.
	/// </summary>
	/// <param name="scene">The scenedata corresponding to the scene to populate.</param>
	public abstract void Populate(SceneData scene, bool debug = true);

	public abstract Vector2 GetPlayerSpawn();

	/// <summary>
	/// Do a tile vertical transition from left to right.
	/// </summary>
	/// <param name="tiles">The tiles array</param>
	/// <param name="empty">The empty tile value</param>
	/// <param name="height">The height of the tiles</param>
	/// <param name="min">the x-min value of the transition</param>
	/// <param name="max">the x-max value of the transition</param>
	/// <param name="left">the tile type on the left of the transition</param>
	/// <param name="right">the tile type on the right of the transition</param>
	/// <param name="br">Transition tile type bottom-right</param>
	/// <param name="lt">Transition tile type left-top</param>
	/// <param name="rt">Transition tile type right-top</param>
	/// <param name="bl">Transition tile type bottom-left</param>
	/// <param name="bt">Transition tile type bottom-top</param>
	/// <param name="leftP">The probablity of the transition to slide on the left at every point</param>
	/// <param name="rightP">The probablity of the transition to slide on the right at every point</param>
	protected void VerticalTransition(int[,] tiles, int empty, int height, int min, int max, int left, int right, int br, int lt, int rt, int bl, int bt, float leftP, float rightP) {
		int tx = Random.Range(min, max);
		bool previousRight = false;
		bool previousLeft = false;
		for(int y = 0; y < height; y++) {

			bool toRight = false;
			bool toLeft = false;

			// Try to go to right
			if(tx < max && Random.value <= rightP && !previousLeft) {
				toRight = true;
				// Try to go to left
			} else if(tx > min && Random.value <= leftP && !previousRight) {
				toLeft = true;
			}

			previousLeft = previousRight = false;

			// fill left elem to the transition
			for(int x = 0; x < tx - 1; x++) {
				if(tiles[x, y] == empty)
					tiles[x, y] = left;
			}

			// do the transition
			if(toRight) {
				tiles[tx - 1, y] = br;
				tiles[tx, y] = lt;
				tx++;
				previousRight = true;
			} else if(toLeft) {
				tx--;
				tiles[tx - 1, y] = rt;
				tiles[tx, y] = bl;
				previousLeft = true;
			} else {
				tiles[tx - 1, y] = bt;
				tiles[tx, y] = right;
			}

		}
	}

	/// <summary>
	/// Generate buildings from the seeds parameters
	/// </summary>
	protected void GenerateBuildingsFromSeeds() {
		foreach(var seed in buildings) {
			int placedsAmount = 0;
			int tentative = 0;
			while(tentative < triesPerSeed) {
				float radius = seed.lockRadius * 0.8f;
				float tx = sizePerTile * Random.Range(seed.minX + radius, seed.maxX - radius);
				float ty = sizePerTile * Random.Range(0 + radius, heightTiles - radius);
				var center = new Vector2(tx, ty);

				if(CanPlaceBuildingHere(center, radius)) {
					// place some buildings, in a circle
					int toPlace = Random.Range(seed.minPerGroup, seed.maxPerGroup);
					for(int n = 0; n < toPlace; n++) {
						Vector2 pos = (Random.insideUnitCircle * radius) + center;
						placeds.Add(new BuildingSeed.Placed(seed, pos));
						placedsAmount++;
					}
					if(placedsAmount > seed.optimalAmount * 1.1f)
						break;
				} else {
					tentative++;
				}
			}
		}
		Debug.Log("Created " + placeds.Count + " buidings.");
	}

	/// <summary>
	/// Test if a building can be placed at a point
	/// </summary>
	/// <param name="pos">The point to try to put the building at</param>
	/// <param name="radius">The radius of the building</param>
	/// <returns></returns>
	protected bool CanPlaceBuildingHere(Vector2 pos, float radius) {
		foreach(var b in placeds) {
			if(Vector2.Distance(new Vector2(b.x, b.y), pos) < Mathf.Max(radius, b.radius))
				return true;
		}
		return true;
	}

	/// <summary>
	/// Remove old building and place new ones on the navmesh
	/// </summary>
	/// <param name="scene">The SceneData containing the navmesh</param>
	/// <param name="buildings">The placed buildings list</param>
	protected void RepopulateBuildings(SceneData scene) {
		// Remove buildings
		var tempList = scene.navmesh.transform.Cast<Transform>().ToList();
		foreach(var child in tempList) {
			if(child.gameObject.GetComponent<Building>() != null)
				DestroyImmediate(child.gameObject);
		}

		// place buildings entities
		foreach(var placed in placeds) {
			var b = GameObject.Instantiate(placed.building, scene.navmesh.transform);
			b.transform.SetPositionAndRotation(new Vector2(placed.x, placed.y), Quaternion.identity);
		}
	}

}