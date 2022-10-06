using UnityEngine;

public class TimerUI : MonoBehaviour {

	[SerializeField] private TMPro.TMP_Text text;

	private static TimerUI Instance;
	private float started = 0;
	private bool stopped = false;

	private void Start() {
		if(Instance != null) {
			Destroy(this);
			return;
		}
		Instance = this;
	}

	public static void StartTimer() {
		if(Instance != null)
			if(Instance.stopped)
				Instance.stopped = false;
			else
				Instance.started = Time.time;
	}

	public static void UnpauseTimer() {
		if(Instance != null)
			Instance.stopped = false;
	}

	public static void Stop() {
		if(Instance != null)
			Instance.stopped = true;
	}

	private void FixedUpdate() {
		if(stopped)
			return;

		int seconds = Mathf.FloorToInt(Time.time - started);
		int mins = seconds / 60;
		seconds %= 60;

		text.text = (mins < 10 ? "0" : "") + mins + ":" + (seconds < 10 ? "0":"") + seconds;
	}

}