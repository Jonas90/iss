#pragma strict

// Hannes Helmholz
//
// nothing to do here anymore


private class AnimationRotateByTime extends AnimationRotateAbstract {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField	protected var RotationTime:float = 10;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------
	
	protected function DoFixedUpdate() {
		var x:float = (RotationAxis == Axis.X) ? 1 : 0;
		var y:float = (RotationAxis == Axis.Y) ? 1 : 0;
		var z:float = (RotationAxis == Axis.Z) ? 1 : 0;
		var step:float = Time.fixedDeltaTime * 360.0 / -RotationTime;
		
		Trans.Rotate(x * step, y * step, z * step);
	}
	// =============================================================================
}