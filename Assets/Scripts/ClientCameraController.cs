
// Hannes Helmholz
//
// 
using UnityEngine;

public class ClientCameraController : MonoBehaviour
{
 
    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    private ConfigClass Config;
    private Camera ClientCamera;
    private Camera ClientCameraBackground;
    private Camera ClientCameraEquipment;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Start()
    {
        Config = GameObject.FindWithTag( "Config" ).GetComponent<ConfigClass>();
     
        ClientCamera = transform.FindChild( "Camera" ).GetComponent<Camera>();
        ClientCameraBackground = transform.FindChild( "CameraBackground" ).GetComponent<Camera>();
        ClientCameraEquipment = transform.FindChild( "CameraEquipment" ).GetComponent<Camera>();
    }
 
 
    void Update()
    {
        if( !Config.IsServer )
        {
            return;
        }
     
        ClientCamera.enabled = Config.OwnClientData.ShowMainCamera;
        ClientCameraBackground.enabled = !Config.OwnClientData.ShowMainCamera;
        ClientCameraEquipment.enabled = Config.OwnClientData.ShowMainCamera;
    }
 
 
    void OnGUI()
    {
        GUI.skin = Config.InterfaceSkin;
     
        if( Config.IsServer && !Config.IsStandalone() )
        {
            Config.OwnClientData.ShowMainCamera = GUI.Toggle( new Rect( Screen.width - 210, Screen.height - 50, 200, 20 ),
             Config.OwnClientData.ShowMainCamera, " render main camera" );
        }
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS STATIC --------------------------------------------------------------
 
    public static void SetAllTransforms( GameObject player )
    { 
        ClientCamera[] cams = player.transform.GetComponentsInChildren<ClientCamera>();
        foreach( ClientCamera cam in cams )
            cam.SetOwnTransform();
    }
    // =============================================================================
}