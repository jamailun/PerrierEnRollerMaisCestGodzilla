using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttractiblesMagnet : MonoBehaviour {

	[Tooltip("The force used to attract object")]
	[SerializeField] private float magnetForce = 2f;

	private void OnTriggerEnter2D(Collider2D collision) {
		var obj = collision.gameObject.GetComponent<Attractible>();
		if(obj != null && ! obj.Attracted) {
			obj.StartAttract(transform, magnetForce);
		}
	}

}