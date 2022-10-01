using UnityEngine;
using System;

[Serializable]
public class HurtboxDescriptor {

	[Serializable]
	public enum Type : int {
		Box,
		Capsule,
		Polygon
	}

	[SerializeField] private Type hurtboxType;

	[Header("Hurtbox data")]

	[SerializeField] private Vector2 size;

	[SerializeField] private Vector2 offset;

	[SerializeField] private CapsuleDirection2D capsuleDirection;

	[SerializeField] private Vector2[] points;

	public Collider2D CreateCollider(Hurtbox hurtbox) {
		if(hurtbox == null) {
			Debug.LogError("#CreateCollider with NULL hurtbox parameter.");
			return null;
		}

		foreach(var old in hurtbox.gameObject.GetComponents<Collider2D>())
			GameObject.DestroyImmediate(old);

		switch(hurtboxType) {
			case Type.Box:
				var box = hurtbox.gameObject.AddComponent<BoxCollider2D>();
				box.isTrigger = true;
				box.size = size;
				box.offset = offset;
				return box;

			case Type.Capsule:
				var capsule = hurtbox.gameObject.AddComponent<CapsuleCollider2D>();
				capsule.isTrigger = true;
				capsule.direction = capsuleDirection;
				capsule.size = size;
				capsule.offset = offset;
				return capsule;

			case Type.Polygon:
				var polygon = hurtbox.gameObject.GetComponent<PolygonCollider2D>();
				polygon.isTrigger = true;
				polygon.offset = offset;
				polygon.pathCount = 1;
				polygon.SetPath(0, points);
				return polygon;
		}

		throw new NotImplementedException("Unknown hurtbox descriptor type : " + hurtboxType + ".");
	}

}