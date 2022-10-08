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
		if(Input.GetKeyDown(KeyCode.Escape)) {
			_player.Pause();
			return;
		}
		if(_player.Animator == null) {
			return; // ignore this frame.
		}

		// Move values
		horizontal = Input.GetAxisRaw("Horizontal");
		vertical = Input.GetAxisRaw("Vertical");

		if(horizontal == 0 && vertical == 0)
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
		} else if(Input.GetKeyDown(KeyCode.LeftShift)) {
			_player.TryDash(horizontal, vertical);
		}
		
	}

	private void FixedUpdate() {
		if(_player.IsDashing()) {
			float remainnigDistance = (_player.DashTarget - _player.transform.position).sqrMagnitude;
			if(remainnigDistance < 0.1f || _player.DashStarted - Time.time >= 2f) {
				_player.StopDashing();
			} else {
				_body.velocity = _player.GetSpeed() * Time.fixedDeltaTime * _player.DashDirection;
			}
		} else {
			// Normal movement
			_body.velocity = _player.GetSpeed() * Time.fixedDeltaTime * new Vector2(horizontal, vertical).normalized;
		}
	}
}
