using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This UI class is used to represent any value in a bar.
/// </summary>
public class BarUI : MonoBehaviour {

	private const string FORMAT = "#";

	[SerializeField] private Image content;
	[SerializeField] private TMPro.TMP_Text label;
	[SerializeField] private float MinValue = 0f;
	[SerializeField] private float MaxValue = 100f;
	private float value;

	private void Start() {
		value = MaxValue;
		//UpdateImage();
	}

	/// <summary>
	/// Init the bar from the code.
	/// </summary>
	/// <param name="minValue">The minimal value of the bar.</param>
	/// <param name="maxValue">The maximal value of the bar.</param>
	/// <param name="value">The current value of the bar.</param>
	public void Init(float minValue, float maxValue, float value) {
		this.MinValue = minValue;
		this.MaxValue = maxValue;
		SetValue(value); // this will also update the image.
	}

	public void SetValue(float value) {
		this.value = Mathf.Max(MinValue, Mathf.Min(MaxValue, value));
		UpdateImage();
	}

	private void UpdateImage() {
		if(label != null) {
			if(value == 0)
				label.text = "0/"+MaxValue.ToString(FORMAT);
			else
				label.text = value.ToString(FORMAT) + "/" + MaxValue.ToString(FORMAT);
		}
		if(content == null)
			return;
		if(MaxValue == 0) {
			content.fillAmount = 0f;
			return;
		}
		content.fillAmount = (value - MinValue) / MaxValue;
	}

}
