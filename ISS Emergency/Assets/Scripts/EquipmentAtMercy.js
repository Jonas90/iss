#pragma strict

// Hannes Helmholzd
//
// 


private class EquipmentAtMercy extends Equipment {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	private enum DependenceType {OtherEquipmentNeededFirst, OtherEquipmentDepends}
	
	@SerializeField private var Dependence:DependenceType;
	@SerializeField private var OtherEquipment:EquipmentAtMercy;
	
	private var PotentionallyAvailable:boolean = false;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS ---------------------------------------------------------------------
	
	public function IsPotentionallyAvailable():boolean {
		return PotentionallyAvailable;
	}
	
	
	public function SetAvailable(available:boolean) {
		if (!available) {
			PotentionallyAvailable = false;
			super.SetAvailable(false);
			
			return;
		}
		
		if (Dependence == DependenceType.OtherEquipmentDepends) {
			PotentionallyAvailable = true;
			super.SetAvailable(true);
			
			if (OtherEquipment.IsPotentionallyAvailable())
				OtherEquipment.SetAvailable(true);
			
			return;
		}
		
		if (Dependence == DependenceType.OtherEquipmentNeededFirst) {
			PotentionallyAvailable = true;
			
			if (OtherEquipment.IsAvailable())
				super.SetAvailable(true);
				
			return;
		}		

	}
	// =============================================================================
}