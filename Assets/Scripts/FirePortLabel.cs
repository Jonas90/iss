
// Hannes Helmholz
//
// 

using UnityEngine;

[ExecuteInEditMode]

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