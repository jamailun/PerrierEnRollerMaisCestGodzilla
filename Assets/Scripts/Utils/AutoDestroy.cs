using UnityEngine;

public class AutoDestroy : MonoBehaviour {
	[SerializeField] private float lifeTime = 1f;

	private void Start() {
		StartCoroutine(Utils.DestroyAfter(gameObject, lifeTime));
	}

}