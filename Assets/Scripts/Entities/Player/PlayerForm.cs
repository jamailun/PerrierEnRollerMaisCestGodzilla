using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Forme actuelle du joueur.
/// </summary>
[CreateAssetMenu(fileName = "PlayerForm", menuName = "PERMCG/PlayerForm", order = 2)]
public class PlayerForm : ScriptableObject {

	[SerializeField] private string _name;
	public string Name => _name;

	[SerializeField] private PlayerForm[] _descendants;
	public List<PlayerForm> Descendants { get { return new List<PlayerForm>(_descendants); } }

	[SerializeField] private string _description;
	public string Description => Description;

	[SerializeField] private Color _color = Color.white;
	public Color Color => _color;

	[SerializeField] private float _scale = 1f;
	public float Scale => _scale;

	[SerializeField] private HurtboxDescriptor _hurtboxDescriptor;
	public HurtboxDescriptor HurtboxDescriptor => _hurtboxDescriptor;

	public Vector2 magnetOffset;
	public float magnetRange;
	public bool groundCollider;
	public Vector2 groundColliderSize;
	public Vector2 groundColliderOffset;

	[SerializeField] private Sprite _bannerImage;
	public Sprite BannerImage => _bannerImage;

	[SerializeField] private float _attackScale = 1f;
	public float AttackScale => _attackScale;

	[SerializeField] private AttackShape _attackShape;
	public AttackShape AttackShape => _attackShape;

	public float dashDistance = 2f;
	public float dashCooldown = 1f;

	public bool CanDash => dashCooldown > 0;

	[SerializeField] public CustomAnimation animation_Idle;       // 2 frames
	[SerializeField] public CustomAnimation animation_Right;      // 2 frames
	[SerializeField] public CustomAnimation animation_Top;        // 2 frames
	[SerializeField] public CustomAnimation animation_Bottom;     // 2 frames

	[SerializeField] public Sprite frame_Attack_Right;		// 1 frame
	[SerializeField] public Sprite frame_Attack_Top;		// 1 frame
	[SerializeField] public Sprite frame_Attack_Bottom;     // 1 frame

	[Header("Bonus stats per level up")]
	[SerializeField] public float bonusMaxHealthPerLevel = 5f;
	[SerializeField] public float bonusAttackPerLevel = 1f;


}