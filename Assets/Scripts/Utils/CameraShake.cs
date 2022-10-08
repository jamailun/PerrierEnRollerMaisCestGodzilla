using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour {

	public IEnumerator Shake(float duration, float magnitude) {
		var origin = transform.position;
		float elapsed = 0;
		while(elapsed < duration) {
			float x = Random.Range(-1f, 1f) * magnitude;
			float y = Random.Range(-1f, 1f) * magnitude;
			transform.localPosition += new Vector3(x, y, origin.z);
			elapsed += Time.deltaTime;
			yield return null;
		}
	}	

}