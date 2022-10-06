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

	[Header("Road parameters")]

	[SerializeField] private int firstRoadX = 20;
	[SerializeField] private int distanceBetweenRoadsMin = 7;
	[SerializeField] private int distanceBetweenRoadsMax = 12;
	[SerializeField] private float chanceAdditionalRoad = 0.8f;

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

	[SerializeField] private Tile roadTile;
	[SerializeField] private Tile road_to_grass_L;
	[SerializeField] private Tile road_to_grass_R;
	[SerializeField] private Tile road_to_grass_T;
	[SerializeField] private Tile road_to_grass_B;
	[SerializeField] private Tile road_to_grass_BR;
	[SerializeField] private Tile road_to_grass_BL;
	[SerializeField] private Tile road_to_grass_TR;
	[SerializeField] private Tile road_to_grass_TL;
	[SerializeField] private Tile road_to_grass_nBR;
	[SerializeField] private Tile road_to_grass_nBL;
	[SerializeField] private Tile road_to_grass_nTR;
	[SerializeField] private Tile road_to_grass_nTL;

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

	private const int TYPE_GROUND_TO_ROAD_L = 3401;
	private const int TYPE_GROUND_TO_ROAD_R = 3402;
	private const int TYPE_GROUND_TO_ROAD_T = 3403;
	private const int TYPE_GROUND_TO_ROAD_B = 3404;
	private const int TYPE_GROUND_TO_ROAD_BR = 3411;
	private const int TYPE_GROUND_TO_ROAD_BL = 3412;
	private const int TYPE_GROUND_TO_ROAD_TR = 3413;
	private const int TYPE_GROUND_TO_ROAD_TL = 3414;
	private const int TYPE_GROUND_TO_ROAD_nBR = 3421;
	private const int TYPE_GROUND_TO_ROAD_nBL = 3422;
	private const int TYPE_GROUND_TO_ROAD_nTR = 3423;
	private const int TYPE_GROUND_TO_ROAD_nTL = 3424;

	private const int TYPE_ROAD = 4;

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

			TYPE_GROUND_TO_ROAD_L => road_to_grass_L,
			TYPE_GROUND_TO_ROAD_R => road_to_grass_R,
			TYPE_GROUND_TO_ROAD_B => road_to_grass_B,
			TYPE_GROUND_TO_ROAD_T => road_to_grass_T,

			TYPE_GROUND_TO_ROAD_BR => road_to_grass_BR,
			TYPE_GROUND_TO_ROAD_TR => road_to_grass_TR,
			TYPE_GROUND_TO_ROAD_BL => road_to_grass_BL,
			TYPE_GROUND_TO_ROAD_TL => road_to_grass_TL,

			TYPE_GROUND_TO_ROAD_nBR => road_to_grass_nBR,
			TYPE_GROUND_TO_ROAD_nTR => road_to_grass_nTR,
			TYPE_GROUND_TO_ROAD_nBL => road_to_grass_nBL,
			TYPE_GROUND_TO_ROAD_nTL => road_to_grass_nTL,

			TYPE_ROAD => roadTile,

			_ => groundTile
		};
	}

	#endregion

	private Vector2 spawn, exit;

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
		spawn = new Vector2(spawnTileX + 1.5f, spawnTileY) * sizePerTile;

		// SAND -> GROUND TRANSITION
		VerticalTransition(tiles, TYPE_NONE,
			heightTiles, sandSizeMin, sandSizeMax,
			TYPE_SAND, TYPE_GROUND, TYPE_SAND_TO_GROUND_BR, TYPE_SAND_TO_GROUND_LT, TYPE_SAND_TO_GROUND_RT, TYPE_SAND_TO_GROUND_BL, TYPE_SAND_TO_GROUND_BT,
			sandDirtTransitionLeft, sandDirtTransitionRight
		);

		// Roads
		int verticalAmount = 2;

		int previousX = -1;
		int previousYbot = -1;
		int previousYtop = -1;

		int roadX = firstRoadX;

		int twentyPercent = heightTiles / 5;

		bool foundSmall = false;
		for(int r = 0; r < verticalAmount; r++) {
			bool isSmall = false;
			if(!foundSmall && (r == verticalAmount - 1 || Random.value <= 0.5))
				isSmall = true;

			roadX += Random.Range(distanceBetweenRoadsMin, distanceBetweenRoadsMax);

			int roadYtop;
			int roadYbot;

			if(isSmall) {
				roadYtop = Random.Range(4 * twentyPercent,  heightTiles - 2);        //	80% -> 100%
				roadYbot = Random.Range(1 * twentyPercent, 2 * twentyPercent);      //	20% ->  40%
			} else {
				roadYtop = Random.Range(3 * twentyPercent, 4 * twentyPercent);      //  60% ->  80%
				roadYbot = Random.Range(		2		 , 1 * twentyPercent);      //   0% ->  20%
			}

			//Debug.Log("road " + (r + 1) + " from (" + roadX + "," + roadYtop + ") to (" + roadX + "," + roadYbot + ").");

			// build this road
			BuildVerticalRoad(roadX, roadYtop, roadYbot);

			if(isSmall) {
				foundSmall = true;
			}

			if(previousYbot != -1) {
				if(isSmall) {
					BuildHorizontalRoadToRight(previousYtop, previousX);
					BuildHorizontalRoadToRight(roadYbot, previousX);
					// additional road from top
					if(Mathf.Abs(roadYtop - previousYtop) > 6 && Random.value <= chanceAdditionalRoad) {
						BuildHorizontalRoadToRight(roadYtop, roadX);
					}
				} else {
					BuildHorizontalRoadToRight(previousYbot, previousX);
					BuildHorizontalRoadToRight(roadYtop, previousX);
					// additional road from bottom
					if(Mathf.Abs(roadYbot - previousYbot) > 6 && Random.value <= chanceAdditionalRoad) {
						BuildHorizontalRoadToRight(roadYbot, roadX);
					}
				}

				// Central road.
				int exitRoad = Random.Range(2*twentyPercent + 3 , 3 * twentyPercent - 3);
				BuildHorizontalRoadToRight(exitRoad, roadX);
				exit = new Vector2(widthTiles, exitRoad + 0.5f) * sizePerTile;
			}

			previousX = roadX;
			previousYbot = roadYbot;
			previousYtop = roadYtop;
		}

		// DO BUILDINGS
		GenerateBuildingsFromSeeds();
	}

	private void BuildVerticalRoad(int x, int ty, int by) {
		if(ty < heightTiles - 1) {
			tiles[x - 1, ty + 1] = TYPE_GROUND_TO_ROAD_BR;
			tiles[x    , ty + 1] = TYPE_GROUND_TO_ROAD_B;
			tiles[x + 1, ty + 1] = TYPE_GROUND_TO_ROAD_BL;
		}
		for(int y = ty; y >= by; y--) {
			if(x > 0)
				tiles[x - 1, y] = TYPE_GROUND_TO_ROAD_R;
				tiles[x    , y] = TYPE_ROAD;
			if(x < widthTiles - 1)
				tiles[x + 1, y] = TYPE_GROUND_TO_ROAD_L;
		}
		if(by > 0) {
			tiles[x - 1, by - 1] = TYPE_GROUND_TO_ROAD_TR;
			tiles[x, by - 1] = TYPE_GROUND_TO_ROAD_T;
			tiles[x + 1, by - 1] = TYPE_GROUND_TO_ROAD_TL;
		}
	}


	private void BuildHorizontalRoadToRight(int y, int lx) {
		if(tiles[lx + 1, y + 1] == TYPE_GROUND_TO_ROAD_BL)
			tiles[lx + 1, y + 1] = TYPE_GROUND_TO_ROAD_B;
		else
			tiles[lx + 1, y + 1] = TYPE_GROUND_TO_ROAD_nTR;

		tiles[lx + 1, y] = TYPE_ROAD;

		if(tiles[lx + 1, y - 1] == TYPE_GROUND_TO_ROAD_TL)
			tiles[lx + 1, y - 1] = TYPE_GROUND_TO_ROAD_T;
		else
			tiles[lx + 1, y - 1] = TYPE_GROUND_TO_ROAD_nBR;


		for(int x = lx+2; x < widthTiles; x++) {
			if(tiles[x, y + 1] == TYPE_NONE) {
				tiles[x, y + 1] = TYPE_GROUND_TO_ROAD_B;
			} else {
				if(tiles[x, y + 1] == TYPE_GROUND_TO_ROAD_BR)
					tiles[x, y + 1] = TYPE_GROUND_TO_ROAD_B;
				else
					tiles[x, y + 1] = TYPE_GROUND_TO_ROAD_nTL;
			}

			if(tiles[x, y] == TYPE_NONE) {
				tiles[x, y] = TYPE_ROAD;
			} else {
				tiles[x, y] = TYPE_ROAD;
			}

			if(tiles[x, y-1] == TYPE_NONE) {
				tiles[x, y-1] = TYPE_GROUND_TO_ROAD_T;
			} else {
				if(tiles[x, y - 1] == TYPE_GROUND_TO_ROAD_TR)
					tiles[x, y - 1] = TYPE_GROUND_TO_ROAD_T;
				else
					tiles[x, y-1] = TYPE_GROUND_TO_ROAD_nBL;
				break;
			}

			if(tiles[x,y-1]==TYPE_NONE) tiles[x, y-1] = TYPE_WATER;
			//tiles[x, y] = TYPE_WATER;
			if(tiles[x, y+1] == TYPE_NONE) tiles[x, y+1] = TYPE_WATER;
		}
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

		// Put worldborder
		scene.borders.SetDimensions(widthTiles * 2f, heightTiles * 2f, 10f);

		// recalcultate navmesh
		scene.navmesh.BuildNavMeshAsync();

		if(debug) {
			scene.player.transform.position = GetPlayerSpawn();
			Debug.Log("(DEBUG) Exit will be placed in " + GetLevelExit());
		}
	}

	public override Vector2 GetPlayerSpawn() {
		return spawn;
	}
	public override Vector2 GetLevelExit() {
		return exit;
	}

}