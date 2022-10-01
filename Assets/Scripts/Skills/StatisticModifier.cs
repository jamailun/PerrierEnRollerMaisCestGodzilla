[System.Serializable]
public struct StatisticModifier {

	public Statistic statistic;

	public StaticticModiierType modifierType;

	public float modifier;

	public bool IsMultiplicative() {
		return modifierType == StaticticModiierType.Multiplicative;
	}

	[System.Serializable]
	public enum StaticticModiierType {
		Additive,
		Multiplicative
	}

}