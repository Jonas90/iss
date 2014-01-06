#pragma strict

// Hannes Helmholz
//
// 


@CustomEditor(GuiScreenPosition)
@CanEditMultipleObjects

public class GuiScreenPositionEditor extends Editor {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	private var PositionAnchor:SerializedProperty;
	private var CenteredSpacing:SerializedProperty;
	private var CenteredWidth:SerializedProperty;
	private var AbsoluteSpacingLeft:SerializedProperty;
	private var AbsoluteSpacingBottom:SerializedProperty;
	private var AbsoluteWidth:SerializedProperty;
	
	private var DifferentInStandalone:SerializedProperty;
	
	private var PositionAnchorStandalone:SerializedProperty;
	private var CenteredSpacingStandalone:SerializedProperty;
	private var CenteredWidthStandalone:SerializedProperty;
	private var AbsoluteSpacingLeftStandalone:SerializedProperty;
	private var AbsoluteSpacingBottomStandalone:SerializedProperty;
	private var AbsoluteWidthStandalone:SerializedProperty;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function OnEnable() {
		PositionAnchor = serializedObject.FindProperty("PositionAnchor");
		CenteredSpacing = serializedObject.FindProperty("CenteredSpacing");
		CenteredWidth = serializedObject.FindProperty("CenteredWidth");
		AbsoluteSpacingLeft = serializedObject.FindProperty("AbsoluteSpacingLeft");
		AbsoluteSpacingBottom = serializedObject.FindProperty("AbsoluteSpacingBottom");
		AbsoluteWidth = serializedObject.FindProperty("AbsoluteWidth");
		
    	DifferentInStandalone = serializedObject.FindProperty("DifferentInStandalone");
    	
    	PositionAnchorStandalone = serializedObject.FindProperty("PositionAnchorStandalone");
		CenteredSpacingStandalone = serializedObject.FindProperty("CenteredSpacingStandalone");
		CenteredWidthStandalone = serializedObject.FindProperty("CenteredWidthStandalone");
		AbsoluteSpacingLeftStandalone = serializedObject.FindProperty("AbsoluteSpacingLeftStandalone");
		AbsoluteSpacingBottomStandalone = serializedObject.FindProperty("AbsoluteSpacingBottomStandalone");
		AbsoluteWidthStandalone = serializedObject.FindProperty("AbsoluteWidthStandalone");
    }


    function OnInspectorGUI() {
    	serializedObject.Update();
    	
    	EditorGUILayout.Space();
    	if (DifferentInStandalone.boolValue)
    		EditorGUILayout.LabelField("Position in CAVE", EditorStyles.boldLabel);
    	else
    		EditorGUILayout.LabelField("Position in CAVE and Standalone", EditorStyles.boldLabel);
    	
    	EditorGUILayout.PropertyField(PositionAnchor, GUIContent("Anchor"));
    	if (PositionAnchor.enumValueIndex < 2) {
    		var text:String = (PositionAnchor.enumValueIndex == 0) ? "Spacing Top" : "Spacing Bottom";
    		EditorGUILayout.PropertyField(CenteredSpacing, GUIContent(text));
    		EditorGUILayout.PropertyField(CenteredWidth, GUIContent("Width"));
    	} else {
    		EditorGUILayout.PropertyField(AbsoluteSpacingLeft, GUIContent("Spacing Left"));
    		EditorGUILayout.PropertyField(AbsoluteSpacingBottom, GUIContent("Spacing Bottom"));
    		EditorGUILayout.PropertyField(AbsoluteWidth, GUIContent("Width"));
    	}
    	
    	EditorGUILayout.Space();
    	EditorGUILayout.PropertyField(DifferentInStandalone);
        if (DifferentInStandalone.boolValue) {
        	
        	EditorGUILayout.Space();
    		EditorGUILayout.LabelField("Position in Standalone", EditorStyles.boldLabel);
        	
        	EditorGUILayout.PropertyField(PositionAnchorStandalone, GUIContent("Anchor Standalone"));
	    	if (PositionAnchorStandalone.enumValueIndex < 2) {
	    		var text1:String = (PositionAnchorStandalone.enumValueIndex == 0) ? "Spacing Top" : "Spacing Bottom";
	    		EditorGUILayout.PropertyField(CenteredSpacingStandalone, GUIContent(text1));
	    		EditorGUILayout.PropertyField(CenteredWidthStandalone, GUIContent("Width"));
	    	} else {
	    		EditorGUILayout.PropertyField(AbsoluteSpacingLeftStandalone, GUIContent("Spacing Left"));
	    		EditorGUILayout.PropertyField(AbsoluteSpacingBottomStandalone, GUIContent("Spacing Bottom"));
	    		EditorGUILayout.PropertyField(AbsoluteWidthStandalone, GUIContent("Width"));
	    	}
        }
        
        serializedObject.ApplyModifiedProperties();
    }
    // =============================================================================
}