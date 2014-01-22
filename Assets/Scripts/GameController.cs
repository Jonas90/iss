// Hannes Helmholz
//
// start script of application
// manage build all network connections
// after game started script only checks for application exit by server

using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[RequireComponent (typeof (NetworkView))] // for RPC

public class GameController : MonoBehaviour {
//--------HelmholzEnd-----------------------------------------------
	public static GameController Instance { get; private set; }

    private Hashtable __physicModels = new Hashtable();
    private Server __server = null;
    private Client __client = null;
    private Voice __voice = null;
 
    public Server Server { get { return __server; } }

 
//   private LogFile fileLog;
 
    public Client Client { get { return __client; } }
	public bool IsServer { get { return __server != null; } }
//--------HelmholzStart-----------------------------------------------
	
	
	// =============================================================================
	// MEMBERS ---------------------------------------------------------------------
	public enum GameState {WaitForClientsSERVERONLY, WaitForFirstStart, WaitForStart, Started}
	
	[SerializeField]	private ApplicationStarter ApplicationStarter;
	[SerializeField]	private GameObject Player;
	[SerializeField]	private GameToManage[] Games;
	
	private Config Config;
	//private NetworkView NetView;
	private PlayerMovement NavTrans;
//private PlayerRotation NavRot;
	
	private static GameState State;  
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY ---------------------------------------------------------------
	
	// start reading data from config file by GameStarter
	void Awake() {
		Instance = this; //Zeile aus 6. Etage
		//Config.Log("====================");
		//Config.Log("awake");
		
		Config = GameObject.FindWithTag("Config").GetComponent<Config>();
		//NetView = networkView;
		
		//Player = GameObject.FindWithTag("Player").GetComponent<Player>();
		Player = GameObject.FindWithTag("Player");//.GetComponent<GameObject>();
		
		//Config.Log("====================");
		//ApplicationStarter.GenerateIpData();
		//Config.OwnClientDataToString();
	}
	
	
	// start application as server or client
	// setup screen and camera settings
	void Start() 
	{
//--------HelmholzEnde-----------------------------------------------
//       fileLog.Write ("i Game:Start start");
        // if we are using cluster-rendering we need to setup the newtork connections
        switch( Config.Instance.Mode )
        {
            case Config.AppMode.CAVE_SERVER:
                Logger.Log( "THIS IS Bond, SERVER Bond" );
                this.startServer();
                break;
            case Config.AppMode.CAVE_RENDERER:
                this.startClient( Config.Instance.caveGameServerAddress, Config.Instance.caveGameServerPort );
                break;
            case Config.AppMode.STANDALONE:
                startServer( false );
                break;
            default:
                Logger.LogError( "Unbekannter Mode bei Game.Start()" );
                break;
        }

        if( Config.Instance.useTeamSpeak )
        {
            this.startTeamspeak( "cave" );
        }

//--------HelmholzStart-----------------------------------------------
		//NavTrans = Player.GetComponent<PlayerMovement>();
		//NavRot = Player.GetComponent<PlayerRotation>();
		//NavTrans.enabled = false;
		//NavRot.enabled = false;
		
		//Config.Log("====================");
		//ApplicationStarter.SetQualityLevel();
		
		//Config.Log("====================");
		//StartCoroutine(ApplicationStarter.SetScreenResolution());
		
		StartApplication();
	}

//--------HelmholzEnde-----------------------------------------------
	void Update()
    {
        try
        {
            if( this.IsServer )
            {
                __server.Update();
            }
            if( Config.Instance.IsStandalone && NetworkPeerType.Client == Network.peerType )
            {
                //wenn Pysikalisch Client
                __client.Update();
            }
        }
        catch(Exception e)
        {
            Logger.LogException( e );
        }
    }
	
	void FixedUpdate()
    {
        try
        {
            if( this.IsServer )
            {
                __server.FixedUpdate();
            }
        }
        catch(Exception e)
        {
            Logger.LogException( e );
        }
    }
	
	void OnDestroy()
    {
        // Fragt intern ab, ob notwendig ist
        if( __voice != null )
        {
            __voice.stop();
        }
    }
	void OnDisconnectedFromServer( NetworkDisconnection info )
    {
        Logger.Log( "Diconnected from server: " + info );
        if( !this.IsServer )
        {
            startServer( false );
            this.restart();
        }
    }
	
    /// <summary>
    /// Starts the server. (immer, ausser wenn CaveClient)
    /// </summary>
    public void startServer( bool withSocket = true )
    {
        try
        {
            Logger.Log( "Game: Starte Server " + ( withSocket ? "mit Netzwerk." : "ohne Netzwerk." ) );
            if( withSocket )
            {
                CamManager cm = GameObject.Find( "CAVEStereoHeadNew" ).GetComponent<CamManager>();
                Network.InitializeServer( cm.camsPerGroup*cm.camGroups.Length + Config.Instance.additionalPlayerCount, Config.Instance.caveGameServerPort, false );
            }
            __client = null;
            __server = new Server( this, Config.Instance.IsStandalone );
        }
        catch(Exception e)
        {
            Logger.LogException( e );
        }
    }
	
    /// <summary>
    /// Starts the client. (alle Cave Clients und wenn auf button Verbinden gedrückt wird)
    /// </summary>
    public void startClient( string serverIp, int serverPort )
    {
        try
        {
            Logger.Log( "Game: Starte Client..." );
            if( Network.peerType != NetworkPeerType.Disconnected )
            {
                Logger.LogWarning( "Bestehende Verbindung wird beendet." );
            }
            Logger.Log( "Verbinde zu: " + serverIp + ":" + serverPort );
            Network.Connect( serverIp, serverPort );

            __server = null;
            __client = new Client( this );
        }
        catch(Exception e)
        {
            Logger.LogException( e );
        }
    }
    public void startTeamspeak( string userName )
    {
        if( __voice == null )
        {
            __voice = new Voice( Config.Instance.teamSpeakServerIp, Config.Instance.teamSpeakServerPort, Config.Instance.soundMachineName, userName );
        }

        Logger.Log( "Starte Teamspeak mit Benutzernamen " + userName );
        __voice.start();
    }

    public void AddToPhysicModels( PhysicModel model )
    {
        __physicModels.Add( model.name, model );
    }

    public void removeFromPhysics( PhysicModel model )
    {
        __physicModels.Remove( model.name );
    }

    /// <summary>
    /// Restart this instance.
    /// </summary>
    public void restart()
    {
        if( this.IsServer )
        {
            __server.reset();
        }
        else
        {
            __client.reset();
        }
    }

    public Hashtable PhysicModels
    {
        get { return __physicModels; }
    }
	
	
//--------HelmholzStart-----------------------------------------------
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS UNITY NETWORK -------------------------------------------------------
	
	void OnServerInitialized(){
		//if (!Config.IsStandalone())
			//Config.Log("waiting for clients");
		State = GameState.WaitForClientsSERVERONLY;
		
		//Config.ConnectedClientNumber++;
		CheckStartGame();
	}
	
	
	void OnConnectedToServer() {
		//Config.Log("waiting for start");
		State = GameState.WaitForFirstStart;
		
		//Config.Log("====================");
		ApplicationStarter.DestroyAllPhysics();
		//Config.Log("====================");
		ApplicationStarter.DestroyAllAudio();
		//Config.Log("====================");
	}
	
	
	// try reconnects or run as standalone
	void OnFailedToConnect(NetworkConnectionError error) {
		//Config.Log("ConnectionError: " + error, true);
		
		//Config.ConnectionRetries--;
		//if (Config.ConnectionRetries >= 0)
		//	Network.Connect(Config.caveGameServerAddress, Config.caveGameServerPort);
		//else {
			// run application as standalone, so as server without clients
		//	ApplicationStarter.ForceStandaloneServer();
		//	Config.OwnClientDataToString();
		//	StartApplication();
		//}
	}
	
	
	void OnPlayerConnected(NetworkPlayer player){
		//Config.Log("player connected: " + player.ToString());
	
		//Config.ConnectedClientNumber++;
		CheckStartGame();
	}
	
	
	void OnPlayerDisconnected(NetworkPlayer player){
		//Config.ConnectedClientNumber--;
	
		//Network.RemoveRPCs(player);
	  	//Network.DestroyPlayerObjects(player);
	}	
	// =============================================================================


	
	// =============================================================================
	// METHODS  --------------------------------------------------------------------
	
	private void StartApplication() {
		//Config.Log("====================");
		ClientCameraController.SetAllTransforms(Player);
		
		//Config.Log("====================");
		Screen.lockCursor = false;
		
		if (Config.IsServer)
			ServerISS();
		else
			ClientISS();
	}
	
	
	private void ServerISS() {
		//Config.Log("server started");
		//Config.Log("====================");
		
		Screen.showCursor = true;
		
		//Network.InitializeServer(Config.ClientData.Count, Config.caveGameServerPort, false);
	}
	
	
	private void ClientISS() {
		//Config.Log("waiting for server " + Config.caveGameServerAddress);
		//Config.Log("====================");
		
		Screen.showCursor = false;
		
		//Network.Connect(Config.caveGameServerAddress, Config.caveGameServerPort);
	}
	
	
	private void CheckStartGame() {
		//if (Config.ConnectedClientNumber == Config.ClientData.Count) {
		//	State = GameState.WaitForFirstStart;
		//}
	}
	
	
	public GameState GetState() {
		return State;
	}
	
	
	public void RestartGame() {
		StopGame();
		StartGame();
		
		foreach (GameToManage game in Games)
			game.Reset();
	}
	
	
	public void StartGame() {
		NavTrans.enabled = true;
		//NavRot.enabled = true;
			
		State = GameState.Started;
		foreach (GameToManage game in Games)
			game.Play();
			
		networkView.RPC("RPCGameStart", RPCMode.AllBuffered);
	}
	
	
	public void StopGame() {
		Input.ResetInputAxes();
		NavTrans.enabled = false;
		//NavRot.enabled = false;
	
		State = GameState.WaitForStart;
		foreach (GameToManage game in Games)
			game.Stop();
		
		networkView.RPC("RPCGameStop", RPCMode.AllBuffered);
	}
	
	
	public void EndGame() {
		networkView.RPC("RPCEndGame", RPCMode.AllBuffered);
	}
	// =============================================================================
	
	
	
	// =============================================================================
	// METHODS RPC -----------------------------------------------------------------
	
	[RPC]
	void RPCGameStart() {
		//Config.Log("game started");
		
		Config.gameStarted = true;
		State = GameState.Started;
	}
	
	
	[RPC]
	void RPCGameStop() {
		//Config.Log("game stoped");
		
		Config.gameStarted = false;
		State = GameState.WaitForStart;
	}
	
	[RPC]
	void RPCEndGame() {
		Application.Quit();
	}
	// =============================================================================
}