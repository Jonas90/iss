//dan

//
//// Hannes Helmholz


using UnityEngine;

/** Verwaltet die Rotation des Spielers
 *  Unterscheidet dabei zwischen Standalone- und Servermodus.
 */
public class PlayerRotation : MouseLook
{

    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [HideInInspector]
    public bool TemporaryDisable = false;
    private Config Config;
    private bool DoRotation = false;
    // =============================================================================
 
 
 	// komplett auskommentiert -> wieder einkommenteirt! 15.01
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Awake ()
    {
        Config = GameObject.FindWithTag ( "Config" ).GetComponent<Config> ();
    }
 
 
    new void Update ()
    {
        if ( TemporaryDisable )
        {
            return;
        }
 
        if ( Config.IsStandalone && Input.GetMouseButtonDown ( 1 ) )
        { // right mouse key
            DoRotation = !DoRotation;
            Screen.lockCursor = DoRotation;
        }
     
        if ( DoRotation )
        {
            axisX = ( Config.InputDevice == Config.Device.Keyboard ) ? "Mouse X" : "LookHorizontal";
            axisY = ( Config.InputDevice == Config.Device.Keyboard ) ? "Mouse Y" : "LookVertical";
         
            base.Update ();
        }
    }
 
 
    void OnEnable ()
    {
        DoRotation = true;
     
        if ( Config.IsStandalone )
        {
            Screen.lockCursor = true;
        }
    }
 
 
    void OnDisable ()
    {
        Screen.lockCursor = false;
    }
    // =============================================================================
}