using UnityEngine;

public class Rotator : MonoBehaviour {
	[SerializeField] private float angularSpeed = 10f;

	private void Update() {
		transform.Rotate(Vector3.forward, Time.deltaTime * angularSpeed);
	}

}