using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(VirtualScreen))]
public class TrapezoidEditor : Editor
{

	override public void OnInspectorGUI()
	{
		VirtualScreen t = target as VirtualScreen;
		t.displayType = (DisplayType)EditorGUILayout.EnumPopup(t.displayType);

		switch (t.displayType)
		{
			case DisplayType.Screen:
				ShowScreenGUI();
				break;
			case DisplayType.Wall:
				ShowWallGUI();
				break;
		}

		t.UpdateScreenSize();
	}

	void ShowWallGUI()
	{
		VirtualScreen t = target as VirtualScreen;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("width");
		t.size.x = EditorGUILayout.FloatField(t.size.x);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("height");
		t.size.y = EditorGUILayout.FloatField(t.size.y);
		EditorGUILayout.EndHorizontal();
	}

	void ShowScreenGUI()
	{
		VirtualScreen t = target as VirtualScreen;
		GUILayoutOption labelWidth = GUILayout.Width(120);
		GUILayoutOption fieldWidth = GUILayout.Width(50);
		GUILayoutOption sepWidth = GUILayout.Width(10);

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("AspectRatio", labelWidth);
		t.aspectRatio.x = EditorGUILayout.FloatField(t.aspectRatio.x, fieldWidth);
		GUILayout.Label(":", sepWidth);
		t.aspectRatio.y = EditorGUILayout.FloatField(t.aspectRatio.y, fieldWidth);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("screen size (diagonal in inches)", labelWidth);
		t.screenSize = EditorGUILayout.FloatField(t.screenSize);
		EditorGUILayout.EndHorizontal();
	}


}
