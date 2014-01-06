#pragma strict

// Hannes Helmholz
//
// manage interaction of player to manage game state
// in kombination with InputManager


@script RequireComponent(NetworkView) // for RPC

private class PlayerInteractionGame extends MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	@SerializeField	private var GameController:GameController;
	@SerializeField	private var ButtonStart:InteractionKey;
	@SerializeField	private var ButtonReset:InteractionKey;
	@SerializeField	private var ButtonStop:InteractionKey;
	
	public var ButtonQuit:String = "escape";
	
	private var Config:ConfigClass;
	private var NetView:NetworkView;
	private var LastState:GameController.GameState;  
	private var ButtonsString:String = "";
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Awake() {
		Config = GameObject.FindWithTag("Config").GetComponent.<ConfigClass>();
		
		NetView = networkView;
		LastState = GameController.GetState();
		
		ButtonStart.Initialize();
		ButtonReset.Initialize();
		ButtonStop.Initialize();
	}
	
	
	function Update() {
		if (Input.GetKey(ButtonQuit)) // can be done with every input device and also by clients
			GameController.EndGame();
	
		if (!Config.IsServer)
			return;
		
		CheckControllerChange();
		
		var state:GameController.GameState = GameController.GetState();
		if (ButtonStop.GetButtonDown() && state == GameController.GameState.Started)
			GameController.StopGame();
			
		else if (ButtonReset.GetButtonDown() && state == GameController.GameState.Started)
			GameController.RestartGame();
			
		else if (ButtonStart.GetButtonDown()
				&& (state == GameController.GameState.WaitForStart || state == GameController.GameState.WaitForFirstStart))
			GameController.StartGame();
		
		CheckStateChange();
	}
	
			
	function OnGUI() {
		GUI.skin = Config.InterfaceSkin;
		
		if (Config.OwnClientData.ShowGuiCamera || Config.IsServer)
			// top centered			- button instructions
			GUI.Label(Rect(Screen.width / 2.0, 40, 0, 0), ButtonsString, "Buttons");
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS  --------------------------------------------------------------------
	
	private function CheckControllerChange() {
		if (!Config.IsStandalone() || GameController.GetState() != GameController.GameState.WaitForFirstStart)
			return;
			
		if (Config.InputDevice != ConfigClass.Device.Keyboard && OneKeyboardKeyPressed()) {
			Config.Log("change input mode from " + Config.InputDevice + " to " + ConfigClass.Device.Keyboard);
			Config.Log("====================");
			Config.InputDevice = ConfigClass.Device.Keyboard;
		}
	}
	
	
	private function OneKeyboardKeyPressed():boolean {
		return ButtonStart.GetButtonDown(ConfigClass.Device.Keyboard) ||
				ButtonReset.GetButtonDown(ConfigClass.Device.Keyboard) ||
				ButtonStop.GetButtonDown(ConfigClass.Device.Keyboard);
	}
	
	
	private function CheckStateChange() {
		var currentState:GameController.GameState = GameController.GetState();
		if (currentState != LastState)
			SetButtonsOnAllSERVERONLY(GetButtonString());
			
		LastState = currentState;
	}
	
	
	// string for button instructions on screen by GameState
	private function GetButtonString():String {
		var text:String = "";
		var state:GameController.GameState = GameController.GetState();
		switch (state) {
			case GameController.GameState.WaitForFirstStart:
				if (Config.InputDevice != ConfigClass.Device.Keyboard)
					text += "Press  " + ButtonStart.GetCurrentButton(ConfigClass.Device.Keyboard) +
							"  to Play    and    change to Keyboard" + System.Environment.NewLine;
			case GameController.GameState.WaitForStart:
				text += "Press  " + ButtonStart.GetCurrentButton() + "  to Play";
				break;
			case GameController.GameState.Started:
				text += "Press  " + ButtonReset.GetCurrentButton() + "  to Restart      ";
				text += "Press  " + ButtonStop.GetCurrentButton() + "  to Stop";
				break;
			default:
				break;
		}
		
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