#pragma strict

// Hannes Helmholz
//
// config contains all client and server informations
// afford a static log function in gui and file for debug use in cave
// used in nearly every other script


@script RequireComponent(NetworkView); // for RPC

private class ConfigClass extends MonoBehaviour {

	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	public enum Device {None, Keyboard, Xbox, Spacepilot}
	
	@HideInInspector	public var ClientData:Array;
	@HideInInspector	public var OwnClientData:ClientCameraInfo;
	@HideInInspector	public var ServerIp:String;
	@HideInInspector	public var IsServer:boolean = false;
	@HideInInspector	public var ConnectedClientNumber:int = 0;
	@HideInInspector	public var ScreenResolution:Vector2 = Vector2(1024, 768);
	@HideInInspector	public var ScreenRefreshrate:int = 60;
	@HideInInspector	public var ScreenReal:ClientCameraScreen;
	@HideInInspector	public var ScreenParallax:float = 0.06;
	@HideInInspector	public var QualitySetting:String = "Fastest";
	@HideInInspector	public var IsGameStarted:boolean = false;
	@HideInInspector	public var LogOnScreen:boolean = false;
	@HideInInspector	public var LogOnScreenClients:boolean = false;
	@HideInInspector	public var InputDevice:Device;
	
	public var InterfaceSkin:GUISkin;
	public var LogOnFile:boolean = false;
	public var IpDataFile:String = "IpData.conf";
	public var WaitInEditorForClientsOrServer:boolean = false;
	public var ConnectionRetries:int = 3;
	public var ServerPort:int = 25000;
	
	private var NetView:NetworkView;
	
	private static var LogString:String = "";
	private static var LogStringScrollPosition:Vector2 = Vector2(0, 0);
	private static var GuiString:String = "";
	private static var StaticLogOnFile:boolean = false;
	private static var StaticNetworkView:NetworkView;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Awake() {
		ClientData = new Array();
		
		NetView = networkView;
		
		StaticLogOnFile = LogOnFile;
		StaticNetworkView = NetView;
		
		WriteNewFile("");
	}
	
	
	function OnGUI() {
		GUI.skin = InterfaceSkin;
		
		if (IsServer) {
			if (!IsStandalone()){
				// upper right corner	- client number on server
				GUI.Label(Rect(Screen.width - 110, 10, 100, 20), "clients " + (ConnectedClientNumber - 1) + " / " + (ClientData.Count - 1), "Debug");
			
				var oldLogOnScreenClients:boolean = LogOnScreenClients;
				LogOnScreenClients = GUI.Toggle(Rect(Screen.width - 210, Screen.height - 70, 200, 20), LogOnScreenClients, " show debug log on clients");
				if (LogOnScreenClients != oldLogOnScreenClients)
					NetView.RPC("RPCSetLogOnScreenSERVERONLY", RPCMode.OthersBuffered, LogOnScreenClients);
			}	
			
			if (!IsStandalone() || (IsStandalone() && !Screen.lockCursor))
				LogOnScreen = GUI.Toggle(Rect(Screen.width - 210, Screen.height - 30, 200, 20), LogOnScreen, " show debug log");
		}
		
		if (LogOnScreen) {
			var style:GUIStyle = GUIStyle.none;
			// upper left corner	- debug log on screen
			LogStringScrollPosition = GUILayout.BeginScrollView(LogStringScrollPosition,
				style, style, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));
			GUILayout.Label(LogString, "Debug");
			GUILayout.EndScrollView();
		}
		
		if (OwnClientData.ShowGuiText && GuiString.Length != 0)
			// centered 			- gui text on clients
			var spacing:Vector2 = Vector2(15, 5);
			var size:Vector2 = GUI.skin.GetStyle("GuiCenter").CalcSize(GUIContent(GuiString));
			GUI.Label(Rect((Screen.width / 2.0) - (size.x / 2.0) - spacing.x,
						   (Screen.height / 2.0) - (size.y / 2.0) - spacing.y,
						   size.x + 2.0 * spacing.x, size.y + 2.0 * spacing.y), GuiString, "GuiCenter");
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS STATIC --------------------------------------------------------------
	
	// show as nice text and call rpc to show as nice text on all clients also
	//TODO dont know if it can be used by clients
	public static function SetGuiOnAllSERVERONLY(text:String) {
		StaticNetworkView.RPC("RPCSetGuiOnAllSERVERONLY", RPCMode.AllBuffered, text);
	}
	
	
	// log and call rpc to log on all clients also
	//TODO not tested yet
	//TODO dont know if it can be used by clients
	public static function LogWithAllSERVERONLY(text:String) {
		StaticNetworkView.RPC("RPCLogWithAllSERVERONLY", RPCMode.AllBuffered, text);
	}
	
	
	// log and show as nice text in center of screen
	public static function LogWithGui(text:String) {
		Log(text, false);
		GuiString = text;
	}
	
	
	// log as no error
	public static function Log(text:String) {
		Log(text, false);
	}
	
	
	// static log function for console, debug-gui and file
	// can be used instead of unity Debug.Log()
	public static function Log(text:String, isError:boolean) {
		var prefix:String = "";
		if (isError) { // log in unity console
			Debug.LogError(text);
			prefix = "[ERR]";
		} else {
			Debug.Log(text);
			prefix = "     ";
		}
		
		text = FormatText(prefix, text, System.Environment.NewLine);
		
		LogString += text + System.Environment.NewLine; // log in debug-gui-interface
		LogStringScrollPosition.y = Mathf.Infinity; // auto scroll text
		
		AppendToFile(text); // log in file
	}
	
	
	private static function FormatText(prefix:String, text:String, separator:String):String {
		text = prefix + text;
		text = text.Replace(separator, separator + prefix);
		return text;
	}
	
	
	private static function WriteNewFile(text:String) {
		if (!StaticLogOnFile)
			return;
		
		var sw:System.IO.StreamWriter;
		try {
			sw = System.IO.StreamWriter(System.Net.Dns.GetHostName() + ".txt", false);
	    	sw.WriteLine(text);
	    } catch (e)
	    	ConfigClass.Log(e.ToString(), true);
	    finally
	    	sw.Close();
	}
	
	
	private static function AppendToFile(text:String) {
		if (!StaticLogOnFile)
			return;
		
		var sw:System.IO.StreamWriter;
		try {
			sw = System.IO.StreamWriter(System.Net.Dns.GetHostName() + ".txt", true);
		    sw.WriteLine(text);
	    } catch (e)
	    	ConfigClass.Log(e.ToString(), true);
	    finally
	    	sw.Close();
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS  --------------------------------------------------------------------
	
	public function OwnClientDataToString() {
		Log(OwnClientData.ToString());
		Log("screen: " + ScreenReal.ToString());
		Log("parallax: " + ScreenParallax);
		Log("device: " + InputDevice);
		Log("isServer: " + IsServer);
	}
	
	
	public function IsStandalone():boolean {
		return (ClientData.Count <= 1);
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS RPC -----------------------------------------------------------------
	
	@RPC
	function RPCSetLogOnScreenSERVERONLY(showLog:boolean) {
		LogOnScreen = showLog;
	}
	
	
	@RPC
	function RPCSetGuiOnAllSERVERONLY(text:String) {
		GuiString = text;
	}
	
	
	@RPC
	function RPCLogWithAllSERVERONLY(text:String) {
		Log(text, false);
	}
	// =============================================================================
}