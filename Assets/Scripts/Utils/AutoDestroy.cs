using UnityEngine;

public class AutoDestroy : MonoBehaviour {
	[SerializeField] public float lifeTime = 1f;

	private void Start() {
		StartCoroutine(Utils.DestroyAfter(gameObject, lifeTime));
	}

}