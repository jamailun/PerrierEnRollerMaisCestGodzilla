using UnityEngine;
using System;

[Serializable]
public class CustomAnimation {

	public float frameDuration = 0.15f;
	public Sprite[] points;

	public Sprite GetCurrent(float time) {
		int frame = Mathf.FloorToInt(time / frameDuration) % points.Length;
		return points[frame];
	}

}