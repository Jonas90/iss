#pragma strict

// Hannes Helmholz
//
// 


@script ExecuteInEditMode()

private class UniqueIDManager extends MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	public var UniqueIDs:UniqueID[];
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Start() {
		UniqueIDs = GameObject.FindObjectsOfType(UniqueID) as UniqueID[];
		for (var i:int = 0; i < UniqueIDs.length; i++)
			UniqueIDs[i].ID = i;
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------
	
	public function GetGameObjectByID(id:int):GameObject {
		return UniqueIDs[id].gameObject;
	}
	// =============================================================================
}