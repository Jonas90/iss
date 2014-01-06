#pragma strict

// Hannes Helmholz
//
// give different possibilities to interact with scene
// with different key for every input device
// in combination with InputManager


@script RequireComponent(NetworkView) // for RPC

private class EquipmentManager extends MonoBehaviour {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	private enum HelpState {NoEquipment, ShowHelp, HideHelp}
	
	@SerializeField private var ButtonNext:InteractionKey; // optional
	@SerializeField private var ButtonLast:InteractionKey; // optional
	@SerializeField private var AdditionalEquipment:Equipment[]; // optional
	
	private var Config:ConfigClass;
	private var NetView:NetworkView;
	private var Equipments:Equipment[];
	private var CurrentShown:int = -1;
	private var State:HelpState;
	private var ButtonsString:String = "";
	// =============================================================================



	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Start() {
		Config = GameObject.FindWithTag("Config").GetComponent.<ConfigClass>();
		
		NetView = networkView;
		
		Equipments = gameObject.GetComponentsInChildren.<Equipment>();
		var oldSize:int = Equipments.length;
		System.Array.Resize.<Equipment>(Equipments, oldSize + AdditionalEquipment.length);
		for (var i:int = 0; i < AdditionalEquipment.length; i++)
			Equipments[oldSize + i] = AdditionalEquipment[i];
		
		ButtonNext.Initialize();
		ButtonLast.Initialize();
	}
	
	
	function Update() {
		if (!Config.IsServer || !Config.IsGameStarted)
			return;
			
		if (ButtonNext.GetButtonDown())
			Next();
		else if (ButtonLast.GetButtonDown())
			Last();
	}
	
	
	function OnGUI() {
		GUI.skin = Config.InterfaceSkin;
		
		if (Config.OwnClientData.AngleOffset == 0 || Config.IsServer)
			// bottom centered			- button instructions
			GUI.Label(Rect(Screen.width / 2.0, Screen.height - 50, 0, 0), ButtonsString, "Buttons");
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS  --------------------------------------------------------------------

	public function Take(equipment:Equipment) {
		if (!equipment)
			return;
		
		equipment.SetAvailable(true); // see also EquipmentAtMercy
		
		if (equipment.IsAvailable()) // important for EquipmentAtMercy
			for (var i:int = 0; i < Equipments.length; i++)
				if (Equipments[i] == equipment) {
					Equipments[i].TriggerStored();
					
					if (!(Equipments[i] as EquipmentAsGui)) {
						if (CurrentShown >= 0)
							Equipments[CurrentShown].TriggerStored();
					
						CurrentShown = i;
						
						if (State == HelpState.NoEquipment) {
							State = HelpState.ShowHelp;
							SetButtonsOnAllSERVERONLY(GetButtonString());
						}
					} 
					
					continue;
				}
	}


	private function Next() {
		for (var i:int = CurrentShown + 1; i < Equipments.length; i++)
			if (TryChange(i))
				return;
				
		for (i = -1; i < CurrentShown; i++)
			if (TryChange(i))
				return;
	}
	
	
	private function Last() {
		for (var i:int = CurrentShown - 1; i >= -1; i--)
			if (TryChange(i))
				return;
				
		for (i = Equipments.length - 1; i > CurrentShown; i--)
			if (TryChange(i))
				return;
	}
	
	
	private function TryChange(i:int):boolean {
		if (i == -1 || (Equipments[i].IsAvailable() && !Equipments[i].IsEquipped())) {
			if (CurrentShown > -1)
				Equipments[CurrentShown].TriggerStored();
				
			if (i > -1)
				Equipments[i].TriggerStored();
				
			CurrentShown = i;
			
			if (State == HelpState.ShowHelp) {
				State = HelpState.HideHelp;
				SetButtonsOnAllSERVERONLY("");
			}
			
			return true;
		}
		
		return false;
	}
	
	
	// string for button instructions on screen by GameState
	private function GetButtonString():String {
		var text:String = "";
		text += "Press  " + ButtonLast.GetCurrentButton() + "   or   ";
		text += ButtonNext.GetCurrentButton() + "  to change equipment";
		
		return text;
	}
	
	
	// show button instructions and call rpc to show button instructions on all clients also
	//TODO dont know if it can be used by clients
	private function SetButtonsOnAllSERVERONLY(text:String) {
		NetView.RPC("RPCSetButtonsOnAllSERVERONLY", RPCMode.AllBuffered, text);
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS RPC -----------------------------------------------------------------
	
	@RPC
	function RPCSetButtonsOnAllSERVERONLY(text:String) {
		ButtonsString = text;
	}
	// =============================================================================
}