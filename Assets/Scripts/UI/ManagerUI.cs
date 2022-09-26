using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage UIs. Define the keys and UI here in the scene.
/// </summary>
public class ManagerUI : MonoBehaviour {

	[Tooltip("The map of keys and windows.")]
	[SerializeField] private ManagerEntry[] windows;

	[System.Serializable]
	public struct ManagerEntry {
		[SerializeField] public KeyCode keyCode;
		[SerializeField] public WindowUI window;
	}

	private void Awake() {
		DontDestroyOnLoad(this);
	}

	private void Update() {
		foreach(var w in windows) {
			if(Input.GetKeyDown(w.keyCode)) {
				bool newIsActive = !w.window.gameObject.activeSelf;
				//ClearPopups();
				w.window.gameObject.SetActive(newIsActive);
				if(newIsActive)
					w.window.effector?.Invoke();
			}
		}
	}


}
