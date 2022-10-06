using UnityEngine;

public class WorldBorder : MonoBehaviour {

	[SerializeField] private BoxCollider2D boxTop;
	[SerializeField] private BoxCollider2D boxRight;
	[SerializeField] private BoxCollider2D boxBottom;
	[SerializeField] private BoxCollider2D boxLeft;

	public void SetDimensions(float width, float height, float margin) {
		float h2 = height / 2f;
		float w2 = width / 2f;
		float m2 = margin / 2f;
		// Sizes
		boxTop.size = new Vector2(width + margin, margin);
		boxBottom.size = new Vector2(width + margin, margin);
		boxRight.size = new Vector2(margin, height + margin*2);
		boxLeft.size = new Vector2(margin, height + margin*2);
		// Offsets
		boxTop.offset = new Vector2(w2 + m2, height + m2);
		boxBottom.offset = new Vector2(w2 + m2, - m2);
		boxRight.offset = new Vector2(width + m2, h2);
		boxLeft.offset = new Vector2(- m2, h2);
	}

}