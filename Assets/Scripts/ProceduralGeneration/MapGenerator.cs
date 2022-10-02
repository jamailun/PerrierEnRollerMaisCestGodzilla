using UnityEngine;

public abstract class MapGenerator : ScriptableObject {

	public abstract void Generate();

	public abstract void Populate(SceneData scene);

}