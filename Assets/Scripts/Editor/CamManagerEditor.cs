using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CamManager))]
public class CamManagerEditor : Editor
{

	override public void OnInspectorGUI()
	{
		CamManager man = target as CamManager;

		DrawDefaultInspector();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("ProjectionMode");
		man.ProjectionMode = (ProjectionMode)EditorGUILayout.EnumPopup(man.ProjectionMode);
		EditorGUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Eye Separation");
		man.EyeSeparation = EditorGUILayout.FloatField(man.EyeSeparation);
		GUILayout.EndHorizontal();
	}
}
