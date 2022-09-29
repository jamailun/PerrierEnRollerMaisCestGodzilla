using UnityEngine;
using System.Collections.Generic;

public class EvolveScreen : MonoBehaviour {

	private EvolveButton[] buttons;

	public delegate void FormCallback(PlayerForm skill);

	private FormCallback callback;

	private void Start() {
		if(buttons == null)
			Init();
	}

	private void Init() {
		buttons = GetComponentsInChildren<EvolveButton>();
		if(buttons == null || buttons.Length == 0)
			Debug.LogError("No EvolveButton for EvolveScreen.");
		if(buttons.Length != 3)
			Debug.LogError("Excpected 3 buttons for EvolveScreen.");
	}

	public void FormChose(PlayerForm skill) {
		Debug.Log("chose skill " + skill);

		// call the callback at the end.
		callback?.Invoke(skill);
	}

	public void DisplayForms(IEnumerable<PlayerForm> forms, FormCallback callback) {
		if(buttons == null)
			Init();

		this.callback = callback;

		// remplir les skills
		int i = 0;
		foreach(var f in forms) {
			buttons[i].SetPlayerForm(f);
			i++;
		}
		// remplir les trous
		for(; i < buttons.Length; i++) {
			buttons[i].SetPlayerForm(null);
		}

	}
	
}