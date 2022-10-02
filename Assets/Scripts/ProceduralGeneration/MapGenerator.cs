using System.Collections.Generic;
using UnityEngine;

public abstract class MapGenerator : ScriptableObject {

	public abstract void Generate();

	public abstract void Populate(SceneData scene);

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

}