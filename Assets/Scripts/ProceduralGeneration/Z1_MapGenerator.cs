using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Map_Generator", menuName = "PERMCG/Z1_Map_Generator", order = 10)]
public class Z1_MapGenerator : MapGenerator {

	[Header("Water / Sand transition")]

	[SerializeField] private int waterSizeMin = 2;
	[SerializeField] private int waterSizeMax = 4;
	[SerializeField] [Range(0.01f, 0.5f)] private float waterSandTransitionRight = 0.2f;
	[SerializeField] [Range(0.01f, 0.5f)] private float waterSandTransitionLeft = 0.2f;

	[Header("Sand / Ground transition")]

	[SerializeField] private int sandSizeMin = 6;
	[SerializeField] private int sandSizeMax = 12;
	[SerializeField] [Range(0.01f, 0.5f)] private float sandDirtTransitionRight = 0.2f;
	[SerializeField] [Range(0.01f, 0.5f)] private float sandDirtTransitionLeft = 0.2f;

	[Header("Tiles used in zone 1")]
	[SerializeField] private Tile waterTile;
	[SerializeField] private Tile sandTile;
	[SerializeField] private Tile groundTile;

	[SerializeField] private Tile water_sand_BR;
	[SerializeField] private Tile water_sand_LT;
	[SerializeField] private Tile water_sand_BT;
	[SerializeField] private Tile water_sand_BL;
	[SerializeField] private Tile water_sand_RT;

	[SerializeField] private Tile sand_dirt_BR;
	[SerializeField] private Tile sand_dirt_LT;
	[SerializeField] private Tile sand_dirt_BT;
	[SerializeField] private Tile sand_dirt_BL;
	[SerializeField] private Tile sand_dirt_RT;

	#region local_types_def

	private const int TYPE_NONE = 0;

	private const int TYPE_WATER = 1;

	private const int TYPE_WATER_TO_SAND_BR = 121;
	private const int TYPE_WATER_TO_SAND_LT = 122;
	private const int TYPE_WATER_TO_SAND_BT = 123;
	private const int TYPE_WATER_TO_SAND_BL = 124;
	private const int TYPE_WATER_TO_SAND_RT = 125;

	private const int TYPE_SAND = 2;

	private const int TYPE_SAND_TO_GROUND_BR = 231;
	private const int TYPE_SAND_TO_GROUND_LT = 232;
	private const int TYPE_SAND_TO_GROUND_BT = 233;
	private const int TYPE_SAND_TO_GROUND_BL = 234;
	private const int TYPE_SAND_TO_GROUND_RT = 235;

	private const int TYPE_GROUND = 3;

	private Tile GetTile(int x, int y) {
		return tiles[x, y] switch {
			TYPE_WATER => waterTile,

			TYPE_WATER_TO_SAND_BR => water_sand_BR,
			TYPE_WATER_TO_SAND_LT => water_sand_LT,
			TYPE_WATER_TO_SAND_BT => water_sand_BT,
			TYPE_WATER_TO_SAND_BL => water_sand_BL,
			TYPE_WATER_TO_SAND_RT => water_sand_RT,

			TYPE_SAND => sandTile,

			TYPE_SAND_TO_GROUND_BR => sand_dirt_BR,
			TYPE_SAND_TO_GROUND_LT => sand_dirt_LT,
			TYPE_SAND_TO_GROUND_BT => sand_dirt_BT,
			TYPE_SAND_TO_GROUND_BL => sand_dirt_BL,
			TYPE_SAND_TO_GROUND_RT => sand_dirt_RT,

			TYPE_GROUND => groundTile,

			_ => groundTile
		};
	}

	#endregion

	private int[,] tiles;
	private Vector2 spawn;

	//TODO : Points délimitants la mer (pour le collider) : // private Vector2[] waterPoints;

	public override void Generate() {
		tiles = new int[widthTiles, heightTiles];
		placeds = new();

		// WATER -> SAND TRANSITION
		VerticalTransition(tiles, TYPE_NONE,
			heightTiles, waterSizeMin, waterSizeMax,
			TYPE_WATER, TYPE_SAND, TYPE_WATER_TO_SAND_BR, TYPE_WATER_TO_SAND_LT, TYPE_WATER_TO_SAND_RT, TYPE_WATER_TO_SAND_BL, TYPE_WATER_TO_SAND_BT,
			waterSandTransitionLeft, waterSandTransitionRight
		);

		// Get spawn : on first tile that is NOT water
		int spawnTileY = Random.Range(heightTiles/2 - 3, heightTiles/2 + 3);
		int spawnTileX = 0;
		for(int sx = waterSizeMin; sx < widthTiles; sx++) {
			if(tiles[sx, spawnTileY] != TYPE_WATER) {
				spawnTileX = sx;
				break;
			}
		}
		spawn = new Vector2(spawnTileX, spawnTileY) * sizePerTile;

		// SAND -> GROUND TRANSITION
		VerticalTransition(tiles, TYPE_NONE,
			heightTiles, sandSizeMin, sandSizeMax,
			TYPE_SAND, TYPE_GROUND, TYPE_SAND_TO_GROUND_BR, TYPE_SAND_TO_GROUND_LT, TYPE_SAND_TO_GROUND_RT, TYPE_SAND_TO_GROUND_BL, TYPE_SAND_TO_GROUND_BT,
			sandDirtTransitionLeft, sandDirtTransitionRight
		);

		// DO PONDS
		// little foretss ?
		// random points of sand on the ground ?

		// DO BUILDINGS
		GenerateBuildingsFromSeeds();
	}

	public override void Populate(SceneData scene, bool debug = true) {
		// Fill the tilemap
		for(int x = 0; x < widthTiles; x++) {
			for(int y = 0; y < heightTiles; y++) {
				scene.tilemap.SetTile(new(x, y, 0), GetTile(x,y));
			}
		}

		// Place buildings
		RepopulateBuildings(scene);

		// recalcultate navmesh
		scene.navmesh.BuildNavMeshAsync();

		if(debug) {
			scene.player.transform.position = GetPlayerSpawn();
		}
	}

	public override Vector2 GetPlayerSpawn() {
		return spawn;
	}

}