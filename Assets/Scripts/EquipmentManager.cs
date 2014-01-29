
// Hannes Helmholz
//
// give different possibilities to interact with scene
// with different key for every input device
// in combination with InputManager

using UnityEngine;

[RequireComponent(typeof(NetworkView))] // for RPC

public class EquipmentManager : MonoBehaviour
{

    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    private enum HelpState
    {
        NoEquipment,
        ShowHelp,
        HideHelp
    }
 	
	//dan
	//todo fix buttons
//    [SerializeField]    private InteractionKey ButtonNext; // optional
//    [SerializeField]    private InteractionKey ButtonLast; // optional
    [SerializeField]    private Equipment[] AdditionalEquipment; // optional
 
    private Config Config;
    private NetworkView NetView;
    private Equipment[] Equipments;
    private int CurrentShown = -1;
    private HelpState State;
    private string ButtonsString = "";
    // =============================================================================



    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Start ()
    {
        Config = GameObject.FindWithTag ( "Config" ).GetComponent<Config> ();
     
        NetView = networkView;
     
        Equipments = gameObject.GetComponentsInChildren<Equipment> ();
        int oldSize = Equipments.Length;
        System.Array.Resize<Equipment> ( ref Equipments, oldSize + AdditionalEquipment.Length );
        for ( int i = 0; i < AdditionalEquipment.Length; i++ )
        {
            Equipments[oldSize + i] = AdditionalEquipment[i];
        }
     
//        ButtonNext.Initialize ();
//        ButtonLast.Initialize ();
    }
 
 
    void Update ()
    {
        if ( !Config.IsServer || !Config.gameStarted )
        {
            return;
        }
         
//        if ( ButtonNext.GetButtonDown () )
//        {
//            Next ();
//        }
//        else if ( ButtonLast.GetButtonDown () )
//        {
//            Last ();
//        }
    }
 
 
    void OnGUI ()
    {
//        GUI.skin = Config.InterfaceSkin;
//     
//        if ( Config.OwnClientData.AngleOffset == 0 || Config.IsServer )
//        {
//            // bottom centered          - button instructions
//            GUI.Label ( new Rect ( Screen.width/2.0f, Screen.height - 50f, 0f, 0f ), ButtonsString, "Buttons" );
//        }
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS  --------------------------------------------------------------------

    public void Take ( Equipment equipment )
    {
		Debug.Log ("----- is in EquipmentManager -> Take (Equipment equipment):"+equipment);
        if ( !equipment )
        {
            return;
        }
     
        equipment.SetAvailable ( true ); // see also EquipmentAtMercy
     
        if ( equipment.IsAvailable () )
        {
            // important for EquipmentAtMercy
            for ( int i = 0; i < Equipments.Length; i++ )
            {
                if ( Equipments[i] == equipment )
                {
                    Equipments[i].TriggerStored ();
                 
                    if ( !( Equipments[i] as EquipmentAsGui ) )
                    {
                        if ( CurrentShown >= 0 )
                        {
                            Equipments[CurrentShown].TriggerStored ();
                        }
                 
                        CurrentShown = i;
                     
                        if ( State == HelpState.NoEquipment )
                        {
                            State = HelpState.ShowHelp;
                            SetButtonsOnAllSERVERONLY ( GetButtonString () );
                        }
                    } 
                 
                    continue;
                }
            }
        }
    }


    private void Next ()
    {
        for ( int i = CurrentShown + 1; i < Equipments.Length; i++ )
        {
            if ( TryChange ( i ) )
            {
                return;
            }
        }
             
        for ( int i = -1; i < CurrentShown; i++ )
        {
            if ( TryChange ( i ) )
            {
                return;
            }
        }
    }
 
 
    private void Last ()
    {
        for ( int i = CurrentShown - 1; i >= -1; i-- )
        {
            if ( TryChange ( i ) )
            {
                return;
            }
        }
             
        for ( int i = Equipments.Length - 1; i > CurrentShown; i-- )
        {
            if ( TryChange ( i ) )
            {
                return;
            }
        }
    }
 
 
    private bool TryChange ( int i )
    {
        if ( i == -1 || ( Equipments[i].IsAvailable () && !Equipments[i].IsEquipped () ) )
        {
            if ( CurrentShown > -1 )
            {
                Equipments[CurrentShown].TriggerStored ();
            }
             
            if ( i > -1 )
            {
                Equipments[i].TriggerStored ();
            }
             
            CurrentShown = i;
         
            if ( State == HelpState.ShowHelp )
            {
                State = HelpState.HideHelp;
                SetButtonsOnAllSERVERONLY ( "" );
            }
         
            return true;
        }
     
        return false;
    }
 
 
    // string for button instructions on screen by GameState
    private string GetButtonString ()
    {
        string text = "";
//        text += "Press  " + ButtonLast.GetCurrentButton () + "   or   ";
//        text += ButtonNext.GetCurrentButton () + "  to change equipment";
//     
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