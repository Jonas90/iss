using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CamGroup))]
public class CamGroupEditor : Editor
{

	override public void OnInspectorGUI()
	{
		DrawDefaultInspector();

		CamGroup group = target as CamGroup;

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Screen");
		group.Screen = EditorGUILayout.ObjectField(group.Screen, typeof(VirtualScreen), true) as VirtualScreen;
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("ProjectionMode");
		group.ProjectionMode = (ProjectionMode)EditorGUILayout.EnumPopup(group.ProjectionMode);
		EditorGUILayout.EndHorizontal();
	}
}
