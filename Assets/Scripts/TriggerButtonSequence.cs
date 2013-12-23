
// Hannes Helmholz
//
// 

using UnityEngine;

[RequireComponent (typeof(NetworkView))] // for RPC
public class TriggerButtonSequence : MonoBehaviour
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]    private bool RestartAfterFinish = true;
    [SerializeField]    private TriggerButton[] Buttons;
    private ConfigClass Config;
    private NetworkView NetView;
    private int Current = -1;
    private bool Finished = false;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Start ()
    {
        Config = GameObject.FindWithTag ( "Config" ).GetComponent<ConfigClass> ();
     
        NetView = networkView;
     
        foreach ( TriggerButton button in Buttons )
        {
            button.SetAnimation ( animation ); // needs to be done
            button.enabled = false;
        }
     
        RPCSetCurrent ( 0 );
        Buttons[Current].SetAsStartingPoint ();
    }
 
 
    void Update ()
    {
        if ( !Config.IsServer )
        {
            return;
        }
         
        if ( IsCurrentTriggered () )
        {
            SetAllCurrentSERVERONLY ( Current + 1 );
        }
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------
     
    private bool IsCurrentTriggered ()
    {
        if ( Finished || !Config.IsServer )
        {
            return false;
        }
         
        return Buttons[Current].GetIsTriggered ();
    }
 
 
    private void SetAllCurrentSERVERONLY ( int newCurrent )
    {
        if ( newCurrent == Buttons.Length && RestartAfterFinish )
        {
            newCurrent = 0;
        }
         
        NetView.RPC ( "RPCSetCurrent", RPCMode.AllBuffered, newCurrent );
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS RPC -----------------------------------------------------------------
 
    [RPC]
    void RPCSetCurrent ( int newCurrent )
    {
        if ( Current >= 0 )
        {
            Buttons[Current].enabled = false;
        }   
         
        Current = newCurrent;
        if ( Current < Buttons.Length )
        {
            Buttons[Current].enabled = true;
        }
        else
        {
            Finished = true;
        }
    }
    // =============================================================================
}