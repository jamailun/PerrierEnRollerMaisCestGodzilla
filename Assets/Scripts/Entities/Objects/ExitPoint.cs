using UnityEngine;

public class ExitPoint : MonoBehaviour {

	private void OnTriggerEnter2D(Collider2D collision) {
		var player = collision.gameObject.GetComponent<PlayerEntity>();
		if(player) {
			Debug.Log("GO TO NEXT LEVEL ! Btw, NOW on a " + LoadingManager.Instance.Stage + ".");
			LoadingManager.Instance.NextStage(player);
		}
	}

}