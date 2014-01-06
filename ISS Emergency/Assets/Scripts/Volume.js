#pragma strict

// Hannes Helmholz
//
// 


@script RequireComponent(Collider)

private class Volume extends MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField private var Name:String;
	
	private var Coll:Collider;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Awake() {
		Coll = collider;
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------
	
	public function GetVolumeData(point:Vector3):VolumeData {
		var distance:float = Coll.bounds.SqrDistance(point);
		return VolumeData(Name, distance);
	}
	// =============================================================================
}



private class VolumeData extends System.ValueType {
	public var Name:String;
	public var Distance:float;
	
	public function VolumeData(name:String, distance:float) {
		this.Name = name;
		this.Distance = distance;
	}
}