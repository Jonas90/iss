#pragma strict

// Hannes Helmholz
//
// 


private class MeasurementFireAir extends MeasurementAbstract {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField private var DistanceTolerance:float = 0.5;
	
	private var Rooms:Volume[];
	private var CurrentData:VolumeData[];
	// =============================================================================
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Start() {
		Rooms = GameObject.FindObjectsOfType(Volume) as Volume[];
		
		CurrentData = new VolumeData[Rooms.length];
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS STATIC --------------------------------------------------------------
	
	public function GetCurrentRoomName(position:Vector3):String {
		for (var i:int = 0; i < Rooms.length; i++) {
			CurrentData[i] = Rooms[i].GetVolumeData(position);
			
			if (CurrentData[i].Distance == 0)
				return CurrentData[i].Name;
		}
		
		// if not in room ... get the nearest one
		var indexLowest:int = 0;
		var valueLowest:float = CurrentData[0].Distance;
		for (i = 1; i < CurrentData.length; i++) {
			if (CurrentData[i].Distance < valueLowest) {
				valueLowest = CurrentData[i].Distance;
				indexLowest = i;
			}
		}
		
		if (CurrentData[indexLowest].Distance <= DistanceTolerance)
			return CurrentData[indexLowest].Name;
		
		return "";
	}
	// =============================================================================
}