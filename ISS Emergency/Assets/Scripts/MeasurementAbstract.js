#pragma strict

// Hannes Helmholz
//
// 


private class MeasurementValues extends System.Object {
	
	public var O2:float;
	public var CO:float;
	public var HCN:float;
	public var HCL:float;
	
	public function MeasurementValues(o2:float, co:float, hcn:float, hcl:float) {
		this.O2 = o2;
		this.CO = co;
		this.HCN = hcn;
		this.HCL = hcl;
	}
	
	
	public function ToString():String {
		return O2 + " O2  " + CO + " CO  " + HCN + " HCN  " + HCL + " HCL";
	}
}


private /*abstract*/ class MeasurementAbstract extends MonoBehaviour {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField private var DataFile:String = "*.conf";
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------
	
	public function GetValues(name:String):MeasurementValues {
		if (!name || name.Length == 0)
			return;
		
		var values:MeasurementValues;
		var sr:System.IO.StreamReader;
		try {
	        sr = new System.IO.StreamReader(DataFile);
			
			var found:boolean = false;		
		    var line:String = sr.ReadLine();
			while (line != null && !found) {
				line = line.Split("|"[0])[0]; // without comments
				
				if (line.Length <= 0) {
					line = sr.ReadLine(); continue; }
				
				var parts:String[] = line.Split(";"[0]);
				for (var part:String in parts) {
					part = part.Replace(' ', ''); // delete all whitespaces
				}
				
				if (parts.Length < 5) {
					line = sr.ReadLine(); continue; }
				
				if (parts[0].Equals(name)) {
					values = MeasurementValues(parseFloat(parts[1]), parseFloat(parts[2]), parseFloat(parts[3]), parseFloat(parts[4]));
					found = true;
				}
				
				line = sr.ReadLine();
		    }
	    } catch (e)
	    	ConfigClass.Log(e.ToString(), true);
	    finally
	    	sr.Close();
	    	
	    return values;
	}
	// =============================================================================
}