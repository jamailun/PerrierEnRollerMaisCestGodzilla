using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Z4_Map_Generator", menuName = "PERMCG/Z4_Map_Generator", order = 13)]
public class Z4_MapGenerator : MapGenerator {

	[SerializeField] private float rxP = 0.5f;
	//[SerializeField] private float ryP = 0.5f;
	[SerializeField] private float elispe = 50f;

	[SerializeField] private Tile tileA;
	[SerializeField] private Tile tileB;

	#region local_types_def

	private const int TYPE_NONE = 0;
	private const int TYPE_TILE_1 = 1;
	private const int TYPE_TILE_2 = 2;

	private Tile GetTile(int x, int y) {
		var type = tiles[x, y];
		return type switch {
			TYPE_TILE_1 => tileA,
			TYPE_TILE_2 => tileB,
			_ => tileA
		};
	}

	#endregion

	public override void Generate() {
		tiles = new int[widthTiles, heightTiles];
		placeds = new();

		float cx = widthTiles / 2f;
		float cy = heightTiles / 2f;
		float rx = (float) widthTiles * rxP;
		float ry = (float) heightTiles * rxP;

		for(int x = 0; x < widthTiles; x++) {
			for(int y = 0; y < heightTiles; y++) {
				bool isIn = IsInEllipse(x, y, rx, ry, cx, cy);
				tiles[x, y] = isIn ? TYPE_TILE_1 : TYPE_TILE_2;
			}
		}
	}

	private bool IsInEllipse(float x, float y, float rx, float ry, float cx, float cy) {
		return ((Mathf.Pow(x-cx, 2f)/rx) + (Mathf.Pow(y - cy, 2f) / ry)) <= elispe;
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

		// recalcultate navmesh
		scene.navmesh.BuildNavMeshAsync();

		// Put worldborder
		scene.borders.SetDimensions(widthTiles * 2f, heightTiles * 2f, 10f);

		if(debug) {
			scene.player.transform.position = GetPlayerSpawn();
			scene.exit.transform.position = GetLevelExit();
		}
	}


	public override Vector2 GetPlayerSpawn() {
		return new(sizePerTile * widthTiles * 0.38f , sizePerTile *  heightTiles/2f);
	}
	public override Vector2 GetBossPos() {
		return new(sizePerTile * widthTiles * 0.62f, sizePerTile * heightTiles / 2f);
	}
	public override Vector2 GetLevelExit() {
		return new(-50, -50);
	}

}