#pragma strict

// Hannes Helmholz
//
// loader script for application
// to show image while big scene with iss is loading


private class LevelLoader extends MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField	private var Cam:Camera;
	@SerializeField	private var FadeSpeed:float = 1.0;
	
	private var This:GameObject;
	private var Async:AsyncOperation;
	private var LoadedFirst:boolean = false;
	private var LoadedSecond:boolean = false;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Start() {
		if (!Application.isEditor)
			Screen.lockCursor = true;
		
		This = gameObject;
		
		Async = Application.LoadLevelAdditiveAsync(1); // start loading
		Async.allowSceneActivation = false;
		yield Async;
	}
	
	
	function Update() {
		if (Async.progress >= 0.9 && !LoadedFirst) // value progress will stop at 0.9 -> scene is ready
			AllowStart();
			
		if (Async.isDone && !LoadedSecond) // value progress is 1.0 -> scene is full loaded
			FadeImage();
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS  --------------------------------------------------------------------
	
	private function AllowStart() {
		LoadedFirst = true;
		Async.allowSceneActivation = true;
	}
	
	
	// do fade with camere field of view
	// and destroy this gameObject
	private function FadeImage() {
		Cam.fieldOfView -= FadeSpeed;
			
		if (Cam.fieldOfView <= 1) {
			LoadedSecond = true;
			Destroy(This, 1);
		}
	}
	// =============================================================================
}