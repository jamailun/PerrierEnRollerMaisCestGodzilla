using UnityEngine;

public class ExitPoint : MonoBehaviour {

	private void OnTriggerEnter2D(Collider2D collision) {
		var player = collision.gameObject.GetComponent<PlayerEntity>();
		if(player) {
			Debug.Log("GO TO NEXT LEVL!!!");
			LoadingManager.Instance.NextStage(player);
		}
	}

}