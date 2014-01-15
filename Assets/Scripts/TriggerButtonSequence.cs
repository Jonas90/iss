
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
     	
		Debug.Log("noloop:");
		for(int i = 0; i <= Current; i++)
		{
			Debug.Log("loop:"+i);
			Buttons.Add(new TriggerButton());
			Debug.Log("count:"+Buttons.Count);
			Buttons[i].SetAnimation ( animation ); // needs to be done
			
								
			//NullReferenceException are thrown when you try to access a reference 
			//variable that isnt referencing any object, hence it is null.
            Buttons[i].enabled = false;
			
			//Now to how to fix them: obviously the easiest way to fix them is to not have 
			//any NullReferences. But many times thats not possible so you need to have either try-catch blocks or conditionals. 
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