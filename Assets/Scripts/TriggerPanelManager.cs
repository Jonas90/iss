
// Hannes Helmholz
// Dieser TriggerManager managt alle Triggerpanels und stellt sicher das diese mit einander interagieren.
// 

using UnityEngine;

[RequireComponent (typeof(NetworkView))] // for RPC

public class TriggerPanelManager : MonoBehaviour
{

    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    private NetworkView NetView;
    private bool State = false;
    private TriggerPanel[] Panels;
    // ============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    protected virtual void Awake ()
    {
        NetView = networkView;
 
        Panels = gameObject.GetComponentsInChildren<TriggerPanel> ();
    }
 
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------

    public void SetState ( bool state )
    {
        NetView.RPC ( "RPCSetState", RPCMode.AllBuffered, state );
    }
 
 
    private void SetPanels ( bool state )
    {
        foreach ( TriggerPanel panel in Panels )
            panel.SetState ( state );
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS RPC -----------------------------------------------------------------
 
    [RPC]
    void RPCSetState ( bool state )
    {
        State = state;
     
        SetPanels ( State );
    }
    // =============================================================================
}