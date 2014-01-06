#pragma strict

// Hannes Helmholz
//
// help functions for GameController
// functions should be available before GameController starts


private class ApplicationStarter extends MonoBehaviour {
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	private enum State {FindResolution = 0, FindScreen = 1, FindQuality = 2, FindInput = 3, FindServer = 4, FindClients = 5}
	
	private var Config:ConfigClass;
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	function Awake() {
		Config = GameObject.FindWithTag("Config").GetComponent.<ConfigClass>();
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS STATIC --------------------------------------------------------------
	
	private static function GetOwnIp():String {
		var strHostName:String = System.Net.Dns.GetHostName();
		var ips:System.Net.IPAddress[] = System.Net.Dns.GetHostAddresses(strHostName);
		return ips[0].ToString(); // just get first ip
	}
	
	
	private static function ParseInputDevice(text:String):ConfigClass.Device {
		var device:ConfigClass.Device = ConfigClass.Device.None;
		
		if (text.Equals("Keyboard"))
			device = ConfigClass.Device.Keyboard;
		else if (text.Equals("Xbox"))
			device = ConfigClass.Device.Xbox;
		else if (text.Equals("Spacepilot"))
			device = ConfigClass.Device.Spacepilot;
		
		return device;
	}
	
	
	// must be called on all clients to avoid collision detection (should only be done on server)
	public static function DestroyAllPhysics() {
		var allRigids:Rigidbody[] = GameObject.FindObjectsOfType(Rigidbody);
		ConfigClass.Log("destroy " + allRigids.length + " rigidbodies");
		for (var body:Rigidbody in allRigids)
			GameObject.Destroy(body);
	        
	    var allCollider:Collider[] = GameObject.FindObjectsOfType(Collider);
	    ConfigClass.Log("disable " + allCollider.length + " collider");
	    for (var col:Collider in allCollider)
	    	col.enabled = false;
	}
	
	
	// sound only on server
	public static function DestroyAllAudio() {
		var allSources:AudioSource[] = GameObject.FindObjectsOfType(AudioSource);
		ConfigClass.Log("destroy " + allSources.length + " audio sources");
		for (var source:AudioSource in allSources)
			GameObject.Destroy(source);
			
		var allListener:AudioListener[] = GameObject.FindObjectsOfType(AudioListener);
		ConfigClass.Log("destroy " + allListener.length + " audio listener");
		for (var listener:AudioListener in allListener)
			GameObject.Destroy(listener);
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS  --------------------------------------------------------------------
	
	// set unity quality level setting by config file
	// not set if started as standalone
	// if setting not supported set lowest quality level
	public function SetQualityLevel() {
		if (Application.isEditor) {
			Config.Log("keep quality level because of editor");
			Config.Log("current quality level " + QualitySettings.names[QualitySettings.GetQualityLevel()]);
			return;
		}
	
		if (Config.IsStandalone()) {
			Config.Log("keep quality level because of standalone");
			Config.Log("current quality level " + QualitySettings.names[QualitySettings.GetQualityLevel()]);
			Config.Log("start application with SHIFT-Key hold down to change quality level");
			return;
		}
	
		Config.Log("set quality level " + Config.QualitySetting);
	
		var number:int = -1;
		for (var i:int = 0; i < QualitySettings.names.length; i++)
	        if (QualitySettings.names[i].Equals(Config.QualitySetting))
	        	number = i;
	        	
	    if (number >= 0)
			QualitySettings.SetQualityLevel(number);
		else {
			Config.Log("quality level not supported", true);
			Config.Log("set quality level " + QualitySettings.names[0]);
			QualitySettings.SetQualityLevel(0);
		}
	}
	
	
	// set screnn resolution by config file
	// not set if started as standalone
	// if resolution not supported set 1024x768@60
	public function SetScreenResolution() {
		var res:Resolution = Screen.currentResolution;
		
		if (Application.isEditor)
			Config.Log("keep resolution because of editor");
	
		else if (Config.IsStandalone()) {
			Config.Log("keep resolution because of standalone");
			Config.Log("start application with SHIFT-Key hold down to change resolution");
		}
		
		if (Application.isEditor || Config.IsStandalone()) {
			Config.Log("current resolution " + res.width + " x " + res.height + " @ " + res.refreshRate);
			
			Config.ScreenResolution.x = res.width;
			Config.ScreenResolution.y = res.height;
			Config.ScreenRefreshrate = res.refreshRate;
			
			return;
		}
	
		Config.Log("set resolution " + Config.ScreenResolution.x + " x " + Config.ScreenResolution.y + " @ " + Config.ScreenRefreshrate);
		Screen.SetResolution(Config.ScreenResolution.x, Config.ScreenResolution.y, Screen.fullScreen, Config.ScreenRefreshrate);
		yield WaitForSeconds(0.5);
		
		res = Screen.currentResolution;
		if (Config.ScreenResolution.x != res.width || Config.ScreenResolution.y != res.height || Config.ScreenRefreshrate != res.refreshRate) {
			Config.Log("resolution not supported", true);
			
			Config.ScreenResolution.x = 1024;
			Config.ScreenResolution.y = 768;
			Config.ScreenRefreshrate = 60;
			
			Config.Log("set resolution " + Config.ScreenResolution.x + " x " + Config.ScreenResolution.y + " @ " + Config.ScreenRefreshrate);
			Screen.SetResolution(Config.ScreenResolution.x, Config.ScreenResolution.y, Screen.fullScreen, Config.ScreenRefreshrate);
			yield WaitForSeconds(0.5);
		}
	}
	
	
	// read config file and setup all necessary data in ConfigClass
	public function GenerateIpData() {
		var foundOwnData:boolean = false;
		try
			foundOwnData = LoadConfigFile();
		catch (e)
			Config.Log("error loading config file, force standalone mode");
			
		CheckIfServer(foundOwnData);
		CheckIfStandalone(foundOwnData);
	}
	
	
	// read config file
	// iterating through Status so change order in Status-enum to change order in config file
	private function LoadConfigFile():boolean {
		var found:boolean = false;
		var sr:System.IO.StreamReader;
		try {
	        sr = new System.IO.StreamReader(Config.IpDataFile);
		
			var state:State = State.FindResolution;
			var ownIp:String = GetOwnIp();
			
		    var line:String = sr.ReadLine();
			while (line != null) {
				line = line.Split("|"[0])[0]; // without comments
				
				if (line.Length <= 0) {
					line = sr.ReadLine(); continue; }
				
				var parts:String[] = line.Split(";"[0]);
				for (var part:String in parts)
					part = part.Replace(' ', ''); // delete all whitespaces
				
				if (state == State.FindResolution) {
					if (parts.Length != 3) {
						line = sr.ReadLine(); continue; }
						
					state += 1;
					Config.ScreenResolution.x = parseInt(parts[0]);
					Config.ScreenResolution.y = parseInt(parts[1]);
					Config.ScreenRefreshrate = parseInt(parts[2]);
				
				} else if (state == State.FindScreen) {
					if (parts.Length != 4) {
						line = sr.ReadLine(); continue; }
						
					state += 1;
					var width:float = parseFloat(parts[0]);
					var height:float = parseFloat(parts[1]);
					var distance:float = parseFloat(parts[2]);
					Config.ScreenReal = ClientCameraScreen(width, height, distance);
					Config.ScreenParallax = parseFloat(parts[3]);
						
				} else if (state == State.FindQuality) {
					if (parts.Length != 1) {
						line = sr.ReadLine(); continue; }
						
					state += 1;
					Config.QualitySetting = parts[0];
				
				} else if (state == State.FindInput) {
					if (parts.Length != 1) {
						line = sr.ReadLine(); continue; }
						
					state += 1;
					Config.InputDevice = ParseInputDevice(parts[0]);
				
				} else if (state == State.FindServer || state == State.FindClients) {
		        	if (parts.Length != 6) {
						line = sr.ReadLine(); continue; }
		        	
		        	var ip:String = parts[0];
		        	var showMainCamera:boolean = (parseInt(parts[1])) ? true : false;
		        	var showGuiText:boolean = (parseInt(parts[2])) ? true : false;
		        	var showGuiCamera:boolean = (parseInt(parts[3])) ? true : false;
		        	var angleOffset:float = parseFloat(parts[4]);
		        	var parallaxOffsetDirection:float = parseFloat(parts[5]);
		        	
		        	var info:ClientCameraInfo = new ClientCameraInfo(ip, showMainCamera, showGuiText, showGuiCamera, angleOffset, parallaxOffsetDirection);
		        	Config.ClientData.Add(info);
		        	if (ownIp.Equals(info.Ip)) {
		        		found = true;
		       			Config.OwnClientData = info;
		       		}
		        	
		        	if (state == State.FindServer) {
		        		state += 1;
		        		Config.ServerIp = ip;
		        	}
		        }
		        
		    	line = sr.ReadLine();
		    }
	    } catch (e)
	    	Config.Log(e.ToString(), true);
	    finally
	    	sr.Close();
	    	
	    return found;
	}
	
	
	private function CheckIfServer(foundOwnData:boolean) {
		if (!foundOwnData)
			return;
			
		Config.IsServer = Config.OwnClientData.Ip.Equals(Config.ServerIp);
		if (Config.IsServer)
			Config.LogOnScreen = true;	
	}
	
	
	private function CheckIfStandalone(foundOwnData:boolean) {
		if (foundOwnData || (Application.isEditor && Config.WaitInEditorForClientsOrServer))
			return;
	
		if (!Application.isEditor)
			Config.Log("this ip not supported", true);
		else
			Config.Log("detected editor standalone mode");
		
		ForceStandaloneServer();
	}
	
	
	public function ForceStandaloneServer() {
		Config.Log("force standalone mode");
		Config.Log("====================");
		
		var ip:String = GetOwnIp();
		Config.IsServer = true;
		Config.ServerIp = ip;
		Config.ConnectionRetries = 0;
		Config.OwnClientData = new ClientCameraInfo(ip, true, true, true, 0, 0);
		
		while (Config.ClientData.length > 0)
			Config.ClientData.Pop();
		Config.ClientData.Add(Config.OwnClientData);
	}
	// =============================================================================
}