#pragma strict

// Hannes Helmholz
//
// 


private class MeterFireAir extends MeterAbstract {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField	private var Button:InteractionKey; // optional

	private var MeasurementScript:MeasurementFireAir;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Awake() {
		super.Awake();
		
		Button.Initialize();
		MeasurementScript = GameObject.FindObjectOfType(MeasurementFireAir);
	}
	
	
	function Update() {
		if (!Config.IsServer)
			return;
			
		if (Button.GetButtonDown() && IsEquipped())
			StartCoroutine(YieldMeasure());
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------
	
	private function YieldMeasure() {
		yield StartCoroutine(MeasureValue());
	}
	
	
	protected function MeasureValue() {
		if (!IsAvailable())
			return;
		
		yield WaitForSeconds(1);
		
		var room:String = MeasurementScript.GetCurrentRoomName(Trans.position);
		var values:MeasurementValues = MeasurementScript.GetValues(room);
		var text:String = values ? values.ToString() : "ERR";
		ConfigClass.Log("AIR room: " + room + " values " + text);
		DisplayValues(values);
		
		Audio.clip = room.Length == 0 ? SoundError : SoundSuccess;
		yield super.MeasureValues(2.5); // play sound
		
		DisplayValues("", "");
	}
	// =============================================================================
}