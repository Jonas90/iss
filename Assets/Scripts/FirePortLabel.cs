
// Hannes Helmholz
//
// 

using UnityEngine;

[ExecuteInEditMode]

//zeigt Namen als Label an

public class FirePortLabel : MonoBehaviour {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	[SerializeField] private TextMesh Label;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	void Start() {
		Label.text = gameObject.name;
	}
	// =============================================================================
}