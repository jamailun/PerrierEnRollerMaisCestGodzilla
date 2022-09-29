using UnityEngine;

[CreateAssetMenu(fileName = "AttackShape", menuName = "PERMCG/AttackShape", order = 3)]
public class AttackShape : ScriptableObject {

	[SerializeField] private Hitbox rightPrefab;
	[SerializeField] private Vector2 rightOffset;

	[SerializeField] private Hitbox bottomPrefab;
	[SerializeField] private Vector2 bottomOffset;

	[SerializeField] private Hitbox topPrefab;
	[SerializeField] private Vector2 topOffset;

	[Tooltip("If true, the attacker can move while attacking")]
	[SerializeField] private bool moveOnAttack = false;
	public bool CanMoveOnAttack => moveOnAttack;

	public Hitbox SpawnHurtbox(Orientation direction, Transform parent, float damage, float duration) {
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
		hitbox.Spawn(damage, duration, (direction == Orientation.Left));

		return hitbox;
	}

}
