
// Hannes Helmholz
//
// showing crosshair gui in world space
// 
// raycast stops on all colliders
// -> DISABLE "Raycast Hit Triggers" IN Edit/Project Setting/Physics
using UnityEngine;

[RequireComponent (typeof(NetworkView))] // for RPC

public class Crosshair : MonoBehaviour
{

    // =============================================================================
    // MEMBERS ---------------------------------------------------------------------
    [SerializeField]
    private Texture2D Tex;
    [SerializeField]
    private Vector2 TextureBaseSize = new Vector2( 130, 130 );
    [SerializeField]
    private Vector2 TextureMinSize = new Vector2( 25, 25 );
    [SerializeField]
    private float MovementSmooth = 5;
    [SerializeField]
    private Camera Cam;
    private ConfigClass Config;
    private Transform CamTransform;
    private NetworkView NetView;
    private Vector3 WorldPosition;
    private Vector3 Position;
    private Vector2 Size;
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS UNITY ---------------------------------------------------------------
 
    void Awake()
    {
        Config = GameObject.FindWithTag( "Config" ).GetComponent<ConfigClass>();
     
        CamTransform = Cam.transform;
        NetView = gameObject.networkView;
    }
 
 
    void Update()
    {
        if( !Config.IsServer )
        {
            return;
        }

        Ray ray = Cam.ScreenPointToRay( Input.mousePosition );
        RaycastHit hit;
        if( Physics.Raycast( ray, out hit ) )
        {
            NetView.RPC( "RPCUpdatePosition", RPCMode.AllBuffered, hit.point );
        }
        else
        {
         // point in infinity under cursor
            NetView.RPC( "RPCUpdatePosition", RPCMode.AllBuffered, ray.GetPoint( Cam.farClipPlane ) );
        }
    }
 
 
    void OnGUI()
    {
        if( Config.OwnClientData.AngleOffset != 0 )
        {
            return;
        }
         
        if( ( Config.IsServer && !Config.IsStandalone() ) || ( Config.IsStandalone() && !Screen.lockCursor ) )
        {
            return;
        }
         
        Position = Vector3.Lerp( Position, Cam.WorldToScreenPoint( WorldPosition ), Time.deltaTime*MovementSmooth );
     
        var newSize = TextureBaseSize/( CamTransform.position - WorldPosition ).magnitude;
        if( newSize.x < TextureMinSize.x )
        {
            newSize.x = TextureMinSize.x;
        }
        if( newSize.y < TextureMinSize.y )
        {
            Size.y = TextureMinSize.y;
        }  
        Size = Vector2.Lerp( Size, newSize, Time.deltaTime*MovementSmooth );
     
        GUI.DrawTexture( new Rect( Position.x - ( Size.x/2 ), Position.y - ( Size.y/2 ), Size.x, Size.y ), Tex, ScaleMode.ScaleToFit );
    }
    // =============================================================================
 
 
 
    // =============================================================================
    // METHODS RPC -----------------------------------------------------------------
 
    [RPC]
    void RPCUpdatePosition( Vector3 worldPosition )
    {
        WorldPosition = worldPosition;
    }
    // =============================================================================
}
