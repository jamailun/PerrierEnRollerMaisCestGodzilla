using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Z2_Map_Generator", menuName = "PERMCG/Z2_Map_Generator", order = 11)]
public class Z2_MapGenerator : MapGenerator {

	[Header("Main road")]

	[SerializeField] private int parcAmountMin = 1;
	[SerializeField] private int parcAmountMax = 2;
	[SerializeField] private int parcCircleAmountMin = 1;
	[SerializeField] private int parcCircleAmountMax = 2;

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

		int beginRoadX = 0;
		int endsRoadX = beginRoadX + Random.Range(widthTiles / 6, widthTiles / 3);
		bool firstGoDown = Random.value > 0.5f;
		int roadYstart = firstGoDown ? Random.Range((int) (0.65f * heightTiles), heightTiles - 4) : Random.Range(4, (int) (0.35f * heightTiles));

		for(int x = beginRoadX; x <= endsRoadX + 2; x++) {
			for(int y = roadYstart - 2; y <= roadYstart + 2; y++) {
				tiles[x, y] = TYPE_ROAD;
			}
		}

		spawn = new Vector2(beginRoadX + 3f, roadYstart + 3f);

		if(firstGoDown) {
			// do to bottom
			for(int y = roadYstart; y >= 0; y--) {
				for(int x = endsRoadX - 2; x <= endsRoadX + 2; x++) {
					tiles[x, y] = TYPE_ROAD;
				}
			}
		} else {
			// go to top
			for(int y = roadYstart; y < heightTiles; y++) {
				for(int x = endsRoadX - 2; x <= endsRoadX + 2; x++) {
					tiles[x, y] = TYPE_ROAD;
				}
			}
		}

		int hh = heightTiles / 2;
		int deltaHh = Random.Range(0, hh - 4);
		hh += (firstGoDown ? -deltaHh : deltaHh);
		int gotox = widthTiles / 2 + Random.Range(-6, 30);
		for(int x = 0; x <= gotox; x++) {
			for(int y = hh - 2; y <= hh + 2; y++) {
				tiles[x, y] = TYPE_ROAD;
			}
		}

		int deltaX = gotox - endsRoadX;
		int sx = endsRoadX + Random.Range(5, 10);
		bool goDown = Random.Range(0f, 1f) > 0.5f;
		int lastSX = sx;
		bool lastGD = goDown;
		while(sx < gotox - 4) {
			if(goDown) {
				// do to bottom
				for(int y = hh; y >= 0; y--) {
					for(int x = sx - 2; x <= sx + 2; x++) {
						tiles[x, y] = TYPE_ROAD;
					}
				}
			} else {
				// go to top
				for(int y = hh; y < heightTiles; y++) {
					for(int x = sx - 2; x <= sx + 2; x++) {
						tiles[x, y] = TYPE_ROAD;
					}
				}
			}
			// change values
			lastSX = sx;
			lastGD = goDown;
			sx += Random.Range(5, deltaX / 2);
			goDown = (Random.Range(0f, 1f) > 0.3f) ? !goDown : goDown;
		}


		int sy = lastGD ? Random.Range(4, hh - 4) : Random.Range(hh + 4, heightTiles - 5);
		for(int x = lastSX; x < widthTiles; x++) {
			for(int y = sy - 2; y <= sy + 2; y++) {
				tiles[x, y] = TYPE_ROAD;
			}
		}

		int nParcs = Random.Range(parcAmountMin, parcAmountMax);
		int pcx = 0;
		int pcy = 0;
		for(int i = 0; i < nParcs; i++) {
			pcx = Random.Range(endsRoadX + 6, widthTiles - 10);
			pcy = lastGD ? Random.Range(sy, heightTiles - 5) : Random.Range(5, sy);
			int rx = Random.Range(4, 12);
			int ry = Random.Range(4, 8);
			for(int px = -rx; px <= rx; px++) {
				for(int py = -ry; py <= ry; py++) {
					int x = pcx + px;
					int y = pcy + py;
					if(!IsTile(x, y, TYPE_ROAD, true))
						tiles[x, y] = TYPE_GRASS;
				}
			}
		}

		int nParcsCircle = Random.Range(parcCircleAmountMin, parcCircleAmountMax);
		for(int i = 0; i < nParcsCircle; i++) {
			pcx = Random.Range(15, widthTiles - 10);
			pcy = lastGD ? Random.Range(sy, heightTiles - 5) : Random.Range(5, sy);
			int r = Random.Range(8, 15);
			for(int px = -r; px <= r; px++) {
				for(int py = -r; py <= r; py++) {
					if(Vector2.Distance(new Vector2(px, py), new Vector2()) > r - 0.1f)
						continue;
					int x = pcx + px;
					int y = pcy + py;
					if(!IsTile(x, y, TYPE_ROAD, true))
						tiles[x, y] = TYPE_GRASS;
				}
			}
		}


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

		// Put worldborder
		scene.borders.SetDimensions(widthTiles * 2f, heightTiles * 2f, 10f);

		// recalcultate navmesh
		scene.navmesh.BuildNavMeshAsync();

		if(debug) {
			scene.player.transform.position = GetPlayerSpawn();
			Debug.Log("(DEBUG) Exit will be placed in " + GetLevelExit());
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