using UnityEngine;

public class ExperienceBall : Attractible {

	[Tooltip("The amount of experience given to the player.")]
	[SerializeField] public ulong experienceAmount = 10L;

	private void OnTriggerEnter2D(Collider2D collision) {
		var player = collision.gameObject.GetComponent<PlayerEntity>();
		if(player != null) {
			player.AddExperience(experienceAmount);
			Destroy(gameObject);
		}
	}

}
		