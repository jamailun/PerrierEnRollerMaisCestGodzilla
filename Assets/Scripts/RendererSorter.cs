using System.Collections;
using UnityEngine;

/// <summary>
/// From the video of CodeMoney https://www.youtube.com/watch?v=CTf0WjhfBx8.
/// Used to sort elements on their Y axis.
/// </summary>
public class RendererSorter : MonoBehaviour {

	[Header("Sorting attributes.")]
	[SerializeField] private int sortingOrderBase = 5000;
	[SerializeField] private int sortOffset = 0;
	[SerializeField] private bool runSortOnlyOnce = true;
	private Renderer _renderer;

	protected virtual void Start() {
		_renderer = GetComponent<Renderer>();
		if(_renderer == null)
			_renderer = GetComponentInChildren<Renderer>();
		Debug.Log("_renderer = " + _renderer);
	}

	private void LateUpdate() {
		
		// If this causes performance issues, we can put a timer.

		_renderer.sortingOrder = (int) (sortingOrderBase - transform.position.y - sortOffset);
		if(runSortOnlyOnce)
			Destroy(this); // destroy the COMPONENT.
	}

}