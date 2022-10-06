﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour {

	public void SelectedRagequit() {
		Debug.Log("Ragequit. Go to main menu");

		SceneManager.LoadScene(0);
	}

	public void SelectedShop() {
		Debug.Log("shop selected. mais ça n'existe pas. donc go au shop.");

		SceneManager.LoadScene(0);
	}
	
}