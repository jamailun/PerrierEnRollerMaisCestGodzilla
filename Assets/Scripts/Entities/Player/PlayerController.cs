using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerEntity))]
public class PlayerController : MonoBehaviour {

	[Tooltip("The renderer for the player. Used to flip on X axis.")]
	[SerializeField] private SpriteRenderer spriteRenderer;

	private PlayerEntity _player;
	private Rigidbody2D _body;

	private Orientation orientation;
	private float horizontal, vertical;

	private void Start() {
		_body = GetComponent<Rigidbody2D>();
		_player = GetComponent<PlayerEntity>();
	}

	private void Update() {
		// Move values
		horizontal = Input.GetAxisRaw("Horizontal");
		vertical = Input.GetAxisRaw("Vertical");

		_player.Animator.Play(PlayerEntity.ANIM_IDLE);

		// swap renderer
		if(horizontal != 0) { // only when it changes
			spriteRenderer.flipX = horizontal < 0;
			_player.Animator.Play(PlayerEntity.ANIM_RIGHT);
			orientation = spriteRenderer.flipX ? Orientation.Left : Orientation.Right;
		} else {
			if(vertical > 0) {
				_player.Animator.Play(PlayerEntity.ANIM_TOP);
				orientation = Orientation.Top;
			} else if(vertical < 0) {
				_player.Animator.Play(PlayerEntity.ANIM_DOWN);
				orientation = Orientation.Bottom;
			}
		}
		
		// Automatically create an attack if possible.
		if(Input.GetKeyDown(KeyCode.Space)) {
			_player.TryAttack(orientation);
		}
	}

	private void FixedUpdate() {
		_body.velocity = _player.GetSpeed() * Time.fixedDeltaTime * new Vector2(horizontal, vertical).normalized;
	}
}
