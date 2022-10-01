using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class AttractiblesMagnet : MonoBehaviour {

	[Tooltip("The force used to attract object")]
	[SerializeField] private float magnetForce = 2f;

	private void OnTriggerEnter2D(Collider2D collision) {
		var obj = collision.gameObject.GetComponent<Attractible>();
		if(obj != null && ! obj.Attracted) {
			obj.StartAttract(transform, magnetForce);
		}
	}

	public void SetParameters(Vector2 offset, float range) {
		transform.localPosition = new Vector3(offset.x, offset.y, 0);
		GetComponent<CircleCollider2D>().radius = range;
	}

}