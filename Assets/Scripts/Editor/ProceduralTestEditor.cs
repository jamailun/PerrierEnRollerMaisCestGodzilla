using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProceduralTest))]
public class ProceduralTestEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		EditorGUILayout.Space();

		if(GUILayout.Button("GENERATE")) {
			ProceduralTest pt = (ProceduralTest) target;
			pt.Generate();

		}

	}

}