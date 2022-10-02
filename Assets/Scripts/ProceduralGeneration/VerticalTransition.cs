using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class VerticalTransition {

	[SerializeField] private int transitionMin = 2;
	[SerializeField] private int transitionMax = 4;

	[SerializeField] private Tile leftTile;
	[SerializeField] private Tile rightTile;

	[SerializeField] [Range(0.01f, 0.5f)] private float probaTransitionRight = 0.2f;
	[SerializeField] [Range(0.01f, 0.5f)] private float probaTransitionLeft = 0.2f;

	[SerializeField] private Tile transition_BR;
	[SerializeField] private Tile transition_LT;
	[SerializeField] private Tile transition_BT;
	[SerializeField] private Tile transition_BL;
	[SerializeField] private Tile transition_RT;

	public void Populate(Tilemap tilemap, int height) {
		int tx = Random.Range(transitionMin, transitionMax);
		bool previousRight = false;
		bool previousLeft = false;
		for(int y = 0; y < height; y++) {

			bool toRight = false;
			bool toLeft = false;

			// Try to go to right
			if(tx < transitionMax && Random.value <= probaTransitionRight && !previousLeft) {
				toRight = true;
				// Try to go to left
			} else if(tx > transitionMin && Random.value <= probaTransitionLeft && !previousRight) {
				toLeft = true;
			}

			previousLeft = previousRight = false;

			// fill left elem to the transition
			for(int x = 0; x < tx - 1; x++) {
				tilemap.SetTile(new Vector3Int(x, y, 0), leftTile);
			}

			// do the transition
			if(toRight) {
				tilemap.SetTile(new Vector3Int(tx - 1, y, 0), transition_BR);
				tilemap.SetTile(new Vector3Int(tx, y, 0), transition_LT);
				tx++;
				previousRight = true;
			} else if(toLeft) {
				tx--;
				tilemap.SetTile(new Vector3Int(tx - 1, y, 0), transition_RT);
				tilemap.SetTile(new Vector3Int(tx, y, 0), transition_BL);
				previousLeft = true;
			} else {
				tilemap.SetTile(new Vector3Int(tx - 1, y, 0), transition_BT);
				tilemap.SetTile(new Vector3Int(tx, y, 0), rightTile);
			}

		}
	}

}