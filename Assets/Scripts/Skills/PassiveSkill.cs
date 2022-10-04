using UnityEngine;

[CreateAssetMenu(fileName = "PassiveSkill", menuName = "PERMCG/Passive Skill", order = 1)]
public class PassiveSkill : Skill {
	public override SkillType SkillType { get { return SkillType.Passive; } }

	[SerializeField] private Statistic _statistic;
	public Statistic Statistic => _statistic;

	[SerializeField] private bool isMultiplicative = true;
	public bool IsMultiplicative => isMultiplicative;

	// if ADDITIVE, on fait "STAT + BONUS", sinon "STAT * BONUS"

	[SerializeIf("isMultiplicative", false, ComparisonType.Boolean)]
	[SerializeField] private float _flatAddPerLevel = 1f; // + 1 par level ici
	public float AdditionPerLevel => _flatAddPerLevel;

	[SerializeIf("isMultiplicative", true, ComparisonType.Boolean)]
	[SerializeField] private float _mutlAddPerLevel = 0.05f; // + 5% par level ici
	public float MultiplicatorPerLevel => _mutlAddPerLevel;


}