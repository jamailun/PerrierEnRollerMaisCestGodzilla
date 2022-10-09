using UnityEngine;

public class Tooltip : MonoBehaviour {

	private static Tooltip Instance;

	[SerializeField] private RectTransform background;
	[SerializeField] private TMPro.TMP_Text tooltip;
	[SerializeField] private Vector2 mouseOffset;

	private RectTransform parentRectTransform;
	private Vector2 localPoint;

	private void Start() {
		if(Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		parentRectTransform = transform.parent.GetComponent<RectTransform>();
		HideTooltip();
	}


	private void Update() {
		RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, Input.mousePosition, Camera.current, out localPoint);
		transform.localPosition = localPoint + mouseOffset;
	}

	public static void ShowTooltip(string text) {
		if(Instance == null) {
			Debug.LogWarning("No tooltip set for the scene !");
			return;
		}
		Instance.gameObject.SetActive(true);
		Instance.tooltip.text = text;
	}

	public static void HideTooltip() {
		if(Instance != null)
			Instance.gameObject.SetActive(false);
	}

}