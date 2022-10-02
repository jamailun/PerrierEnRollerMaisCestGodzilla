using UnityEngine;

[CreateAssetMenu(fileName = "AttackShape", menuName = "PERMCG/AttackShape", order = 3)]
public class AttackShape : ScriptableObject {

	[Header("Attack data")]

	[Tooltip("Duration of an attack")]
	[SerializeField] private float attackDuration = 0.2f;
	public float AttackDuration => attackDuration;

	[Tooltip("Damage bonus of an attack")]
	[SerializeField] private float attackDamageBonus = 0f;
	public float AttackDamageBonus => attackDamageBonus;

	[Tooltip("Cooldown between 2 attacks")]
	[SerializeField] private float attackCooldown = 0.25f;
	public float AttackCooldown => attackCooldown;

	[Header("Shapes")]

	[SerializeField] private Hitbox rightPrefab;
	[SerializeField] private Vector3 rightOffset;

	[SerializeField] private Hitbox bottomPrefab;
	[SerializeField] private Vector3 bottomOffset;

	[SerializeField] private Hitbox topPrefab;
	[SerializeField] private Vector3 topOffset;

	[Tooltip("If true, the attacker can move while attacking")]
	[SerializeField] private bool moveOnAttack = false;
	public bool CanMoveOnAttack => moveOnAttack;

	public Hitbox SpawnHitbox(Orientation direction, Transform parent, float damage, float duration, float scale = 1f) {
		Hitbox prefab = direction switch {
			Orientation.Top => topPrefab,
			Orientation.Bottom => bottomPrefab,
			_ => rightPrefab
		};
		Vector2 offset = direction switch {
			Orientation.Top => topOffset,
			Orientation.Bottom => bottomOffset,
			_ => rightOffset
		};

		var hitbox = Instantiate(prefab, parent);
		hitbox.transform.localPosition = offset;
		hitbox.transform.localScale = new Vector3(scale, scale, 1f);
		hitbox.Spawn(damage, duration, (direction == Orientation.Left));

		return hitbox;
	}

}
