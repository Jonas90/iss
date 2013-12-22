
// Hannes Helmholz
//
//

using UnityEngine;

public class PlayerRotation : MouseLook
{

    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [HideInInspector]
    public bool TemporaryDisable = false;
    private ConfigClass Config;
    private bool DoRotation = false;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Awake ()
    {
        Config = GameObject.FindWithTag ( "Config" ).GetComponent<ConfigClass> ();
    }
 
 
    new void Update ()
    {
        if ( TemporaryDisable )
        {
            return;
        }
 
        if ( Config.IsStandalone () && Input.GetMouseButtonDown ( 1 ) )
        { // right mouse key
            DoRotation = !DoRotation;
            Screen.lockCursor = DoRotation;
        }
     
        if ( DoRotation )
        {
            axisX = ( Config.InputDevice == ConfigClass.Device.Keyboard ) ? "Mouse X" : "LookHorizontal";
            axisY = ( Config.InputDevice == ConfigClass.Device.Keyboard ) ? "Mouse Y" : "LookVertical";
         
            base.Update ();
        }
    }
 
 
    void OnEnable ()
    {
        DoRotation = true;
     
        if ( Config.IsStandalone () )
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