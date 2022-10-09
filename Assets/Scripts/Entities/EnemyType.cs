[System.Serializable]

public enum EnemyType {
	/// <summary>
	/// N'a pas d'IA.
	/// </summary>
	None,

	/// <summary>
	/// Run toward the player and hit him.
	/// </summary>
	Melee,

	/// <summary>
	/// Keeps his distance from the player and attack him.
	/// </summary>
	Distance,

	/// <summary>
	/// Boss, overriden by code.
	/// </summary>
	Boss
}