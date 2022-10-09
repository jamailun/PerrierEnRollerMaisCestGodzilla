using UnityEngine;

//[CreateAssetMenu(fileName = "Skill", menuName = "PERMCG/Skill", order = 2)]
public abstract class Skill : ScriptableObject {

	[SerializeField] private string _name;
	public string Name => _name;

	[SerializeField] private Sprite _sprite;
	public Sprite Sprite => _sprite;

	[SerializeField] private int _levelMax = 5;
	public float LevelMax => _levelMax;

	[SerializeField] private string _description;
	public string Description => _description;

	public abstract SkillType SkillType { get; }

	public bool IsActive() {
		return SkillType == SkillType.Active;
	}

}