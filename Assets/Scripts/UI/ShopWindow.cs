using System.Collections;
using UnityEngine;
using TMPro;

public class ShopWindow : MonoBehaviour {

	[SerializeField] private TMP_Text amountLabel;
	private ShopUpgrader[] buttons;

	private void Start() {
		UpdateUI();
	}

	public void UpdateUI() {
		amountLabel.text = ":" + PersistentData.UpgradePoints;
		if(buttons == null)
			buttons = GetComponentsInChildren<ShopUpgrader>();
		foreach(var button in buttons) {
			button.UpdateUI();
		}
	}

}