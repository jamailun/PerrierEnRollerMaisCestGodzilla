using UnityEngine;

public class ExitPoint : MonoBehaviour {

	private void OnTriggerEnter2D(Collider2D collision) {
		if(collision.gameObject.GetComponent<AttractiblesMagnet>() != null)
			return;
		var player = collision.gameObject.GetComponent<PlayerEntity>();
		if(!player)
			player = collision.gameObject.GetComponentInParent<PlayerEntity>();
		if(player) {
			Debug.Log("GO TO NEXT LEVEL ! Btw, NOW on a " + LoadingManager.Instance.Stage + ".");
			LoadingManager.Instance.NextStage(player);
		}
	}

}