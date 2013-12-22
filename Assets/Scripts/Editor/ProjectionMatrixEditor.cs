using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ProjectionMatrix))]
public class ProjectionMatrixEditor : Editor
{

	override public void OnInspectorGUI()
	{
		Debug.Log(System.Environment.StackTrace);
		DrawDefaultInspector();

		ProjectionMatrix pm = target as ProjectionMatrix;
		pm.ProjectionMode = (ProjectionMode)EditorGUILayout.EnumPopup("ProjectionMode", pm.projectionMode);

		//		if (GUILayout.Button("Update Homography")) {
		//			ProjectionMatrix pm = target as ProjectionMatrix;
		//			
		//			pm.LoadBimberMatrix(pm.pathToMatrixFile);
		//		}
	}
}
