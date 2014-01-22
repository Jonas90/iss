
// Hannes Helmholz
//
// 

using UnityEngine;

public class TriggerPanel : TriggerAbstract
{

    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]
    private string ObjectDescriptionOff;
    private string ObjectDescriptionOn;
    private bool State = false;
    private TriggerPanelButton[] Buttons;
    private TriggerPanelManager Manager;
    // ============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    protected virtual void Start ()
    {
        base.Start ();
     
        Buttons = GetComponentsInChildren<TriggerPanelButton> ();
        Manager = FindObjectOfType ( typeof( TriggerPanelManager) ) as TriggerPanelManager;
     
        ObjectDescriptionOn = ObjectDescription;
    }
 
 
    protected virtual void OnMouseEnter ()
    {
        if ( State )
        {
            NetView.RPC ( "RPCSetObjectDescription", RPCMode.AllBuffered, ObjectDescriptionOff );
        }
        else
        {
            NetView.RPC ( "RPCSetObjectDescription", RPCMode.AllBuffered, ObjectDescriptionOn );
        }
     
        base.OnMouseEnter ();
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------
 
    public void SetState ( bool state )
    {
        State = state;
         
        base.DoTrigger ();
    }
 
 
    protected new void DoTrigger ()
    {
        Manager.SetState ( !State );
     
        // StatusLast should not be reset on server
        // because focus still on FirePort but Highlighting should be off during measurement
        bool last = statusLast;
        NetView.RPC ( "RPCSetStatus", RPCMode.AllBuffered, false );
        statusLast = last;
    }
 
 
    private void TriggerButtons ()
    {
        foreach ( TriggerPanelButton button in Buttons )
            button.Trigger ();
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS RPC -----------------------------------------------------------------
 
    [RPC]
    void RPCDoTrigger ()
    {
        base.RPCDoTrigger ();
     
        TriggerButtons ();
    }
 
 
    [RPC]
    void RPCSetObjectDescription ( string text )
    {
        ObjectDescription = text;
    }
    // =============================================================================
}