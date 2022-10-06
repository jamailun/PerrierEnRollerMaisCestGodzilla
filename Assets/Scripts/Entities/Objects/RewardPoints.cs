using UnityEngine;

public class RewardPoints : Attractible {

	[Tooltip("The amount of points to give to the player.")]
	[SerializeField] public uint rewardAmount = 0;

	private void OnTriggerEnter2D(Collider2D collision) {
		var player = collision.gameObject.GetComponent<PlayerEntity>();
		if(player != null) {
			player.AddRewardPoints(rewardAmount);
			Destroy(gameObject);
		} else {
			var hb = collision.gameObject.GetComponent<Hurtbox>();
			if(hb != null && hb.IsPlayer()) {
				hb.gameObject.GetComponentInParent<PlayerEntity>().AddRewardPoints(rewardAmount);
				Destroy(gameObject);
			}
		}
	}

}
		