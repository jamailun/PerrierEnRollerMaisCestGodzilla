using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUpgrader : MonoBehaviour {

	[SerializeField] private Statistic stat;
	[SerializeField] private bool isMultiplicative = true;

	[Header("Static config")]
	[SerializeField] private Button up_button;
	[SerializeField] private TMP_Text label_price;
	[SerializeField] private TMP_Text label_effect;
	[SerializeField] private TMP_Text label_level;

	private int price;
	private int level;

	private ShopWindow shop;

	public void Start() {
		shop = GetComponentInParent<ShopWindow>();

		UpdateUI();
	}


	public void UpdateUI() {
		level = stat switch {
			Statistic.Attack => PersistentData.UpgradeLevelForce,
			Statistic.Defense => PersistentData.UpgradeLevelDefense,
			Statistic.Speed => PersistentData.UpgradeLevelSpeed,
			Statistic.ExpGained => PersistentData.UpgradeLevelIntelligence,
			Statistic.Range => PersistentData.UpgradeLevelRange,
			_ => throw new System.Exception("Unexpected stat : " + stat + " for upgrader " + name)
		};

		price = Mathf.FloorToInt(Mathf.Pow(2, level));
		up_button.interactable = PersistentData.UpgradePoints >= price;

		label_price.text = ":" + price;
		label_price.color = up_button.interactable ? Color.white : Color.red;

		if(level <= 0) {
			label_level.color = new Color(0.77f, 0.77f, 0.77f);
			label_level.text = "[Locked]";

			label_effect.text = "";
		} else {
			label_level.color = Color.white;
			label_level.text = "Level " + level;

			label_effect.text = "+" + (isMultiplicative? (GetPerLevel() * level * 100).ToString("#.##")+"%":""+ (GetPerLevel() * level));
		}
	}

	private float GetPerLevel() {
		return stat switch {
			Statistic.Attack => PersistentData.ForcePerUpgrade,
			Statistic.Defense => PersistentData.DefenPerUpgrade,
			Statistic.Speed => PersistentData.SpeedPerUpgrade,
			Statistic.ExpGained => PersistentData.IntelPerUpgrade,
			Statistic.Range => PersistentData.RangePerUpgrade,
			_ => throw new System.Exception("Unexpected stat : " + stat + " for upgrader " + name)
		};
	}

	public void ClickOnUpgrade() {
		if(price > PersistentData.UpgradePoints) {
			shop.UpdateUI();
			return;
		}

		switch(stat) {
			case Statistic.Attack:
				PersistentData.UpgradeLevelForce += 1;
				break;
			case Statistic.Defense:
				PersistentData.UpgradeLevelDefense += 1;
				break;
			case Statistic.Speed:
				PersistentData.UpgradeLevelSpeed += 1;
				break;
			case Statistic.ExpGained:
				PersistentData.UpgradeLevelIntelligence += 1;
				break;
			case Statistic.Range:
				PersistentData.UpgradeLevelRange += 1;
				break;
			default:
				Debug.LogError("ERREUR variable non liée à persistent data !! (" + stat + ", from " + gameObject.name + ")");
				return;
		}

		level++;
		PersistentData.UpgradePoints -= price;
		PersistentData.Save();

		shop.UpdateUI();
	}

}