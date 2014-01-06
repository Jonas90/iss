#pragma strict

// Hannes Helmholz
//
// nothing to do here anymore


private class AnimationRotateByStep extends AnimationRotateAbstract {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField	protected var AngleStep:float = 45;
	@SerializeField	protected var TimeStep:float = 0.25;
	
	protected var LastStep:float = 0;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Start () {
		super.Start();
	
		LastStep = Time.time;
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------
	
	protected function DoFixedUpdate() {
		if (LastStep + TimeStep <= Time.time) {
			LastStep = Time.time;
			
			var x:float = (RotationAxis == Axis.X) ? 1 : 0;
			var y:float = (RotationAxis == Axis.Y) ? 1 : 0;
			var z:float = (RotationAxis == Axis.Z) ? 1 : 0;
			Trans.Rotate(x * AngleStep, y * AngleStep, z * AngleStep);
		}
	}
	// =============================================================================
}