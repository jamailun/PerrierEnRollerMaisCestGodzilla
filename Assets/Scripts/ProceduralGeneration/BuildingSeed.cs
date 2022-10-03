using UnityEngine;

[System.Serializable]
public class BuildingSeed {

	public Building buildingPrefab;
	public float lockRadius = 2f;

	public float minX = 2f;
	public float maxX = 4f;

	public int minPerGroup = 1;
	public int maxPerGroup = 1;

	public int optimalAmount = 15;

	public int bonusTries = 0;

	public float avoidCenterY = 0;

	public int[] requireTile;

	public bool CanBePlaced(int tile) {
		if(requireTile.Length == 0)
			return true;
		foreach(var t in requireTile)
			if(t == tile)
				return true;
		return false;
	}

	public float GetBufferY() {
		var coll =  buildingPrefab.GetComponentInChildren<Hurtbox>()?.GetComponent<Collider2D>();
		if(coll == null)
			return 0;
		if(typeof(BoxCollider2D).IsInstanceOfType(coll))
			return ((BoxCollider2D) coll).size.y / 2f;
		if(typeof(CircleCollider2D).IsInstanceOfType(coll))
			return ((CircleCollider2D) coll).radius / 2f;
		if(typeof(CapsuleCollider2D).IsInstanceOfType(coll))
			return ((CapsuleCollider2D) coll).size.y / 2f;
		if(typeof(PolygonCollider2D).IsInstanceOfType(coll)) {
			float maxY = float.MinValue;
			float minY = float.MaxValue;
			var poly = (PolygonCollider2D) coll;
			for(int i = 0; i < poly.pathCount; i++) {
				foreach(var pt in poly.GetPath(i)) {
					if(pt.y < minY)
						minY = pt.y;
					else if(pt.y > maxY)
						maxY = pt.y;
				}
			}
			return Mathf.Abs(maxY - minY) / 2f;
		}
		Debug.LogWarning("Unknown collider2D for building prefab \"" + buildingPrefab.name + "\" : " + coll);
		return 0f;
	}

	public struct Placed {
		public Building building;
		public float x, y;
		public float radius;

		public Placed(BuildingSeed seed, UnityEngine.Vector2 pos) {
			building = seed.buildingPrefab;
			radius = seed.lockRadius;
			x = pos.x;
			y = pos.y;
		}
	}

}