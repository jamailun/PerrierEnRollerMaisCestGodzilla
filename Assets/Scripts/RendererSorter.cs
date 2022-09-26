using System.Collections;
using UnityEngine;

/// <summary>
/// From the video of CodeMoney https://www.youtube.com/watch?v=CTf0WjhfBx8.
/// Used to sort elements on their Y axis.
/// </summary>
public class RendererSorter : MonoBehaviour {

	[SerializeField] private int sortingOrderBase = 5000;
	private Renderer _renderer;

	private void Start() {
		_renderer = GetComponent<Renderer>();
		if(_renderer == null)
			_renderer = GetComponentInChildren<Renderer>();
		Debug.Log("_renderer = " + _renderer);
	}

	private void LateUpdate() {
		_renderer.sortingOrder = (int) (sortingOrderBase - transform.position.y);
	}

}