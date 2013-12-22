
// Hannes Helmholz
//
// help functions for GameController
// functions should be available before GameController starts

using UnityEngine;
using System.Net;
using System.Collections;
using System;
using System.IO;

public class ApplicationStarter : MonoBehaviour
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    private enum State
    {
        FindResolution = 0,
        FindScreen = 1,
        FindQuality = 2,
        FindInput = 3,
        FindServer = 4,
        FindClients = 5
    }
 
    private ConfigClass Config;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Awake ()
    {
        Config = GameObject.FindWithTag ( "Config" ).GetComponent<ConfigClass> ();
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS STATIC --------------------------------------------------------------
 
    private static string GetOwnIp ()
    {
        string strHostName = System.Net.Dns.GetHostName ();
        IPAddress[] ips = System.Net.Dns.GetHostAddresses ( strHostName );
        return ips[0].ToString (); // just get first ip
    }
 
 
    private static ConfigClass.Device ParseInputDevice ( string text )
    {
        ConfigClass.Device device = ConfigClass.Device.None;
     
        if ( text.Equals ( "Keyboard" ) )
        {
            device = ConfigClass.Device.Keyboard;
        }
        else if ( text.Equals ( "Xbox" ) )
            {
                device = ConfigClass.Device.Xbox;
            }
            else if ( text.Equals ( "Spacepilot" ) )
                {
                    device = ConfigClass.Device.Spacepilot;
                }
     
        return device;
    }
 
 
    // must be called on all clients to avoid collision detection (should only be done on server)
    public static void DestroyAllPhysics ()
    {

        Rigidbody[] allRigids = FindObjectsOfType( typeof(Rigidbody) ) as Rigidbody[];


        ConfigClass.Log ( "destroy " + allRigids.Length + " rigidbodies" );
        foreach ( Rigidbody body in allRigids )
        {
            GameObject.Destroy ( body );
        }

        Collider[] allCollider = FindObjectsOfType ( typeof(Collider) ) as Collider[];
        ConfigClass.Log ( "disable " + allCollider.Length + " collider" );
        foreach ( Collider col in allCollider )
        {
            col.enabled = false;
        }
    }
 
 
    // sound only on server
    public static void DestroyAllAudio ()
    {
        AudioSource[] allSources = FindObjectsOfType ( typeof(AudioSource) ) as AudioSource[];
        ConfigClass.Log ( "destroy " + allSources.Length + " audio sources" );
        foreach ( AudioSource source in allSources )
        {
            GameObject.Destroy ( source );
        }
         
        AudioListener[] allListener = FindObjectsOfType ( typeof(AudioListener) ) as AudioListener[];
        ConfigClass.Log ( "destroy " + allListener.Length + " audio listener" );
        foreach ( AudioListener listener in allListener )
        {
            GameObject.Destroy ( listener );
        }
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS  --------------------------------------------------------------------
 
    // set unity quality level setting by config file
    // not set if started as standalone
    // if setting not supported set lowest quality level
    public void SetQualityLevel ()
    {
        if ( Application.isEditor )
        {
            ConfigClass.Log ( "keep quality level because of editor" );
            ConfigClass.Log ( "current quality level " + QualitySettings.names[QualitySettings.GetQualityLevel ()] );
            return;
        }

        if ( Config.IsStandalone () )
        {
            ConfigClass.Log ( "keep quality level because of standalone" );
            ConfigClass.Log ( "current quality level " + QualitySettings.names[QualitySettings.GetQualityLevel ()] );
            ConfigClass.Log ( "start application with SHIFT-Key hold down to change quality level" );
            return;
        }

        ConfigClass.Log ( "set quality level " + Config.QualitySetting );
 
        int number = -1;
        for ( int i = 0; i < QualitySettings.names.Length; i++ )
        {
            if ( QualitySettings.names[i].Equals ( Config.QualitySetting ) )
            {
                number = i;
            }
        }
             
        if ( number >= 0 )
        {
            QualitySettings.SetQualityLevel ( number );
        }
        else
        {
            ConfigClass.Log ( "quality level not supported", true );
            ConfigClass.Log ( "set quality level " + QualitySettings.names[0] );
            QualitySettings.SetQualityLevel ( 0 );
        }
    }
 
 
    // set screnn resolution by config file
    // not set if started as standalone
    // if resolution not supported set 1024x768@60
    public IEnumerator SetScreenResolution ()
    {
        Resolution res = Screen.currentResolution;
     
        if ( Application.isEditor )
        {
            ConfigClass.Log ( "keep resolution because of editor" );
        }
        else if ( Config.IsStandalone () )
            {
                ConfigClass.Log ( "keep resolution because of standalone" );
                ConfigClass.Log ( "start application with SHIFT-Key hold down to change resolution" );
            }
     
        if ( Application.isEditor || Config.IsStandalone () )
        {
            ConfigClass.Log ( "current resolution " + res.width + " x " + res.height + " @ " + res.refreshRate );

            Config.ScreenResolution.x = res.width;
            Config.ScreenResolution.y = res.height;
            Config.ScreenRefreshrate = res.refreshRate;
         
            return true;
        }
 
        ConfigClass.Log ( "set resolution " + Config.ScreenResolution.x + " x " + Config.ScreenResolution.y + " @ " + Config.ScreenRefreshrate );
        Screen.SetResolution ( Config.ScreenResolution.x, Config.ScreenResolution.y, Screen.fullScreen, Config.ScreenRefreshrate );
     
        //dan 
        //TODO: TEST!
        yield return new WaitForSeconds(0.5f);
     
        res = Screen.currentResolution;
        if ( Config.ScreenResolution.x != res.width || Config.ScreenResolution.y != res.height || Config.ScreenRefreshrate != res.refreshRate )
        {
            ConfigClass.Log ( "resolution not supported", true );

            Config.ScreenResolution.x = 1024;
            Config.ScreenResolution.y = 768;
            Config.ScreenRefreshrate = 60;
         
            ConfigClass.Log ( "set resolution " + Config.ScreenResolution.x + " x " + Config.ScreenResolution.y + " @ " + Config.ScreenRefreshrate );
            Screen.SetResolution ( Config.ScreenResolution.x, Config.ScreenResolution.y, Screen.fullScreen, Config.ScreenRefreshrate );
            //dan 
            //TODO: TEST!
            yield return new WaitForSeconds(0.5f);
        }
    }
 
 
    // read config file and setup all necessary data in ConfigClass
    public void GenerateIpData ()
    {
        bool foundOwnData = false;
        try
           {
            foundOwnData = LoadConfigFile ();
        }
        catch ( Exception e )
        {
            Console.WriteLine (e.StackTrace);
            ConfigClass.Log( "error loading config file, force standalone mode" );
        }   
        CheckIfServer ( foundOwnData );
        CheckIfStandalone ( foundOwnData );
    }
 
 
    // read config file
    // iterating through Status so change order in Status-enum to change order in config file
    private bool LoadConfigFile ()
    {
        bool found = false;
        StreamReader sr;

        sr = new System.IO.StreamReader ( Config.IpDataFile );

        try
        {
            State state = State.FindResolution;
            string ownIp = GetOwnIp ();
         
            string line = sr.ReadLine ();
            while ( line != null )
            {
                line = line.Split ( "|"[0] )[0]; // without comments
             
                if ( line.Length <= 0 )
                {
                    line = sr.ReadLine ();
                    continue;
                }
             
                String[] parts = line.Split ( ';' );
             
                for ( int i = 0; i < parts.Length; i++ )
                {
                    parts[i] = parts[i].Replace ( " ", "" );
                }
             
                if ( state == State.FindResolution )
                {
                    if ( parts.Length != 3 )
                    {
                        line = sr.ReadLine ();
                        continue;
                    }

                    state += 1;
                    Config.ScreenResolution.x = int.Parse ( parts[0] );
                    Config.ScreenResolution.y = int.Parse ( parts[1] );
                    Config.ScreenRefreshrate = int.Parse ( parts[2] );
             
                }
                else if ( state == State.FindScreen )
                {
                    if ( parts.Length != 4 )
                    {
                        line = sr.ReadLine ();
                        continue;
                    }
    
                    state += 1;
                    float width = float.Parse ( parts[0] );
                    float height = float.Parse ( parts[1] );
                    float distance = float.Parse ( parts[2] );
                    Config.ScreenReal = new ClientCameraScreen ( width, height, distance );
                    Config.ScreenParallax = float.Parse ( parts[3] );
    
                }
                else if ( state == State.FindQuality )
                {
                    if ( parts.Length != 1 )
                    {
                        line = sr.ReadLine ();
                        continue;
                    }
             
                    state += 1;
                    Config.QualitySetting = parts[0];
     
                }
                else if ( state == State.FindInput )
                {
                    if ( parts.Length != 1 )
                    {
                        line = sr.ReadLine ();
                        continue;
                    }

                    state += 1;
                    Config.InputDevice = ParseInputDevice ( parts[0] );

                }
                else if ( state == State.FindServer || state == State.FindClients )
                {
                    if ( parts.Length != 6 )
                    {
                        line = sr.ReadLine ();
                        continue;
                    }

                    string ip = parts[0];
                    bool showMainCamera = ( int.Parse( parts[1] ) == 1 ) ? true : false;
                    bool showGuiText = ( int.Parse( parts[2] ) == 1 ) ? true : false;
                    bool showGuiCamera = ( int.Parse( parts[3]) == 1) ? true : false;
                    float angleOffset = float.Parse( parts[4] );
                    float parallaxOffsetDirection = float.Parse ( parts[5] );
 
                    ClientCameraInfo info = new ClientCameraInfo ( ip, showMainCamera, showGuiText, showGuiCamera, angleOffset, parallaxOffsetDirection );
                    Config.ClientData.Add ( info );
                    if ( ownIp.Equals ( info.Ip ) )
                    {
                        found = true;
                        Config.OwnClientData = info;
                    }
 
                    if ( state == State.FindServer )
                    {
                        state += 1;
                        Config.ServerIp = ip;
                    }
                }
             
                line = sr.ReadLine ();
            }
        }
        catch ( Exception e )
        {
            ConfigClass.Log ( e.ToString (), true );
        }
        finally
        {
            sr.Close ();
        }
     
        return found;
    }
 
 
    private void CheckIfServer ( bool foundOwnData )
    {
        if ( !foundOwnData )
        {
            return;
        }
         
        Config.IsServer = Config.OwnClientData.Ip.Equals ( Config.ServerIp );
        if ( Config.IsServer )
        {
            Config.LogOnScreen = true;
        }  
    }
 
 
    private void CheckIfStandalone ( bool foundOwnData )
    {
        if ( foundOwnData || ( Application.isEditor && Config.WaitInEditorForClientsOrServer ) )
        {
            return;
        }
 
        if ( !Application.isEditor )
        {
            ConfigClass.Log ( "this ip not supported", true );
        }
        else
        {
            ConfigClass.Log ( "detected editor standalone mode" );
        }
     
        ForceStandaloneServer ();
    }
 
 
    public void ForceStandaloneServer ()
    {
        ConfigClass.Log ( "force standalone mode" );
        ConfigClass.Log ( "====================" );
     
        string ip = GetOwnIp ();
        Config.IsServer = true;
        Config.ServerIp = ip;
        Config.ConnectionRetries = 0;
        Config.OwnClientData = new ClientCameraInfo ( ip, true, true, true, 0, 0 );

        Config.ClientData.Clear();

//        while ( Config.ClientData > 0 )
//        {
//            Config.ClientData.Pop ();
//        }

        Config.ClientData.Add ( Config.OwnClientData );
    }
    // =============================================================================
}