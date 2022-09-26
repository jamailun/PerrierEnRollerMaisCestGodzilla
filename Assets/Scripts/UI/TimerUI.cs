using UnityEditor;
using UnityEngine;

public class TimerUI : MonoBehaviour {

	[SerializeField] private TMPro.TMP_Text text;

	private void FixedUpdate() {
		int seconds = (int) Time.time;
		int mins = seconds / 60;
		seconds %= 60;

		text.text = (mins < 10 ? "0" : "") + mins + ":" + (seconds < 10 ? "0":"") + seconds;
	}

}