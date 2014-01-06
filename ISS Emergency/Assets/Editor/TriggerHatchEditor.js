#pragma strict

// Hannes Helmholz
//
// 


@CustomEditor(TriggerHatch)

private class TriggerHatchEditor extends Editor {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	private var Type:SerializedProperty;
	private var Trigger:SerializedProperty;
	private var Script:SerializedProperty;
	
	private var ShowList:boolean = true;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------	
	
	function OnEnable() {
		Type = serializedObject.FindProperty("Type");
		Trigger = serializedObject.FindProperty("Trigger");
		Script = serializedObject.FindProperty("Script");
    }


    function OnInspectorGUI() {
    	serializedObject.Update();
    	EditorGUIUtility.LookLikeInspector();
    	
    	EditorGUILayout.PropertyField(Type, GUIContent("Trigger Type"));
    	if (Type.enumValueIndex != 0)
    		EditorGUILayout.Space();
    	
    	var hatch:TriggerHatch = target as TriggerHatch;
    	if (Type.enumValueIndex == 0) {
    		EditorGUILayout.PropertyField(Trigger, GUIContent("Trigger Object"));
    		EditorGUILayout.Space();
    		
    		if (Trigger.enumValueIndex == 0) {
    			EditorGUILayout.LabelField("hatch will be triggered by distance to player", EditorStyles.boldLabel);
    			EditorGUILayout.LabelField("you have to add a collider component (trigger)", EditorStyles.boldLabel);

	    		hatch.AnimationsOpen = ShowOwnInspector("Animations To Open", hatch.AnimationsOpen, hatch.AnimationsOpenSize);
	    		hatch.AnimationsOpenSize = hatch.AnimationsOpen.length;
	    		hatch.AnimationsClose = ShowOwnInspector("Animations To Close", hatch.AnimationsClose, hatch.AnimationsCloseSize);
	    		hatch.AnimationsCloseSize = hatch.AnimationsClose.length;
    				
    		} else {
    			EditorGUILayout.LabelField("hatch will be triggered by script", EditorStyles.boldLabel);
    			EditorGUILayout.LabelField("first button in script will define start position", EditorStyles.boldLabel);
    			EditorGUILayout.Space();
    			
    			EditorGUILayout.PropertyField(Script, GUIContent("Buttons Script"));
    		}
    	
    	} else if (Type.enumValueIndex == 1) {
    		EditorGUILayout.LabelField("hatch will open instant on application start", EditorStyles.boldLabel);
    		EditorGUILayout.LabelField("only last animation will be played", EditorStyles.boldLabel);
    		
    		hatch.AnimationsOpen = ShowOwnInspector("Animations To Open", hatch.AnimationsOpen, hatch.AnimationsOpenSize);
    		hatch.AnimationsOpenSize = hatch.AnimationsOpen.length;
    	
    	} else if (Type.enumValueIndex == 2) {
    		EditorGUILayout.LabelField("hatch will open on application start", EditorStyles.boldLabel);
    		
    		hatch.AnimationsOpen = ShowOwnInspector("Animations To Open", hatch.AnimationsOpen, hatch.AnimationsOpenSize);
    		hatch.AnimationsOpenSize = hatch.AnimationsOpen.length;
    	
    	} else
    		EditorGUILayout.LabelField("hatch will stay locked", EditorStyles.boldLabel);
        
        EditorGUIUtility.LookLikeControls();
        serializedObject.ApplyModifiedProperties();
    }
    // =============================================================================
    
    
    private function ShowOwnInspector(title:String, animations:AnimationClip[], size:int):AnimationClip[] {	
		EditorGUILayout.Space();
		
		EditorGUIUtility.LookLikeControls();
		ShowList = EditorGUILayout.Foldout(ShowList, title);
		EditorGUIUtility.LookLikeInspector();
		
		if (ShowList) {
			var x:int = 0;
			
			size = EditorGUILayout.IntField("Size", size);
			if (animations.length != size) {
				var newArray:AnimationClip[] = new AnimationClip[size];
				for (x = 0; x < size; x++)
					if (animations.length > x)
						newArray[x] = animations[x];

				animations = newArray;
			}
			
			for (x = 0; x < animations.length; x++)
				animations[x] = EditorGUILayout.ObjectField("Element " + x, animations[x], typeof(AnimationClip), true) as AnimationClip;
		}

		return animations;
    }
}