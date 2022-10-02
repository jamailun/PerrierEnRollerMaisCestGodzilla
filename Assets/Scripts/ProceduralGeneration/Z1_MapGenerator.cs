using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

/// <summary>
/// ZONE 1.
/// Contraintes :
/// - de gauche à droite,
/// - de la mer à gauche
/// - de plus en plus de batiments
/// 
/// 
/// </summary>

[CreateAssetMenu(fileName = "Map_Generator", menuName = "PERMCG/Z1_Map_Generator", order = 10)]
public class Z1_MapGenerator : MapGenerator {

	[SerializeField] private int widthTiles = 10;
	[SerializeField] private int heightTiles = 5;
	[SerializeField] private int waterSizeMin = 2;
	[SerializeField] private int waterSizeMax = 4;

	[Header("Seeds")]
	[SerializeField] [Range(0.01f, 0.5f)] private float waterSandTransitionRight = 0.2f;
	[SerializeField] [Range(0.01f, 0.5f)] private float waterSandTransitionLeft = 0.2f;

	[Header("Tiles")]
	[SerializeField] private Tile waterTile;
	[SerializeField] private Tile sandTile;
	[SerializeField] private Tile groundTile;

	[SerializeField] private Tile water_sand_BR;
	[SerializeField] private Tile water_sand_LT;
	[SerializeField] private Tile water_sand_BT;
	[SerializeField] private Tile water_sand_BL;
	[SerializeField] private Tile water_sand_RT;

	private enum TileType : int {
		Ground = 0,

		Water,
		WaterToSand_BR,
		WaterToSand_LT,
		WaterToSand_BT,
		WaterToSand_BL,
		WaterToSand_RT,
		Sand,

		Building,
		Road
	}
	private Tile GetTile(int x, int y) {
		return tiles[x,y] switch {
			TileType.Water => waterTile,

			TileType.WaterToSand_BR => water_sand_BR,
			TileType.WaterToSand_LT => water_sand_LT,
			TileType.WaterToSand_BT => water_sand_BT,
			TileType.WaterToSand_BL => water_sand_BL,
			TileType.WaterToSand_RT => water_sand_RT,

			TileType.Sand => sandTile,

			_ => groundTile
		};
	}
	TileType[,] tiles;

	public int sizePerTile = 64;

	// Points délimitants la mer (pour le collider)
	// tiles où mettre la mer

	public override void Generate() {
		tiles = new TileType[widthTiles, heightTiles];

		// WATER -> SAND TRANSITION
		int waterX = Random.Range(waterSizeMin, waterSizeMax);
		bool previousRight = false;
		bool previousLeft = false;
		for(int y = 0; y < heightTiles; y++) {

			bool toRight = false;
			bool toLeft = false;

			// Try to go to right
			if(waterX < waterSizeMax && Random.value <= waterSandTransitionRight && ! previousLeft) {
				toRight = true;
			// Try to go to left
			} else if(waterX > waterSizeMin && Random.value <= waterSandTransitionLeft && ! previousRight) {
				toLeft = true;
			}

			previousLeft = previousRight = false;
			
			// fill water to the transition
			for(int x = 0; x < waterX - 1; x++) {
				tiles[x, y] = TileType.Water;
			}
			// do the transition
			if(toRight) {
				tiles[waterX-1, y] = TileType.WaterToSand_BR;
				tiles[waterX, y] = TileType.WaterToSand_LT;
				waterX++;
				previousRight = true;
			} else if(toLeft) {
				waterX--;
				tiles[waterX - 1, y] = TileType.WaterToSand_RT;
				tiles[waterX, y] = TileType.WaterToSand_BL;
				previousLeft = true;
			} else {
				tiles[waterX - 1, y] = TileType.WaterToSand_BT;
				tiles[waterX, y] = TileType.Sand;
			}

		}
	}

	public override void Populate(SceneData scene) {
		for(int x = 0; x < widthTiles; x++) {
			for(int y = 0; y < heightTiles; y++) {
				scene.tilemap.SetTile(new(x, y, 0), GetTile(x,y));
			}
		}

	}


}