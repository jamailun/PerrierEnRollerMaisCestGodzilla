using System.Collections;
using UnityEngine;

public class SkillShot : MonoBehaviour {

	[SerializeField] private float waitBeforeActivate;
	[SerializeField] private float waitBeforeDestroy;

	[SerializeField] private Activable activablePart;

	private void Start() {
		StartCoroutine(DoTruc());
	}

	private float damages, radius;

	public void Init(float damages = 1, float radius = 1) {
		this.damages = damages;
		this.radius = radius;
	}

	private IEnumerator DoTruc() {
		yield return new WaitForSeconds(waitBeforeActivate);

		if(activablePart != null) {
			activablePart.enabled = false;
			activablePart.gameObject.SetActive(true);
			activablePart.Activate(damages, radius);
			activablePart.enabled = true;
		}

		var sr = GetComponent<SpriteRenderer>();
		if(sr == null)
			sr = GetComponentInChildren<SpriteRenderer>();
		if(sr)
			sr.enabled = false;

		yield return new WaitForSeconds(waitBeforeDestroy);

		if(gameObject != null)
			Destroy(gameObject);
	}

}