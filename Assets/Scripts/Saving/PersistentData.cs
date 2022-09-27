using UnityEngine;

public class PersistentData {

	// Load data at startup. Can be replaced in a better way i guess.
	private static PersistentData INSTANCE = new();
	private PersistentData() {
		Load();
	}

	private static readonly string accountName = "perier";

	// Incremental

	public static float PlayedTime { set; get; }
	public static int RunsAmount { set; get; }
	public static int BestLevel { set; get; }

	// Dynamic
	public static int UpgradePoints { set; get; }

	public void Load() {
		PlayedTime = PlayerPrefs.GetFloat(Key("playerTime"), 0f);
		RunsAmount = PlayerPrefs.GetInt(Key("runsAmount"), 0);
		UpgradePoints = PlayerPrefs.GetInt(Key("upgradePoints"), 0);
		BestLevel = PlayerPrefs.GetInt(Key("bestLevel"), 0);

		Debug.Log("read " + RunsAmount);
	}

	public static void Save() {
		PlayerPrefs.SetFloat(Key("playerTime"), PlayedTime);
		PlayerPrefs.SetInt(Key("runsAmount"), RunsAmount);
		PlayerPrefs.SetInt(Key("upgradePoints"), UpgradePoints);
		PlayerPrefs.SetInt(Key("bestLevel"), BestLevel);
	}

	public static void EndRun(float runDuration, int level, uint upgradePoints) {
		PlayedTime += runDuration;
		UpgradePoints += (int) upgradePoints;
		BestLevel = Mathf.Max(level, BestLevel);
		RunsAmount++;
		// à retirer ? jsp
		Save();
	}

	private static string Key(string local) {
		return accountName + "." + local;
	}



}