using UnityEngine;

public class PersistentData {

	// Load data at startup. Can be replaced in a better way i guess.
	private static PersistentData INSTANCE = new();
	private PersistentData() {
		Load();
	}

	private static readonly string accountName = "perier";

	public static float PlayedTime { set; get; }
	public static int RunsAmount { set; get; }
	public static int UpgradePoints { set; get; }

	public void Load() {
		PlayedTime = PlayerPrefs.GetFloat(Key("playerTime"), 0f);
		RunsAmount = PlayerPrefs.GetInt(Key("runsAmount"), 0);
		UpgradePoints = PlayerPrefs.GetInt(Key("upgradePoints"), 0);

		Debug.Log("read " + RunsAmount);
	}

	public static void Save() {
		PlayerPrefs.SetFloat(Key("playerTime"), PlayedTime);
		PlayerPrefs.SetInt(Key("runsAmount"), RunsAmount);
		PlayerPrefs.SetInt(Key("upgradePoints"), UpgradePoints);
	}

	public static void EndRun(float runDuration, int upgradePoints) {
		PlayedTime += runDuration;
		UpgradePoints += upgradePoints;
		RunsAmount++;
		// à retirer ? jsp
		Save();
	}

	private static string Key(string local) {
		return accountName + "." + local;
	}



}