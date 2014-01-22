
// Hannes Helmholz
//
// 

using UnityEngine;
using System.Collections.Generic;

[RequireComponent (typeof(NetworkView))] // for RPC
public class TriggerButtonSequence : MonoBehaviour
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]    private bool RestartAfterFinish = true;
    [SerializeField]    private List<TriggerButton> Buttons;
    private Config Config;
    private NetworkView NetView;
    private int Current = 0; //dan war -1
    private bool Finished = false;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Start ()
    {
        Config = GameObject.FindWithTag ( "Config" ).GetComponent<Config> ();
     	Buttons = new List<TriggerButton>();		
		
        NetView = networkView;
     	
		//Debug.Log("noloop:");
		for(int i = 0; i <= Current; i++)
		{
			//Debug.Log("loop:"+i+ " / "+Current);
			Buttons.Add(new TriggerButton());
			//Debug.Log("count:"+Buttons.Count);
			Buttons[i].SetAnimation ( animation ); // needs to be done
            Buttons[i].enabled = false;
		}
     
		
		
        RPCSetCurrent ( 0 );
        Buttons[Current].SetAsStartingPoint ();
    }
 
 
    void Update ()
    {
        if ( !Config.IsServer ) return;
         
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
        if ( Finished || !Config.IsServer ) return false;         
        return Buttons[Current].GetIsTriggered ();
    }
 
 
    private void SetAllCurrentSERVERONLY ( int newCurrent )
    {
        if ( newCurrent == Buttons.Count && RestartAfterFinish )
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
        if ( Current > 0 )
        {
            Buttons[Current].enabled = false;
        }   
         
        Current = newCurrent;
        if ( Current < Buttons.Count )
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