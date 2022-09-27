using UnityEngine;

public class TimerUI : MonoBehaviour {

	[SerializeField] private TMPro.TMP_Text text;

	private static TimerUI instance;
	private float started = 0;
	private bool stopped = false;

	private void Start() {
		if(instance != null)
			Destroy(instance);
		instance = this;
	}

	public static void StartTimer() {
		if(instance != null)
			instance.started = Time.time;
	}

	public static void Stop() {
		if(instance != null)
			instance.stopped = true;
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