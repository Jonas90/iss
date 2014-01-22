
// Hannes Helmholz
//
//

using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Collider))] // for trigger of mouse
[RequireComponent (typeof(NetworkView))] // for RPC

public /*abstract*/ class TriggerAbstract : FunkyGlowingThingsElement
{

    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]    protected HoveringText HoverText; // optional
    [SerializeField]    protected Vector3 HoverTextOffset; // optional
	//dan
	//TODO: fix keys
	//    [SerializeField]    protected InteractionKey Button; // optional
    [SerializeField]    protected string ObjectDescription; // optional
 
    protected Collider Coll;
    // ============================================================================
 
	public TriggerAbstract()
	{
		HoverTextOffset = new Vector3 ( 0f, 0f, -0.15f );
	}
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    protected virtual void Start ()
    {
        base.Start ();
//dan		
//		Button = new InteractionKey();		
//        Button.Initialize ();
     
        Coll = collider;
    }
 
 
    protected virtual void Update ()
    {
        base.Update ();
     
        if ( !config.IsServer )
        {
            return;
        }
         
        if ( GetIsTriggered () )
        {
            DoTrigger ();
        }
    }
 
 
    protected virtual void OnDisable ()
    {
        base.RPCSetStatus ( false );
     
        if ( HoverText )
        {
            HoverText.Hide ();
        }
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------
 
    public new bool GetIsTriggered ()
    {
        //dan
		//todo
		//return base.GetIsTriggered () && Button.GetButtonDown ();
		return base.GetIsTriggered();
    }
 
 
    // for override
    protected void DoTrigger ()
    {
        NetView.RPC ( "RPCDoTrigger", RPCMode.AllBuffered );
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS RPC -----------------------------------------------------------------
 
    [RPC]
    void RPCSetStatus ( bool status )
    {
        base.RPCSetStatus ( status );
 
		//dan
//        if ( HoverText )
//        {
//            if ( status )
//            {
//                string text = "";
//                if ( Button.GetCurrentButton ().Length != 0 )
//                {
//                    text += /*"System.Environment.NewLine +*/ "press button " + Button.GetCurrentButton ( " or " );
//                }
//                if ( ObjectDescription.Length != 0 )
//                {
//                    text += System.Environment.NewLine + ObjectDescription;
//                }
//             
//                HoverText.ViewOnTarget ( Trans, HoverTextOffset, text );
//            }
//            else
//            {
//                HoverText.Hide ();
//            }
//        }
    }
 
 
    [RPC]
    // for override
    protected void RPCDoTrigger (){}
    // =============================================================================
}