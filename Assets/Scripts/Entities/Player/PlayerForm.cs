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

	[SerializeField] private Color _color;
	public Color Color => _color;

	[SerializeField] private AttackShape _attackShape;
	public AttackShape AttackShape => _attackShape;

	[SerializeField] public CustomAnimation animation_Idle;       // 2 frames
	[SerializeField] public CustomAnimation animation_Right;      // 2 frames
	[SerializeField] public CustomAnimation animation_Top;        // 2 frames
	[SerializeField] public CustomAnimation animation_Bottom;     // 2 frames

	[SerializeField] public Sprite frame_Attack_Right;		// 1 frame
	[SerializeField] public Sprite frame_Attack_Top;		// 1 frame
	[SerializeField] public Sprite frame_Attack_Bottom;		// 1 frame

}