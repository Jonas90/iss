
// Hannes Helmholz
//
// running on every client and server
// set camera settings by client data from config
// calculate new client camera screen when in standalone
using UnityEngine;

public class ClientCamera : MonoBehaviour
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    public bool IndividualPlanes = false;
    private ConfigClass Config;
    private Transform Trans;
    private Camera Cam;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Awake()
    {
        Config = GameObject.FindWithTag( "Config" ).GetComponent<ConfigClass>();
        Trans = transform;
        Cam = camera;
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS ---------------------------------------------------------------------
 
    public void SetOwnTransform()
    {
        Trans.Rotate( new Vector3( 0, Config.OwnClientData.AngleOffset, 0 ) );
 
        if( Config.IsStandalone() )
        {
            Resolution res = Screen.currentResolution;
            float h = Mathf.Tan( ( Cam.fieldOfView/2.0f )*Mathf.Deg2Rad )*Cam.nearClipPlane*2.0f;
            Config.ScreenReal = new ClientCameraScreen( res.width*h/res.height, h, Cam.nearClipPlane );
        }
     
        float offset = ( Config.ScreenParallax/2.0f )*Config.OwnClientData.ParallaxOffsetDirection;

        Vector3 v3T = transform.localPosition;
        v3T.x = offset;
        Trans.localPosition = v3T;


        ClientCameraVanishingPoint.SetVanishingPoint( Cam, -offset, Config.ScreenReal );
        ConfigClass.Log( "set camera " + gameObject.name + " (matrix: " + Cam.projectionMatrix + ")" );
    }
    // =============================================================================
}