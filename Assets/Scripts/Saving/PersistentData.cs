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

	// Stats d'upgrade
	public static int UpgradeLevelForce { set; get; }
	public static int UpgradeLevelDefense { set; get; }
	public static int UpgradeLevelSpeed { set; get; }
	public static int UpgradeLevelIntelligence { set; get; }
	public static int UpgradeLevelRange { set; get; }

	// Dynamic
	public static int UpgradePoints { set; get; }

	public void Load() {

		PlayedTime = PlayerPrefs.GetFloat(Key("playerTime"), 0f);
		RunsAmount = PlayerPrefs.GetInt(Key("runsAmount"), 0);
		UpgradePoints = PlayerPrefs.GetInt(Key("upgradePoints"), 0);
		BestLevel = PlayerPrefs.GetInt(Key("bestLevel"), 0);

		UpgradeLevelForce = PlayerPrefs.GetInt(Key("upgradeLevelForce"), 0);
		UpgradeLevelDefense = PlayerPrefs.GetInt(Key("upgradeLevelDefense"), 0);
		UpgradeLevelSpeed = PlayerPrefs.GetInt(Key("upgradeLevelSpeed"), 0);
		UpgradeLevelIntelligence = PlayerPrefs.GetInt(Key("upgradeLevelIntelligence"), 0);
		UpgradeLevelRange = PlayerPrefs.GetInt(Key("upgradeLevelRange"), 0);

		Debug.Log("PersistentData read " + RunsAmount + " runs.");
	}

	public static void Save() {
		PlayerPrefs.SetFloat(Key("playerTime"), PlayedTime);
		PlayerPrefs.SetInt(Key("runsAmount"), RunsAmount);
		PlayerPrefs.SetInt(Key("upgradePoints"), UpgradePoints);
		PlayerPrefs.SetInt(Key("bestLevel"), BestLevel);

		PlayerPrefs.SetInt(Key("upgradeLevelForce"), UpgradeLevelForce);
		PlayerPrefs.SetInt(Key("upgradeLevelDefense"), UpgradeLevelDefense);
		PlayerPrefs.SetInt(Key("upgradeLevelSpeed"), UpgradeLevelSpeed);
		PlayerPrefs.SetInt(Key("upgradeLevelIntelligence"), UpgradeLevelIntelligence);
		PlayerPrefs.SetInt(Key("upgradeLevelRange"), UpgradeLevelRange);

		PlayerPrefs.Save();
		Debug.Log("Saved persistent data");

	}

	public static void EndRun(float runDuration, int level, uint upgradePoints) {
		PlayedTime += runDuration;
		UpgradePoints += (int) upgradePoints;
		BestLevel = Mathf.Max(level, BestLevel);
		RunsAmount++;

		Save();
	}

	private static string Key(string local) {
		return accountName + "." + local;
	}




}