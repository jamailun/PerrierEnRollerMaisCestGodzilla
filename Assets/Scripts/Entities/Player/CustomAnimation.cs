using UnityEngine;
using System;

[Serializable]
public class CustomAnimation {

	public float frameDuration = 0.15f;
	public Sprite[] points;

	public Sprite GetFirst() {
		return points[0];
	}

	public Sprite GetCurrent(float time) {
		int frame = Mathf.FloorToInt(time / frameDuration) % points.Length;
		return points[frame];
	}
	public Sprite GetCurrentForced(float time, float _frameDuration) {
		int frame = Mathf.FloorToInt(time / _frameDuration) % points.Length;
		Debug.LogWarning("time="+time+", _fd="+_frameDuration+". FRAME="+frame);
		return points[frame];
	}

}