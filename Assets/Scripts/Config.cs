






using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using System.Threading;
using Cave;
//using log4net;


[ExecuteInEditMode]
public class Config : MonoBehaviour
{
//	private static ILog logger = LogManager.GetLogger(typeof(Config));
	
	#region Einstellungen
	// EINSTELLUNGEN vom Inspector aus erreichbar
	
 	//dan
	[HideInInspector]    public List<System.Object> ClientData;
	[HideInInspector]    public ClientCameraInfo OwnClientData;
	public string IpDataFile;
	public bool WaitInEditorForClientsOrServer = false;
	
    [HideInInspector]    public Device InputDevice;
	public enum Device
    {
        None,
        Keyboard,
        Xbox,
        Spacepilot
    };
	
	//TODO input devices von der iss fixen bzw entfernen
    //[HideInInspector]    public Device InputDevice;
	
	//--
	
	public bool gameStarted = false;
	/**
	<summary>
	/// Wii im Standalone nutzen
	</summary>
	*/
	public bool useWiiInStandalone = false;
	/**
	<summary>
	/// Kinect im Standalone nutzen. Für den Remote Telepräsenz-Client am besten mit
	/// Kommandozeilenparameter useKinectInStandalone starten und das hier immer
	/// false lassen.
	</summary>
	*/
	public bool useKinectInStandalone = false;
	/**
	<summary>
	/// Die Kinect Server Adresse.
	</summary>
	*/
	public string caveKinectServerAddress = "cave00"; //"141.45.154.176";
	/**
	<summary>
	/// Die Wii Server Adresse.
	</summary>
	*/
	public string caveWiiServerAddress = "cave01"; //"141.45.154.177";
	/**
	<summary>
	/// Die Adresse des Spiel-Hosts
	</summary>
	*/
	public string caveGameServerAddress = "cave01"; // "141.45.154.177";	
	/**
	<summary>
	/// Der Maschinenname des Spiel-Hosts. Wird zur automatischen Erkennung des Modus genutzt
	</summary>
	*/
	public string caveGameServerMachineName = "cave01";
	/**
	<summary>
	/// Der Port des des Spiel-Hosts
	</summary>
	*/
	public int caveGameServerPort = 50000;
	/**
	<summary>
	/// Lässt den CAVE_SERVER beim Start mit dem TS-Server verbinden. Sollte
	/// immer false sein außer zu Hause zum Testen. In der CAVE kann das mit dem
	/// Kommandozeilenparameter useTeamSpeak aktiviert werden.
	</summary>
	*/
	public bool useTeamSpeak = false;
	/** 
	<summary>
		Die Teamspiel Server IP.
	</summary>
	*/
	public string teamSpeakServerIp = "141.45.154.168";
	/**
	<summary>
	/// Der Teamspiel Server Port.
	</summary>
	*/
	public int teamSpeakServerPort = 9987;
	/** 
	<summary>
		Computer mit Soundkarte in der CAVE, case insensitive
	</summary>
	*/
	public string soundMachineName = "cave09";
	/**
	<summary>
	/// Anzahl der remote Spieler, die sich maximal in die CAVE connecten können
	</summary>
	*/
	public int additionalPlayerCount = 1;
	/**
	<summary>
	/// Basispfad für die Gerätekoordinaten. [CAVENODE] wird eingesetzt.
	</summary>
	*/
	public string matrixPathTemplate = "../../NodeData/geraetekoordinaten{0:d}.txt";
	/**
	<summary>
	/// Pfad zur Datei, in der die IPs der Nodes abgelegt sind
	</summary>
	*/
	public string ipConfigFile = "ipconfig.txt";
	/**
	<summary>
	/// Button auf der Wii, mit dem geschossen wird
	</summary>
	*/
	public WiiMote.ButtonId wiiButtonShoot = WiiMote.ButtonId.B;
	/**
	<summary>
	/// Button auf der Wii, mit dem das Licht umgeschaltet wird
	</summary>
	*/
	public WiiMote.ButtonId wiiButtonLight = WiiMote.ButtonId.A;
	/**
	<summary>
	/// Button auf der Wii, mit dem die Szene resettet wird
	</summary>
	*/
	public WiiMote.ButtonId wiiButtonReset = WiiMote.ButtonId.ONE;
	/**
	<summary>
	/// Taste, mit der das Licht umgeschaltet wird
	</summary>
	*/
	public KeyCode keyboardButtonLight = KeyCode.L;
	/**
	<summary>
	/// Taste, mit der die Szene resettet wird
	</summary>
	*/
	public KeyCode keyboardButtonReset = KeyCode.R;
	/**
	<summary>
	/// Taste, mit der nach vorn gegangen wird
	</summary>
	*/
	public KeyCode keyboardButtonForward = KeyCode.W;
	/**
	<summary>
	/// Taste, mit der nach links gedreht wird
	</summary>
	*/
	public KeyCode keyboardButtonLeft = KeyCode.A;
	/**
	<summary>
	/// Taste, mit der zurück gegangen wird
	</summary>
	*/
	public KeyCode keyboardButtonBackward = KeyCode.S;
	/**
	<summary>
	/// Taste, mit der nach rechts gedreht wird
	</summary>
	*/
	public KeyCode keyboardButtonRight = KeyCode.D;
	#endregion

	#region Properties
	// EINSTELLUNGEN ENDE. PROPERTIES ANFANG

	/**
	<summary>
	/// STANDALONE: Einzelne Instanz. Kann einfach Test zu Hause sein, aber auch
	/// zum Verbinden als Remote Client für Telepräsenz
	///
	/// CAVE_RENDERER: Einer der acht Render-Knoten der CAVE
	///
	/// CAVE_SERVER: Der Spieleserver in der CAVE, von dem die Renderer Daten erhalten
	</summary>
	*/
	public enum AppMode
	{
		STANDALONE,
		CAVE_RENDERER,
		CAVE_SERVER
	}
	
	/**
	<summary>
	/// Siehe Doku zu AppMode
	</summary>
	*/
	public AppMode Mode { get; private set; }
	
	/**
	<summary>
	/// Die CAVE-Nodenummer. Kommt von der Umgebungsvariable CAVENODE und wird per Startskript gesetzt.
	</summary>
	<value>
	/// Die CAVE-Nodenummer.
	</value>
	*/
	public int CaveNode { get; private set; }
	
	/**
	<summary>
	/// Anwendung läuft nicht auf einem CAVE-Renderer. Wo sonst, sagt das nicht.
	</summary>
	*/
	public const int NOT_ON_CAVE_RENDERER = -1;
	
	/**
	<summary>
	/// Singleton-Instanz der Config.
	</summary>
	*/
	public static Config Instance { get; private set; }
	
	#endregion

	//public float bodyHeightLocalKinect = 1.8f;	// TODO: höhe am Client für abwurf der Bälle bei Kinect

	void Awake ()
	{
		Debug.Log ("config::Awake start");
		
        IsServer = false;
        gameStarted = false;
		IpDataFile = "IpData.conf";
		
		ClientData = new List<System.Object>();
        //NetView = networkView;
        //StaticNetworkView = networkView;
     
        //WriteNewFile ( "" );
		

		
		//6E---------------------------------------------------------------------------------------------------------
		
		Instance = this;
		this.ParseCommandLine ();
		this.GetNodeNumber ();
		this.DetermineMode ();

		if (this.Mode == AppMode.CAVE_RENDERER) {
			Screen.showCursor = false;
		}
		
		Debug.Log ("config::Awake end");
	}

	public bool IsStandalone
	{
		get
		{
//			logger.Debug ("CheckIfServerconfig::IsStandalone.get called");
			return this.Mode == AppMode.STANDALONE;
		}
	}
	
	//dan
	public bool IsServer
	{
		get
		{
//			logger.Debug ("CheckIfServerconfig::IsServer.get called");
			return (this.Mode == AppMode.CAVE_SERVER);
		}
		set
		{
			if(value)	this.Mode = AppMode.CAVE_SERVER;
			//AppMode.renderer not checked !
			else this.Mode = AppMode.STANDALONE;
		}
	}
	
	//dan
	public bool IsGameStarted
	{
		get
		{
//			logger.Debug ("CheckIfServerconfig::IsServer.get called");
			return this.gameStarted;
		}
		set
		{
			gameStarted = value;
		}
	}

	/**
	<summary>
	/// Gets a value indicating whether this <see cref="Config"/> use kinect.
	</summary>
	<value>
	/// <c>true</c> if use kinect; otherwise, <c>false</c>.
	</value>
	*/
	public bool UseKinect
	{
		get
		{
//			logger.Debug ("config::UseKinect.get called");
			return this.Mode == AppMode.CAVE_SERVER || this.Mode == AppMode.STANDALONE && this.useKinectInStandalone;
		}
	}

	public string KinectAddress
	{
		get
		{
			
			if (this.Mode == AppMode.STANDALONE)
			{
//				logger.Debug ("config::KinectAddress.get called: Mode = STANDALONE");
				return "Kinect@localhost";
			}
			else
			{
//				logger.Debug ("config::KinectAddress.get called: Mode != STANDALONE");
				return "Kinect@" + this.caveKinectServerAddress;
			}
		}
	}

	public bool UseWii
	{
		get
		{
			return this.Mode == AppMode.CAVE_SERVER || this.Mode == AppMode.STANDALONE && this.useWiiInStandalone;
		}
	}

	public string WiiAddress
	{
		get
		{
			if (this.Mode == AppMode.STANDALONE)
			{
				return "Wiimote1@localhost:1337";
			}
			else
			{
				return "Wiimote1@" + this.caveWiiServerAddress;
			}
		}
	}

	public string DeviceCoordinateFileName
	{
		get
		{
			if (this.CaveNode == NOT_ON_CAVE_RENDERER)
			{
				return "geraetekoordinaten_dummy.txt";
			}
			else
			{
				return string.Format(this.matrixPathTemplate, this.CaveNode);
			}
		}
	}

	private void ParseCommandLine()
	{
		string[] args = System.Environment.GetCommandLineArgs();
		for (int i = 0; i < args.Length; i++)
		{
			if (args[i].Equals("useTeamSpeak", StringComparison.OrdinalIgnoreCase))
			{
				Logger.Log("Kommandozeilenoption: CAVE-Server nutzt TeamSpeak");
				this.useTeamSpeak = true;
			}
			else if (args[i].Equals("useKinectInStandalone", StringComparison.OrdinalIgnoreCase))
			{
				this.useKinectInStandalone = true;
			}
		}
	}

	private void GetNodeNumber()
	{
		string nodeEnv = System.Environment.GetEnvironmentVariable("CAVENODE");
		this.CaveNode = string.IsNullOrEmpty(nodeEnv) ? NOT_ON_CAVE_RENDERER : int.Parse(nodeEnv);
	}

	private void DetermineMode()
	{
		if (this.caveGameServerMachineName.Equals(System.Environment.MachineName, StringComparison.OrdinalIgnoreCase))
		{
			Logger.Log("Auf " + this.caveGameServerMachineName + ". Starte als Server.");
			this.Mode = AppMode.CAVE_SERVER;
		}
		else if (this.CaveNode != NOT_ON_CAVE_RENDERER)
		{
			Logger.Log("CAVE-Umgebung erkannt. Starte als Node " + this.CaveNode + ".");
			this.Mode = AppMode.CAVE_RENDERER;
		}
		else
		{
			Logger.Log("Starte im Standalone-Modus" + (Application.isEditor ? " im Editor." : "."));
			this.Mode = AppMode.STANDALONE;
		}
	}
}

#region old
//using UnityEngine;
//
//using System;
//using System.IO;
//using System.Collections;
//
//using System.Threading;
//
//using Cave;
////using log4net;
//
//
//
//[ExecuteInEditMode]
//public class Config : MonoBehaviour
//{
////	private static ILog logger = LogManager.GetLogger(typeof(Config));
//	
//	#region Einstellungen
//	// EINSTELLUNGEN vom Inspector aus erreichbar
//	
// 	//dan
//	public bool gameStarted = false;
//	//--
//	
//	/**
//	<summary>
//	/// Wii im Standalone nutzen
//	</summary>
//	*/
//	public bool useWiiInStandalone = false;
//	/**
//	<summary>
//	/// Kinect im Standalone nutzen. Für den Remote Telepräsenz-Client am besten mit
//	/// Kommandozeilenparameter useKinectInStandalone starten und das hier immer
//	/// false lassen.
//	</summary>
//	*/
//	public bool useKinectInStandalone = false;
//	
//	/**
//	<summary>
//	/// Die Kinect Server Adresse.
//	</summary>
//	*/
//	public string caveKinectServerAddress = "cave00"; //"141.45.154.176";
//	/**
//	<summary>
//	/// Die Wii Server Adresse.
//	</summary>
//	*/
//	public string caveWiiServerAddress = "cave01"; //"141.45.154.177";
//	/**
//	<summary>
//	/// Die Adresse des Spiel-Hosts
//	</summary>
//	*/
//	public string caveGameServerAddress = "cave01"; // "141.45.154.177";
//	
//	/**
//	<summary>
//	/// Der Maschinenname des Spiel-Hosts. Wird zur automatischen Erkennung des Modus genutzt
//	</summary>
//	*/
//	public string caveGameServerMachineName = "cave01";
//	/**
//	<summary>
//	/// Der Port des des Spiel-Hosts
//	</summary>
//	*/
//	public int caveGameServerPort = 50000;
//	
//	/**
//	<summary>
//	/// Lässt den CAVE_SERVER beim Start mit dem TS-Server verbinden. Sollte
//	/// immer false sein außer zu Hause zum Testen. In der CAVE kann das mit dem
//	/// Kommandozeilenparameter useTeamSpeak aktiviert werden.
//	</summary>
//	*/
//	public bool useTeamSpeak = false;
//	/**
//	<summary>
//	/// Die Teamspiel Server IP.
//	</summary>
//	*/
//	public string teamSpeakServerIp = "141.45.154.168";
//	/**
//	<summary>
//	/// Der Teamspiel Server Port.
//	</summary>
//	*/
//	public int teamSpeakServerPort = 9987;
//	
//	/**
//	<summary>
//	/// Computer mit Soundkarte in der CAVE, case insensitive
//	</summary>
//	*/
//	public string soundMachineName = "cave09";
//	
//	/**
//	<summary>
//	/// Anzahl der remote Spieler, die sich maximal in die CAVE connecten können
//	</summary>
//	*/
//	public int additionalPlayerCount = 1;
//	
//	/**
//	<summary>
//	/// Basispfad für die Gerätekoordinaten. [CAVENODE] wird eingesetzt.
//	</summary>
//	*/
//	public string matrixPathTemplate = "../../NodeData/geraetekoordinaten{0:d}.txt";
//	
//	/**
//	<summary>
//	/// Pfad zur Datei, in der die IPs der Nodes abgelegt sind
//	</summary>
//	*/
//	public string ipConfigFile = "ipconfig.txt";
//	
//	/**
//	<summary>
//	/// Button auf der Wii, mit dem geschossen wird
//	</summary>
//	*/
//	public WiiMote.ButtonId wiiButtonShoot = WiiMote.ButtonId.B;
//	/**
//	<summary>
//	/// Button auf der Wii, mit dem das Licht umgeschaltet wird
//	</summary>
//	*/
//	public WiiMote.ButtonId wiiButtonLight = WiiMote.ButtonId.A;
//	/**
//	<summary>
//	/// Button auf der Wii, mit dem die Szene resettet wird
//	</summary>
//	*/
//	public WiiMote.ButtonId wiiButtonReset = WiiMote.ButtonId.ONE;
//	
//	/**
//	<summary>
//	/// Taste, mit der das Licht umgeschaltet wird
//	</summary>
//	*/
//	public KeyCode keyboardButtonLight = KeyCode.L;
//	/**
//	<summary>
//	/// Taste, mit der die Szene resettet wird
//	</summary>
//	*/
//	public KeyCode keyboardButtonReset = KeyCode.R;
//	/**
//	<summary>
//	/// Taste, mit der nach vorn gegangen wird
//	</summary>
//	*/
//	public KeyCode keyboardButtonForward = KeyCode.W;
//	/**
//	<summary>
//	/// Taste, mit der nach links gedreht wird
//	</summary>
//	*/
//	public KeyCode keyboardButtonLeft = KeyCode.A;
//	/**
//	<summary>
//	/// Taste, mit der zurück gegangen wird
//	</summary>
//	*/
//	public KeyCode keyboardButtonBackward = KeyCode.S;
//	/**
//	<summary>
//	/// Taste, mit der nach rechts gedreht wird
//	</summary>
//	*/
//	public KeyCode keyboardButtonRight = KeyCode.D;
//	#endregion
//
//	#region Properties
//	// EINSTELLUNGEN ENDE. PROPERTIES ANFANG
//
//	/**
//	<summary>
//	/// STANDALONE: Einzelne Instanz. Kann einfach Test zu Hause sein, aber auch
//	/// zum Verbinden als Remote Client für Telepräsenz
//	///
//	/// CAVE_RENDERER: Einer der acht Render-Knoten der CAVE
//	///
//	/// CAVE_SERVER: Der Spieleserver in der CAVE, von dem die Renderer Daten erhalten
//	</summary>
//	*/
//	public enum AppMode
//	{
//		STANDALONE,
//		CAVE_RENDERER,
//		CAVE_SERVER
//	}
//	/**
//	<summary>
//	/// Siehe Doku zu AppMode
//	</summary>
//	*/
//	public AppMode Mode { get; private set; }
//	/**
//	<summary>
//	/// Die CAVE-Nodenummer. Kommt von der Umgebungsvariable CAVENODE und wird per Startskript gesetzt.
//	</summary>
//	*/
//	/// <value>
//	/// Die CAVE-Nodenummer.
//	/// </value>
//	public int CaveNode { get; private set; }
//	/**
//	<summary>
//	/// Anwendung läuft nicht auf einem CAVE-Renderer. Wo sonst, sagt das nicht.
//	</summary>
//	*/
//	public const int NOT_ON_CAVE_RENDERER = -1;
//	/**
//	<summary>
//	/// Singleton-Instanz der Config.
//	</summary>
//	*/
//	public static Config Instance { get; private set; }
//	#endregion
//
//	//public float bodyHeightLocalKinect = 1.8f;	// TODO: höhe am Client für abwurf der Bälle bei Kinect
//
//	void Awake ()
//	{
////		logger.Debug ("config::awake start");
//		
//		Instance = this;
//		this.ParseCommandLine ();
//		this.GetNodeNumber ();
//		this.DetermineMode ();
//
//		if (this.Mode == AppMode.CAVE_RENDERER) {
//			Screen.showCursor = false;
//		}
//		
////		logger.Debug ("config::awake end");
//	}
//
//	public bool IsStandalone
//	{
//		get
//		{
////			logger.Debug ("CheckIfServerconfig::IsStandalone.get called");
//			return this.Mode == AppMode.STANDALONE;
//		}
//	}
//	
//	//dan
//	public bool IsServer
//	{
//		get
//		{
////			logger.Debug ("CheckIfServerconfig::IsServer.get called");
//			return this.Mode == AppMode.CAVE_SERVER;
//		}
//	}
//	
//	//dan
//	public bool IsGameStarted
//	{
//		get
//		{
////			logger.Debug ("CheckIfServerconfig::IsServer.get called");
//			return this.gameStarted;
//		}
//		set
//		{
//			gameStarted = value;
//		}
//	}
//
//	/**
//	<summary>
//	/// Gets a value indicating whether this <see cref="Config"/> use kinect.
//	</summary>
//	*/
//	/// <value>
//	/// <c>true</c> if use kinect; otherwise, <c>false</c>.
//	/// </value>
//	public bool UseKinect
//	{
//		get
//		{
////			logger.Debug ("config::UseKinect.get called");
//			return this.Mode == AppMode.CAVE_SERVER || this.Mode == AppMode.STANDALONE && this.useKinectInStandalone;
//		}
//	}
//
//	public string KinectAddress
//	{
//		get
//		{
//			
//			if (this.Mode == AppMode.STANDALONE)
//			{
////				logger.Debug ("config::KinectAddress.get called: Mode = STANDALONE");
//				return "Kinect@localhost";
//			}
//			else
//			{
////				logger.Debug ("config::KinectAddress.get called: Mode != STANDALONE");
//				return "Kinect@" + this.caveKinectServerAddress;
//			}
//		}
//	}
//
//	public bool UseWii
//	{
//		get
//		{
//			return this.Mode == AppMode.CAVE_SERVER || this.Mode == AppMode.STANDALONE && this.useWiiInStandalone;
//		}
//	}
//
//	public string WiiAddress
//	{
//		get
//		{
//			if (this.Mode == AppMode.STANDALONE)
//			{
//				return "Wiimote1@localhost:1337";
//			}
//			else
//			{
//				return "Wiimote1@" + this.caveWiiServerAddress;
//			}
//		}
//	}
//
//	public string DeviceCoordinateFileName
//	{
//		get
//		{
//			if (this.CaveNode == NOT_ON_CAVE_RENDERER)
//			{
//				return "geraetekoordinaten_dummy.txt";
//			}
//			else
//			{
//				return string.Format(this.matrixPathTemplate, this.CaveNode);
//			}
//		}
//	}
//
//	private void ParseCommandLine()
//	{
//		string[] args = System.Environment.GetCommandLineArgs();
//		for (int i = 0; i < args.Length; i++)
//		{
//			if (args[i].Equals("useTeamSpeak", StringComparison.OrdinalIgnoreCase))
//			{
//				Logger.Log("Kommandozeilenoption: CAVE-Server nutzt TeamSpeak");
//				this.useTeamSpeak = true;
//			}
//			else if (args[i].Equals("useKinectInStandalone", StringComparison.OrdinalIgnoreCase))
//			{
//				this.useKinectInStandalone = true;
//			}
//		}
//	}
//
//	private void GetNodeNumber()
//	{
//		string nodeEnv = System.Environment.GetEnvironmentVariable("CAVENODE");
//		this.CaveNode = string.IsNullOrEmpty(nodeEnv) ? NOT_ON_CAVE_RENDERER : int.Parse(nodeEnv);
//	}
//
//	private void DetermineMode()
//	{
//		if (this.caveGameServerMachineName.Equals(System.Environment.MachineName, StringComparison.OrdinalIgnoreCase))
//		{
//			Logger.Log("Auf " + this.caveGameServerMachineName + ". Starte als Server.");
//			this.Mode = AppMode.CAVE_SERVER;
//		}
//		else if (this.CaveNode != NOT_ON_CAVE_RENDERER)
//		{
//			Logger.Log("CAVE-Umgebung erkannt. Starte als Node " + this.CaveNode + ".");
//			this.Mode = AppMode.CAVE_RENDERER;
//		}
//		else
//		{
//			Logger.Log("Starte im Standalone-Modus" + (Application.isEditor ? " im Editor." : "."));
//			this.Mode = AppMode.STANDALONE;
//		}
//	}
//}
#endregion
