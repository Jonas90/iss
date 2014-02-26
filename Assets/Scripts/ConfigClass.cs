////dan
//Alte Config-Klasse des Helmholz Projekts ist in die jetzige Config gemergt
//worden

//// Hannes Helmholz
////
//// config contains all client and server informations
//// afford a static log function in gui and file for debug use in cave
//// used in nearly every other script
//
//using UnityEngine;
//using System;
//using System.IO;
//
//using System.Collections.Generic;
//
//
//
//
//
//[RequireComponent (typeof(NetworkView))]// for RPC
//
//public class ConfigClass : MonoBehaviour
//{
//
//    // =============================================================================
//    #region MEMBERS
//
//	//kann wohl auch entfernt werden weils mit der kamera zu tun hat
//    [HideInInspector]    public List<System.Object> ClientData;
//	//muss mit weg, wir jedoch problematisch da es in: Crosshair.cs, EquipmentAsGui.cs, 
//	//EquipmentManager.cs, FramesPerSecond.cs, GameController.cs, GuiScreenPosition.cs
//	//Overview.cs und PlayerInteractionGame.cs vorhanden ist 
//    [HideInInspector]    public ClientCameraInfo OwnClientData;
//	//--
//	
//	//done--------------------------------------------------
//    [HideInInspector]    public string caveGameServerAddress;
//    [HideInInspector]    public bool IsServer;
//	//renamed
//    [HideInInspector]    public bool gameStarted;
//	public int caveGameServerPort;
//	//-------------------------------------------------------
//
//	
//	//muss entfernt werden--------------------------------------------------
//    [HideInInspector]    public int ConnectedClientNumber;
//	[HideInInspector]    public IntVector2 ScreenResolution;
//	[HideInInspector]    public int ScreenRefreshrate;
//	[HideInInspector]    public ClientCameraScreen ScreenReal;
//	[HideInInspector]    public float ScreenParallax;
//	
//	public GUISkin InterfaceSkin;
//	public string IpDataFile;
//	public bool WaitInEditorForClientsOrServer;
//	public int ConnectionRetries;
//	//----------------------------------------------------------------------
//	
//    //wird enfternt, kann später hinzugefügt werden
//    [HideInInspector]    public string QualitySetting;
//	
//	//log fliegt raus und muss noch mit log4net ersetzt werden
//    [HideInInspector]    public bool LogOnScreen;
//    [HideInInspector]    public bool LogOnScreenClients;
//	private static string LogString = "";
//	public bool LogOnFile;
//	private static Vector2 LogStringScrollPosition = new Vector2 ( 0, 0 );
//	//--
//	
//	
//	//TODO input devices von der iss fixen bzw entfernen
//	//InteractionKey.cs, PlayerInteractionGame.cs
//    [HideInInspector]    public Device InputDevice;
//	public enum Device
//    {
//        None,
//        Keyboard,
//        Xbox,
//        Spacepilot
//    };
//	//--
//    
//    //ignore
//    private NetworkView NetView;
//	  private static NetworkView StaticNetworkView;
//	
//    
//    private static string GuiString = "";
//    private static bool StaticLogOnFile = false;
//	
//    //---
//
//
//
//    #endregion
//    // =============================================================================
//
//    #region METHODS UNITY
// 
//    void Awake ()
//    {
//        IsServer = false;
//        ConnectedClientNumber = 0;
//        ScreenResolution = new IntVector2 ( 1024, 768 );
//        ScreenRefreshrate = 60;
//        ScreenParallax = 0.06f;
//        QualitySetting = "Fastest";
//        gameStarted = false;
//        LogOnScreen = false;
//        LogOnScreenClients = false;
//        LogOnFile = false;
//        IpDataFile = "IpData.conf";
//        WaitInEditorForClientsOrServer = false;
//        ConnectionRetries = 3;
//        caveGameServerPort = 25000;
//		
//		ClientData = new List<System.Object>();
//        NetView = networkView;
//     
//        StaticLogOnFile = LogOnFile;
//        StaticNetworkView = networkView;
//     
//        WriteNewFile ( "" );
//    }
// 
// 
//    void OnGUI ()
//    {
//        GUI.skin = InterfaceSkin;
//     
//        if ( IsServer )
//        {
//            if ( !IsStandalone () )
//            {
//                // upper right corner   - client number on server
//                GUI.Label ( new Rect ( Screen.width - 110, 10, 100, 20 ), "clients " + ( ConnectedClientNumber - 1 ) + " / " + ( ClientData.Count - 1 ), "Debug" );
//         
//                bool oldLogOnScreenClients = LogOnScreenClients;
//                LogOnScreenClients = GUI.Toggle ( new Rect ( Screen.width - 210, Screen.height - 70, 200, 20 ), LogOnScreenClients, " show debug log on clients" );
//                if ( LogOnScreenClients != oldLogOnScreenClients )
//                {
//                    NetView.RPC ( "RPCSetLogOnScreenSERVERONLY", RPCMode.OthersBuffered, LogOnScreenClients );
//                }
//            }   
//         
//            if ( !IsStandalone () || ( IsStandalone () && !Screen.lockCursor ) )
//            {
//                LogOnScreen = GUI.Toggle ( new Rect ( Screen.width - 210, Screen.height - 30, 200, 20 ), LogOnScreen, " show debug log" );
//            }
//        }
//     
//        if ( LogOnScreen )
//        {
//            GUIStyle style = GUIStyle.none;
//            // upper left corner    - debug log on screen
//            LogStringScrollPosition = GUILayout.BeginScrollView ( LogStringScrollPosition,
//             style, style, GUILayout.Width ( Screen.width ), GUILayout.Height ( Screen.height ) );
//            GUILayout.Label ( LogString, "Debug" );
//            GUILayout.EndScrollView ();
//        }
//     
//        if ( OwnClientData.ShowGuiText && GuiString.Length != 0 )
//        {
//            // centered             - gui text on clients
//            Vector2 spacing = new Vector2 ( 15, 5 );
//
//            //Vector2 spacing(15, 5);
//            Vector2 size = GUI.skin.GetStyle ( "GuiCenter" ).CalcSize ( new GUIContent ( GuiString ) );
//            GUI.Label ( new Rect ( ( Screen.width/2.0f ) - ( size.x/2.0f ) - spacing.x,
//                            ( Screen.height/2.0f ) - ( size.y/2.0f ) - spacing.y,
//                            size.x + 2.0f*spacing.x, size.y + 2.0f*spacing.y ), GuiString, "GuiCenter" );
//       }
//    }
//
//    #endregion
//    // =============================================================================
// 
// 
// 
//    // =============================================================================
//    // METHODS STATIC --------------------------------------------------------------
// 
//    // show as nice text and call rpc to show as nice text on all clients also
//    //TODO dont know if it can be used by clients
//    public static void SetGuiOnAllSERVERONLY ( string text )
//    {
//        StaticNetworkView.RPC ( "RPCSetGuiOnAllSERVERONLY", RPCMode.AllBuffered, text );
//    }
// 
//	#region log
//    // log and call rpc to log on all clients also
//    //TODO not tested yet
//    //TODO dont know if it can be used by clients
//    public static void LogWithAllSERVERONLY ( string text )
//    {
//        StaticNetworkView.RPC ( "RPCLogWithAllSERVERONLY", RPCMode.AllBuffered, text );
//    }
// 
// 
//    // log and show as nice text in center of screen
//    public static void LogWithGui ( string text )
//    {
//        Log ( text, false );
//        GuiString = text;
//    }
// 
// 
//    // log as no error
//    public static void Log ( string text )
//    {
//        Log ( text, false );
//    }
// 
// 
//    // static log function for console, debug-gui and file
//    // can be used instead of unity Debug.Log()
//    public static void Log ( string text, bool isError )
//    {
//        string prefix = "";
//        if ( isError )
//        { // log in unity console
//            Debug.LogError ( text );
//            prefix = "[ERR]";
//        }
//        else
//        {
//            Debug.Log ( text );
//            prefix = "     ";
//        }
//     
//        text = FormatText ( prefix, text, System.Environment.NewLine );
//     
//        LogString += text + System.Environment.NewLine; // log in debug-gui-interface
//        LogStringScrollPosition.y = Mathf.Infinity; // auto scroll text
//     
//        AppendToFile ( text ); // log in file
//    }
// 
// 
//    private static string FormatText ( string prefix, string text, string separator )
//    {
//        text = prefix + text;
//        text = text.Replace ( separator, separator + prefix );
//        return text;
//    }
// 
// 
//    private static void WriteNewFile ( string text )
//    {
//        if ( !StaticLogOnFile )
//        {
//            return;
//        }
//
//
//        try
//        {
//            StreamWriter sw = new System.IO.StreamWriter ( System.Net.Dns.GetHostName () + ".txt", false );
//            sw.WriteLine ( text );
//            sw.Close ();
//        }
//        catch ( Exception e )
//        {
//            Config.Log ( e.ToString (), true );
//        }
//    }
//
// 
//    private static void AppendToFile ( string text )
//    {
//        if ( !StaticLogOnFile )
//        {
//            return;
//        }
//     
//
//        try
//       {
//            StreamWriter sw = new System.IO.StreamWriter ( System.Net.Dns.GetHostName () + ".txt", true );
//            sw.WriteLine ( text );
//        }
//        catch ( Exception e )
//        {
//            Config.Log ( e.ToString (), true );
//        }
//    }
//    // =============================================================================
//	
// 
// 
//    // =============================================================================
//    // METHODS  --------------------------------------------------------------------
// 
//    public void OwnClientDataToString ()
//    {
//        Log ( OwnClientData.ToString () );
//        Log ( "screen: " + ScreenReal.ToString () );
//        Log ( "parallax: " + ScreenParallax );
//        Log ( "device: " + InputDevice );
//        Log ( "isServer: " + IsServer );
//    }
//	#endregion
//	
//	
//	//moved
//    public bool IsStandalone ()
//    {
//        return ( ClientData.Count <= 1 );
//    }
//    // =============================================================================
// 
// 
// 
//    // =============================================================================
//    // METHODS RPC -----------------------------------------------------------------
// 
//    [RPC]
//    void RPCSetLogOnScreenSERVERONLY ( bool showLog )
//    {
//        LogOnScreen = showLog;
//    }
// 
// 
//    [RPC]
//    void RPCSetGuiOnAllSERVERONLY ( string text )
//    {
//        GuiString = text;
//    }
// 
// 
//    [RPC]
//    void RPCLogWithAllSERVERONLY ( string text )
//    {
//        Log ( text, false );
//    }
//    // =============================================================================
//}