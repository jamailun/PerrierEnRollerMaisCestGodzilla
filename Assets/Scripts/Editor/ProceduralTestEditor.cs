#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProceduralTest))]
public class ProceduralTestEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		EditorGUILayout.Space();

		if(GUILayout.Button("GENERATE")) {
			ProceduralTest pt = (ProceduralTest) target;
			pt.GenerateAndPopulate();
		}
		if(GUILayout.Button("Clear buildings")) {
			ProceduralTest pt = (ProceduralTest) target;
			pt.ClearBuildings();
		}

	}

}
#endif