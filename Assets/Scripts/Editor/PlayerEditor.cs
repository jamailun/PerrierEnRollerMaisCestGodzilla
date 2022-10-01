using UnityEditor;
using UnityEngine;

/// <summary>
/// Allows to update player form directly in Editor.
/// </summary>
[CustomEditor(typeof(PlayerEntity))]
public class PlayerEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		EditorGUILayout.Space();

		if(GUILayout.Button("UPDATE FORM")) {
			PlayerEntity player = (PlayerEntity) target;
			if(player.PlayerForm == null) {
				EditorUtility.DisplayDialog("Erreur", "Pas de player form on the player.", "Pardon monsieur.");
				return;
			}

			player.ChangePlayerForm(player.PlayerForm);

		}

	}

}