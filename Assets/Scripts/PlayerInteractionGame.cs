
// Hannes Helmholz
//
// manage interaction of player to manage game state
// in kombination with InputManager

using UnityEngine;

[RequireComponent (typeof(NetworkView))] // for RPC

public class PlayerInteractionGame : MonoBehaviour
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]
    private GameController GameController;
    [SerializeField]
    private InteractionKey ButtonStart;
    [SerializeField]
    private InteractionKey ButtonReset;
    [SerializeField]
    private InteractionKey ButtonStop;
    public string ButtonQuit = "escape";
    private ConfigClass Config;
    private NetworkView NetView;
    private GameController.GameState LastState;
    private string ButtonsString = "";
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Awake ()
    {
        Config = GameObject.FindWithTag ( "Config" ).GetComponent<ConfigClass> ();
     
        NetView = networkView;
        LastState = GameController.GetState ();
     
        ButtonStart.Initialize ();
        ButtonReset.Initialize ();
        ButtonStop.Initialize ();
    }
 
 
    void Update ()
    {
        if ( Input.GetKey ( ButtonQuit ) )
        {
         // can be done with every input device and also by clients
            GameController.EndGame ();
        }
 
        if ( !Config.IsServer )
        {
            return;
        }
     
        CheckControllerChange ();
     
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
 
         
    void OnGUI ()
    {
        GUI.skin = Config.InterfaceSkin;
     
        if ( Config.OwnClientData.ShowGuiCamera || Config.IsServer )
        {
         // top centered         - button instructions
            GUI.Label ( new Rect ( Screen.width/2.0f, 40, 0, 0 ), ButtonsString, "Buttons" );
        }
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS  --------------------------------------------------------------------
 
    private void CheckControllerChange ()
    {
        if ( !Config.IsStandalone () || GameController.GetState () != GameController.GameState.WaitForFirstStart )
        {
            return;
        }
         
        if ( Config.InputDevice != ConfigClass.Device.Keyboard && OneKeyboardKeyPressed () )
        {
            ConfigClass.Log ( "change input mode from " + Config.InputDevice + " to " + ConfigClass.Device.Keyboard );
            ConfigClass.Log ( "====================" );
            Config.InputDevice = ConfigClass.Device.Keyboard;
        }
    }
 
 
    private bool OneKeyboardKeyPressed ()
    {
        return ButtonStart.GetButtonDown ( ConfigClass.Device.Keyboard ) ||
             ButtonReset.GetButtonDown ( ConfigClass.Device.Keyboard ) ||
             ButtonStop.GetButtonDown ( ConfigClass.Device.Keyboard );
    }
 
 
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
        GameController.GameState state = GameController.GetState ();
        switch ( state )
        {
            case GameController.GameState.WaitForFirstStart:
                if ( Config.InputDevice != ConfigClass.Device.Keyboard )
                {
                    text += "Press  " + ButtonStart.GetCurrentButton ( ConfigClass.Device.Keyboard ) +
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
    private void SetButtonsOnAllSERVERONLY ( string text )
    {
        NetView.RPC ( "RPCSetButtonsOnAllSERVERONLY", RPCMode.AllBuffered, text );
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS RPC -----------------------------------------------------------------
 
    [RPC]
    void RPCSetButtonsOnAllSERVERONLY ( string text )
    {
        ButtonsString = text;
    }
    // =============================================================================
}