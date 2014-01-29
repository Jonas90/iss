
// Hannes Helmholz
//
// 

using UnityEngine;
using System.Collections;

public class TriggerFirePort : TriggerAbstract
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]
    private string ObjectDescriptionExtinguish; // optional
 
    private MeterFirePort MeasurenentMeter;
    private MeterFireExtinguisher Extinguisher;
    private string ObjectDescriptionMeasure;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------    
 
    void Awake()
    {
        base.Awake();
     
        MeasurenentMeter = FindObjectOfType( typeof(MeterFirePort) ) as MeterFirePort;
        Extinguisher = FindObjectOfType( typeof(MeterFireExtinguisher) ) as MeterFireExtinguisher;
     
        ObjectDescriptionMeasure = ObjectDescription;
    }


    void OnMouseEnter()
    {
        if( MeasurenentMeter.IsEquipped() )
        {
            NetView.RPC( "RPCSetObjectDescription", RPCMode.AllBuffered, ObjectDescriptionMeasure );
            base.OnMouseEnter();
     
        }
        else if( Extinguisher.IsEquipped() )
		{
			NetView.RPC( "RPCSetObjectDescription", RPCMode.AllBuffered, ObjectDescriptionExtinguish );
			base.OnMouseEnter();
		}
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------

    protected new void DoTrigger()
    {
        // StatusLast should not be reset on server
        // because focus still on FirePort but Highlighting should be off during measurement
        bool last = statusLast;
        NetView.RPC( "RPCSetStatus", RPCMode.AllBuffered, false );
        statusLast = last;
     
        if( MeasurenentMeter.IsEquipped() )
        {
            MeasurenentMeter.MeasureValues( gameObject.name, transform );
        }
        else if( Extinguisher.IsEquipped() )
		{
			Extinguisher.Extinguish( transform );
		}
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS RPC -----------------------------------------------------------------
 
    [RPC]
    void RPCSetObjectDescription( string text )
    {
        ObjectDescription = text;
    }
    // =============================================================================
}