using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Game : MonoBehaviour
{

    public static Game Instance { get; private set; }

    private Hashtable __physicModels = new Hashtable();
    private Server __server = null;
    private Client __client = null;
    private Voice __voice = null;
 
    public Server Server { get { return __server; } }

 
//   private LogFile fileLog;
 
    public Client Client { get { return __client; } }

    void Awake()
    {
        Instance = this;
//       fileLog = new LogFile ("log.txt", true, false);
//       fileLog.Write("i Game:Awake");
    }

    /// <summary>
    /// Start this instance.
    /// 2. Einstiegspunkt 
    /// </summary>
    void Start()
    {
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
     
//       fileLog.Write ("Game:Start end");
    }

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

    public bool IsServer { get { return __server != null; } }

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
}
