using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Z3_Map_Generator", menuName = "PERMCG/Z3_Map_Generator", order = 12)]
public class Z3_MapGenerator : MapGenerator {

	[Header("Tiles used in zone 2")]
	[SerializeField] private Tile waterTile;
	[SerializeField] private Tile groundTile;
	[SerializeField] private Tile grassTile;

	[SerializeField] private Tile roadTile;
	[SerializeField] private Tile road_L;
	[SerializeField] private Tile road_R;
	[SerializeField] private Tile road_T;
	[SerializeField] private Tile road_B;
	[SerializeField] private Tile road_BR;
	[SerializeField] private Tile road_BL;
	[SerializeField] private Tile road_TR;
	[SerializeField] private Tile road_TL;
	[SerializeField] private Tile road_nBR;
	[SerializeField] private Tile road_nBL;
	[SerializeField] private Tile road_nTR;
	[SerializeField] private Tile road_nTL;

	#region local_types_def

	private const int TYPE_NONE = 0;

	private const int TYPE_WATER = 1;

	private const int TYPE_GROUND = 3;

	private const int TYPE_GRASS = 31;

	private const int TYPE_ROAD_L = 3401;
	private const int TYPE_ROAD_R = 3402;
	private const int TYPE_ROAD_T = 3403;
	private const int TYPE_ROAD_B = 3404;
	private const int TYPE_ROAD_BR = 3411;
	private const int TYPE_ROAD_BL = 3412;
	private const int TYPE_ROAD_TR = 3413;
	private const int TYPE_ROAD_TL = 3414;
	private const int TYPE_ROAD_nBR = 3421;
	private const int TYPE_ROAD_nBL = 3422;
	private const int TYPE_ROAD_nTR = 3423;
	private const int TYPE_ROAD_nTL = 3424;

	private const int TYPE_ROAD = 4;

	private Tile GetTile(int x, int y) {
		var type = tiles[x, y];
		if(type == TYPE_ROAD)
			type = TransformRoad(x, y);

		return type switch {
			TYPE_WATER => waterTile,

			TYPE_GROUND => groundTile,

			TYPE_GRASS => grassTile,

			TYPE_ROAD_L => road_L,
			TYPE_ROAD_R => road_R,
			TYPE_ROAD_B => road_B,
			TYPE_ROAD_T => road_T,

			TYPE_ROAD_BR => road_BR,
			TYPE_ROAD_TR => road_TR,
			TYPE_ROAD_BL => road_BL,
			TYPE_ROAD_TL => road_TL,

			TYPE_ROAD_nBR => road_nBR,
			TYPE_ROAD_nTR => road_nTR,
			TYPE_ROAD_nBL => road_nBL,
			TYPE_ROAD_nTL => road_nTL,


			TYPE_ROAD => roadTile,

			_ => groundTile
		};
	}

	#endregion

	private Vector2 spawn, exit;

	public override void Generate() {
		tiles = new int[widthTiles, heightTiles];
		placeds = new();

		spawn = new Vector2(2.5f, heightTiles / 2 * sizePerTile + 2f);
		exit = new Vector2(widthTiles * sizePerTile - 2.5f, heightTiles / 2 * sizePerTile + 2f);

		// DO BUILDINGS
		GenerateBuildingsFromSeeds();
	}

	public override void Populate(SceneData scene, bool debug = true) {
		scene.tilemap.ClearAllTiles();

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
			scene.exit.transform.position = GetLevelExit();
		}
	}

	private bool IsTile(int x, int y, int looked, bool acceptBad = true) {
		if(x < 0 || y < 0 || x >= widthTiles || y >= heightTiles)
			return acceptBad;
		return tiles[x, y] == looked;
	}

	private int TransformRoad(int x, int y) {
		bool up = IsTile(x, y + 1, TYPE_ROAD);
		bool down = IsTile(x, y - 1, TYPE_ROAD);
		bool left = IsTile(x - 1, y, TYPE_ROAD);
		bool right = IsTile(x + 1, y, TYPE_ROAD);

		if(x > 0)
			left = tiles[x - 1, y] == TYPE_ROAD;
		if(y > 0)
			down = tiles[x, y - 1] == TYPE_ROAD;
		if(x < widthTiles - 1)
			right = tiles[x + 1, y] == TYPE_ROAD;
		if(y < heightTiles - 1)
			up = tiles[x, y + 1] == TYPE_ROAD;

		if(up) {
			if(down) {
				if(left) {
					if(right) {
						if(!IsTile(x + 1, y + 1, TYPE_ROAD))
							return TYPE_ROAD_nTR;
						if(!IsTile(x - 1, y + 1, TYPE_ROAD))
							return TYPE_ROAD_nTL;
						if(!IsTile(x + 1, y - 1, TYPE_ROAD))
							return TYPE_ROAD_nBR;
						if(!IsTile(x - 1, y - 1, TYPE_ROAD))
							return TYPE_ROAD_nBL;
						return TYPE_ROAD;
					}
					return TYPE_ROAD_L;
				}
				// !left
				if(right)
					return TYPE_ROAD_R;
				return TYPE_WATER;
			}
			// !down
			if(right) {
				if(left)
					return TYPE_ROAD_B;
				return TYPE_ROAD_TR;
			}
			// !right
			if(left)
				return TYPE_ROAD_TL;
			return TYPE_WATER;
		}
		// !up
		if(right) {
			if(down) {
				if(left)
					return TYPE_ROAD_T;
				return TYPE_ROAD_BR;
			}
			return TYPE_WATER;
		}
		// ! right
		if(left) {
			if(down)
				return TYPE_ROAD_BL;
			return TYPE_WATER;
		}
		return TYPE_WATER;
		
	}

	public override Vector2 GetPlayerSpawn() {
		return spawn;
	}
	public override Vector2 GetLevelExit() {
		return exit;
	}

}