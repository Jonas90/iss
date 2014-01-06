#pragma strict

// Hannes Helmholz
//
// give different possibilities to interact with scene
// with different key for every input device
// in combination with InputManager


private class InteractionKey extends System.Object {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	public enum MouseKey {Left = 0, Right = 1, Middle = 2, WheelUp = 3, WheelDown = 4, None = 5}
	
	@SerializeField private var ButtonMouse:MouseKey = MouseKey.None;
	@SerializeField private var ButtonKeyboard:String = ""; // optional
	@SerializeField private var ButtonXbox:String = ""; // optional
	@SerializeField private var ButtonSpacepilot:String = ""; // optional
	
	private var Config:ConfigClass;
	// =============================================================================


	
	// =============================================================================
	// METHODS  --------------------------------------------------------------------
	
	// must be used because Start() is not available (derives not from MonoBehaviour)
	public function Initialize() {
		Config = GameObject.FindWithTag("Config").GetComponent.<ConfigClass>();
	}
	
	
	public function GetButtonDown(device:ConfigClass.Device):boolean {
		if (device == Config.Device.Keyboard && ButtonMouse != MouseKey.None) {
			var mouse:boolean;
			var buttonNumber:int = ButtonMouse;
			if (ButtonMouse == MouseKey.WheelUp)
				mouse = Input.GetAxis("Mouse ScrollWheel") > 0;
			else if (ButtonMouse == MouseKey.WheelDown)
				mouse = Input.GetAxis("Mouse ScrollWheel") < 0;
			else
				mouse = Input.GetMouseButtonDown(buttonNumber);
			
	    	return Input.GetButtonDown(ButtonKeyboard) || mouse;
	    }
	
		var button:String = GetCurrentButton(device);
		if (button.Length == 0)
			return false;

	    return Input.GetButtonDown(button);
	}
	
	
	public function GetButtonDown():boolean {
		return GetButtonDown(Config.InputDevice);
	}
	
	
	public function GetCurrentButton(device:ConfigClass.Device):String {
		return GetCurrentButton(device, "  /  ");
	}
	
	
	public function GetCurrentButton(device:ConfigClass.Device, separator:String):String {
		var button:String = "";
		
		// in case of NullReferenceException you forgot to call Initialize() perhaps
		switch (device) {
			case Config.Device.Spacepilot:
				button = ButtonSpacepilot;
				break;
				
			case Config.Device.Xbox:
				button = ButtonXbox;
				break;
				
			case Config.Device.Keyboard:
				button = ButtonKeyboard;
				if (ButtonMouse != MouseKey.None)
					button += separator + "Mouse" + ButtonMouse;
				break;
		}
		
		return button;
	}
	
	
	public function GetCurrentButton():String {
		return GetCurrentButton(Config.InputDevice);
	}
	
	
	public function GetCurrentButton(separator:String):String {
		return GetCurrentButton(Config.InputDevice, separator);
	}
	// =============================================================================
}