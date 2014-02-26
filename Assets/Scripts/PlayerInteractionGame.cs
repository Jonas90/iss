
// Hannes Helmholz
//
// manage interaction of player to manage game state
// in kombination with InputManager

using UnityEngine;

[RequireComponent (typeof(NetworkView))] // for RPC


/* Klasse ist zuständig fuer die Eingabegeräte um mit dem Programm zu interagieren.
 * Speziell zum Starten, Stoppen und Restarten des Spiels.
 * Ermoeglicht ein umschalten zwischen Gamecontroller und Tastatur als Eingabegerät, zur Laufzeit.
 */
public class PlayerInteractionGame : MonoBehaviour
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    
	//TODO: fix keys, gameController
	[SerializeField]    private GameController GameController;
    [SerializeField]    private InteractionKey ButtonStart;
    [SerializeField]    private InteractionKey ButtonReset;
    [SerializeField]    private InteractionKey ButtonStop;
	
    public string ButtonQuit = "escape";
    private Config Config;
    private NetworkView NetView;
    private GameController.GameState LastState;
    private string ButtonsString = "";
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 

	/* Initialisierung der Objekte ButtonStart, ButtonReset und ButtonStop.
	 * Der GameController GameController wird auf Instance gesetzt.
	 * Die Variable LastState wird auf den Status von GameController gesetzt, welcher beim Start der Klasse aktiv ist.
	 */
    void Awake ()
    {
        Config = GameObject.FindWithTag ( "Config" ).GetComponent<Config> ();
     
        NetView = networkView;
//dan
		
		Debug.Log(">>>>>PlayerInteractionGame_Awake");
		GameController = GameController.Instance;
        LastState = GameController.GetState ();
     
		ButtonStart = new InteractionKey(InteractionKey.MouseKey.Right, "[0]", "START" , "(1)");
        ButtonStart.Initialize ();
		ButtonReset = new InteractionKey(InteractionKey.MouseKey.None, "[0]", "START" , "(1)");
        ButtonReset.Initialize ();
		ButtonStop = new InteractionKey(InteractionKey.MouseKey.None, "[1]", "BACK" , "(2)");
        ButtonStop.Initialize ();
	}

 
 	/* Fragt die eingehenden Signale regelmaessig ab.
 	 * Laeuft die Anwendung nicht in der Cave, wird aus dieser Funktion herausgesprungen.
 	 * Fragt auch ab ob die Tastatur oder der Gamecontroller verwendet wird.
 	 * Ist die Anwendung am laufen und der Stop-Knopf wurde gedruekt, so wird die Anwendung gestoppt.
 	 * Ist die Anwengung am laufen und der Reset-Knof wurde gedruekt, so wird die Anwendung neu gestartet.
 	 * Wartet die Anwendung auf den (ersten) Start und der Start-Knopf wurde gedruekt, so startet das Spiel.
 	 * 
 	 */ 
    void Update ()
    {
        if ( Input.GetKey ( ButtonQuit ) )
        {
         // can be done with every input device and also by clients
			//dan
			//GameController.EndGame ();
        }
 
        if ( !Config.IsServer )
        {
            return;
        }
     
        CheckControllerChange ();

//dan
        GameController.GameState state = GameController.GetState ();
        if ( ButtonStop.GetButtonDown () && state == GameController.GameState.Started )
        {
            GameController.StopGame ();
        }
        else if ( ButtonReset.GetButtonDown () && state == GameController.GameState.Started )
        {
            GameController.RestartGame ();
        }
        else if ( ButtonStart.GetButtonDown ()
         && ( state == GameController.GameState.WaitForStart || state == GameController.GameState.WaitForFirstStart ) )
        {
            GameController.StartGame ();
        }
     
        CheckStateChange ();
    }
 
//to kommentieren: Durch merge haben wir gui von 6ter etage
         
    void OnGUI ()
    {
//dan
//        GUI.skin = Config.InterfaceSkin;
//     
//        if ( Config.OwnClientData.ShowGuiCamera || Config.IsServer )
//        {
//         // top centered         - button instructions
//            GUI.Label ( new Rect ( Screen.width/2.0f, 40, 0, 0 ), ButtonsString, "Buttons" );
//        }
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS  --------------------------------------------------------------------
 
	/* Ueberwacht die Verwendung von Tastatur oder Gamecontroller als Eingabegeraet.
	 * Wird waehrent der Laufzeit ein anderes Eingabegeraet verwendet, wird darauf reagiert indem 
	 * nun auf Eingaben des neuen Eingabegeraetes gewartet wird.
	 */
    private void CheckControllerChange ()
    {
        if ( !Config.IsStandalone )//dan  || GameController.GetState () != GameController.GameState.WaitForFirstStart )
        {
            return;
        }
        //dan 
        if ( Config.InputDevice != Config.Device.Keyboard && OneKeyboardKeyPressed () )
        {
            
//			Config.Log ( "change input mode from " + Config.InputDevice + " to " + Config.Device.Keyboard );
//          Config.Log ( "====================" );
            Config.InputDevice = Config.Device.Keyboard;
        }
    }
 
//dan 
	/* Liefert true zurueck falls auf der Tastatur der als Startknopf definierter Knopf,
	 * der als Resetknopf definierter Knopf oder der als Stopknopf definierter Knopf gedrueckt wird. 
	 */
    private bool OneKeyboardKeyPressed ()
    {
        return ButtonStart.GetButtonDown ( Config.Device.Keyboard ) ||
             ButtonReset.GetButtonDown ( Config.Device.Keyboard ) ||
             ButtonStop.GetButtonDown ( Config.Device.Keyboard );
    }
 
//dan 
	/* Ueberwacht den Status des Spiels.
	 * Dabei werden folgende Zustaende unterschieden:
	 * 	- WaitForClientsSERVERONLY:	Der Server wartet auf die Clients.
	 * 	- WaitForFirstStart:		Es wird auf den ersten Start des Spiels gewartet.
	 *  - WaitForStart:				Es wird auf den Start des Spiels gewartet.
	 * 	- Started:					Das Spiel ist gestartet. 
	 */
    private void CheckStateChange ()
    {
        GameController.GameState currentState = GameController.GetState ();
        if ( currentState != LastState )
        {
            SetButtonsOnAllSERVERONLY ( GetButtonString () );
        }
         
        LastState = currentState;
    }
 
 
    // string for button instructions on screen by GameState
    private string GetButtonString ()
    {
        string text = "";
//dan		
        GameController.GameState state = GameController.GetState ();
        switch ( state )
        {
            case GameController.GameState.WaitForFirstStart:
                if ( Config.InputDevice != Config.Device.Keyboard )
                {
                    text += "Press  " + ButtonStart.GetCurrentButton ( Config.Device.Keyboard ) +
                         "  to Play    and    change to Keyboard" + System.Environment.NewLine;
                }
                break;
            case GameController.GameState.WaitForStart:
                text += "Press  " + ButtonStart.GetCurrentButton () + "  to Play";
                break;
            case GameController.GameState.Started:
                text += "Press  " + ButtonReset.GetCurrentButton () + "  to Restart      ";
                text += "Press  " + ButtonStop.GetCurrentButton () + "  to Stop";
                break;
            default:
                break;
        }
     
        return text;
    }
 
 
    // show button instructions and call rpc to show button instructions on all clients also
    //TODO dont know if it can be used by clients
	/* Sendet an den RPC einen String, der auf allen Clients angezeigt werden soll.
	 * @param text String der an alle Clients gesendet wird.
	 */
    private void SetButtonsOnAllSERVERONLY ( string text )
    {
        NetView.RPC ( "RPCSetButtonsOnAllSERVERONLY", RPCMode.AllBuffered, text );
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS RPC -----------------------------------------------------------------
 
	/* Weist ButtonString einem String zu.
	 * @param text String der ButtonString zugewiesen wird.
	 */
    [RPC]
    void RPCSetButtonsOnAllSERVERONLY ( string text )
    {
        ButtonsString = text;
    }
    // =============================================================================
}